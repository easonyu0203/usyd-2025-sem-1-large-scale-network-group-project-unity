using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

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
    public List<List<float>> Matrix = new List<List<float>>();
}

public class GraphDataManager : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://localhost:5001/api";
    
    private SetupResponse _setupData;
    private CorrMatrixResponse _currentCorrMatrixData;
    private bool _isInitialized = false;
    
    public event Action OnSetupComplete;
    public event Action OnCorrMatrixDataUpdated;
    
    public bool IsInitialized => _isInitialized;
    public List<string> Tickers => _setupData?.Tickers;
    public List<string> Dates => _setupData?.Dates;
    public List<List<float>> CurrentCorrMatrix => _currentCorrMatrixData.Matrix;
    public string CurrentDate => _currentCorrMatrixData.Date;

    private void Start()
    {
        StartCoroutine(InitializeSetup());
    }
    
    private IEnumerator InitializeSetup()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{serverUrl}/setup"))
        {
            // Send request and wait for response
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                _setupData = JsonConvert.DeserializeObject<SetupResponse>(response);
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
        string url = $"{serverUrl}/corr_matrix/{date}?window_size={windowSize}";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                _currentCorrMatrixData = JsonConvert.DeserializeObject<CorrMatrixResponse>(response);
                
                Debug.Log($"Received graph data for {date}");
                OnCorrMatrixDataUpdated?.Invoke();
            }
            else
            {
                Debug.LogError($"Graph data request failed: {webRequest.error}");
            }
        }
    }
}