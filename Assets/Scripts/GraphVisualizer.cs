using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GraphVisualizer : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)]
    [Tooltip("only display edge that its weight above threshold")]
    [SerializeField] private float threshold = 0.5f;
    [Tooltip("the half length of the volume cube [-half_length, half_length]^3")]
    [SerializeField] private float half_length = 10.0f;
    
    [Header("Prefab Configuration")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;
    
    private GraphDataManager _graphDataManager;
    private GraphFactory _graphFactory;
    
    private bool _needsVisibilityUpdate; // Flag to queue visibility update
    
    // Graph storage
    private Node[] _nodes;
    private Edge[,] _edges;
    
    private void Awake()
    {
        _graphFactory = GetComponent<GraphFactory>();
        _graphDataManager = GetComponent<GraphDataManager>();
        _graphDataManager.OnSetupComplete += GraphSetup;
        _graphDataManager.OnCorrMatrixDataUpdated += UpdateGraph;
    }
    
    private void OnValidate()
    {
        // Queue visibility update
        if (_edges != null && _nodes != null)
        {
            _needsVisibilityUpdate = true;
        }
    }
    
    private void Update()
    {
        // Process queued visibility update
        if (_needsVisibilityUpdate)
        {
            UpdateVisibility();
            _needsVisibilityUpdate = false;
        }
    }

    private void GraphSetup()
    {
        List<string> tickers = _graphDataManager.Tickers;
        int n = tickers.Count;
        
        // Initialize node array
        _nodes = new Node[n];
        for (int i = 0; i < n; i++)
        {
            // Generate random position within [-half_length, half_length]^3
            Vector3 randomPosition = new Vector3(
                Random.Range(-half_length, half_length),
                Random.Range(-half_length, half_length),
                Random.Range(-half_length, half_length)
            );
            _nodes[i] = _graphFactory.CreateNode(tickers[i], randomPosition);
        }

        // Initialize edge and weight matrices
        _edges = new Edge[n, n];
        
        // Create edges for every pair (i,j) where i < j
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                Edge edge = _graphFactory.CreateEdge(_nodes[i], _nodes[j]);
                _edges[i, j] = edge;
                _edges[j, i] = edge; // Same edge object for undirected graph
                edge.SetVisibility(false); // Initially hidden
            }
        }
    }

    private void UpdateGraph()
    {
        // update weight for every edges
        int n = _nodes.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                Edge e = _edges[i, j];
                e.Weight = _graphDataManager.CurrentCorrMatrix[i,j];
            }
        }

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (_nodes == null)
        {
            return;
        }
        
        int n = _nodes.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                Edge e = _edges[i, j];
                bool visible = math.abs(e.Weight) >= threshold;
                e.SetVisibility(visible);
            }
        }
    }

    public void SetThreshold(float value)
    {
        value = math.min(math.max(value, 0.0f), 1.0f);
        threshold = value;

        UpdateVisibility();
    }
}
