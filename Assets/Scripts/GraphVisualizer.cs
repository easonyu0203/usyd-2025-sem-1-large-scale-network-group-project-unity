using System;
using System.Collections.Generic;
using System.Linq;
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
    
    private void Awake()
    {
        _graphFactory = GetComponent<GraphFactory>();
        _graphDataManager = GetComponent<GraphDataManager>();
        _graphDataManager.OnSetupComplete += GraphSetup;
        _graphDataManager.OnCorrMatrixDataUpdated += UpdateGraph;
    }

    private void GraphSetup()
    {
        List<string> tickers = _graphDataManager.Tickers;
        // tickers = tickers.Take(10).ToList();

        // Create nodes for each ticker at random positions
        Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        foreach (string ticker in tickers)
        {
            // Generate random position within [-half_length, half_length]^3
            Vector3 randomPosition = new Vector3(
                Random.Range(-half_length, half_length),
                Random.Range(-half_length, half_length),
                Random.Range(-half_length, half_length)
            );
            Node node = _graphFactory.CreateNode(ticker, randomPosition);
            nodes[ticker] = node;
        }

        // Create edges for every pair of nodes
        for (int i = 0; i < tickers.Count; i++)
        {
            for (int j = i + 1; j < tickers.Count; j++)
            {
                Node node1 = nodes[tickers[i]];
                Node node2 = nodes[tickers[j]];
                Edge edge = _graphFactory.CreateEdge(node1, node2);
                // Optionally set initial visibility based on threshold (if weights are available)
                edge.SetVisibility(true); // Default to hidden; update in UpdateGraph if needed
            }
        }
    }

    private void UpdateGraph()
    {
        
    }
}
