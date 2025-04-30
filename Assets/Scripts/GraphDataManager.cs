using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System.Text;

[Serializable]
public class SetupResponse
{
    public List<string> Tickers = new List<string>();
    public List<string> Dates = new List<string>();
}

[Serializable]
public class CorrMatrixResponse
{
    public string Date;
    public float[,] Matrix;
}

public class GraphDataManager : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://localhost:5001/api";

    private SetupResponse _setupData;
    private CorrMatrixResponse _currentCorrMatrixData;
    private bool _isInitialized = false;
    
    // Reusable string builders to avoid allocations
    private readonly StringBuilder _urlBuilder = new StringBuilder(128);
    
    public event Action OnSetupComplete;
    public event Action OnCorrMatrixDataUpdated;
    
    public bool IsInitialized => _isInitialized;
    public List<string> Tickers => _setupData?.Tickers;
    public List<string> Dates => _setupData?.Dates;
    public float[,] CurrentCorrMatrix => _currentCorrMatrixData?.Matrix;
    public string CurrentDate => _currentCorrMatrixData?.Date;

    // Cache strings that are frequently used
    private readonly string _apiCorr = "/corr_matrix/";
    private readonly string _windowSizeParam = "?window_size=";

    private void Start()
    {
        StartCoroutine(InitializeSetup());
    }
    
    private IEnumerator InitializeSetup()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{serverUrl}/setup"))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                _setupData = JsonConvert.DeserializeObject<SetupResponse>(response);
                
                // Initialize _currentCorrMatrixData with preallocated matrix
                int n = _setupData!.Tickers.Count;
                _currentCorrMatrixData = new CorrMatrixResponse
                {
                    Matrix = new float[n, n]
                };
                
                _isInitialized = true;
                
                Debug.Log($"Setup successful. Found {_setupData?.Tickers.Count} tickers and {_setupData?.Dates.Count} dates.");
                OnSetupComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"Setup request failed: {webRequest.error}");
            }
        }
    }
    
    public void RequestCorrMatrix(string date, int windowSize)
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("Cannot request graph data before initialization is complete.");
            return;
        }
        
        StartCoroutine(FetchCorrMatrixData(date, windowSize));
    }
    
    private IEnumerator FetchCorrMatrixData(string date, int windowSize)
    {
        // Clear and reuse StringBuilder instead of string concatenation
        _urlBuilder.Clear();
        _urlBuilder.Append(serverUrl);
        _urlBuilder.Append(_apiCorr);
        _urlBuilder.Append(date);
        _urlBuilder.Append(_windowSizeParam);
        _urlBuilder.Append(windowSize);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_urlBuilder.ToString()))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                DeserializeCorrMatrixResponse(response, _currentCorrMatrixData);
                
                // Use cached strings for debug logs
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Received graph data for " + date);
                }
                OnCorrMatrixDataUpdated?.Invoke();
            }
            else
            {
                Debug.LogError("Graph data request failed: " + webRequest.error);
            }
        }
    }
    
    // Static cache for property names to avoid string allocations during parsing
    private static readonly string _dateProperty = "date";
    private static readonly string _matrixProperty = "matrix";
    
    private void DeserializeCorrMatrixResponse(string json, CorrMatrixResponse response)
    {
        using (StringReader stringReader = new StringReader(json))
        using (JsonTextReader reader = new JsonTextReader(stringReader))
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName)
                        {
                            string propertyName = (string)reader.Value;
                            
                            // Use string reference comparison instead of ToString()
                            if (propertyName == _dateProperty)
                            {
                                reader.Read();
                                response.Date = (string)reader.Value;
                            }
                            else if (propertyName == _matrixProperty)
                            {
                                int n = _setupData.Tickers.Count;
                                reader.Read(); // Start of matrix array
                                for (int i = 0; i < n; i++)
                                {
                                    reader.Read(); // Start of row array
                                    for (int j = 0; j < n; j++)
                                    {
                                        double? value = reader.ReadAsDouble();
                                        if (value.HasValue)
                                        {
                                            float floatValue = (float)value.Value;
                                            response.Matrix[i, j] = float.IsNaN(floatValue) ? 0f : floatValue;
                                        }
                                        else
                                        {
                                            response.Matrix[i, j] = 0f;
                                            reader.Skip(); // Skip invalid token
                                        }
                                    }
                                    reader.Read(); // End of row array
                                }
                                reader.Read(); // End of matrix array
                            }
                        }
                        else if (reader.TokenType == JsonToken.EndObject)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}