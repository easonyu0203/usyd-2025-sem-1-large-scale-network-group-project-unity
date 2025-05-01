using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ForceDirectAlgo : MonoBehaviour
{
    [Header("Setting")]
    [Tooltip("optimal distance between nodes")]
    [SerializeField] private float ideaDistance = 4.0f;
    [Tooltip("Idea distance coefficient")]
    [SerializeField] private float ideaDistCoef = 0.01f;
    [Tooltip("l2 regulize to (0,0,0)")]
    [SerializeField] private float l2_coef = 1f;
    
    private Vector3[] _netForces;
    private int _n; // node count
    private Dictionary<Vector3Int, List<int>> _spatialGrid; // Spatial grid for optimization

    private void Awake()
    {
        GraphDataManager graphDataManager = GetComponent<GraphDataManager>();
        graphDataManager.OnSetupComplete += () =>
        {
            // init
            _n = graphDataManager.Tickers.Count;
            _netForces = new Vector3[_n];
            _spatialGrid = new Dictionary<Vector3Int, List<int>>();
        };
    }
    

    public void ComputeForce(Node[] nodes, Edge[,] edges)
    {
        // init force to 0
        Array.Fill(_netForces, Vector3.zero);

        // loop through each pair
        for (int u = 0; u < _n - 1; u++)
        {
            Vector3 posU = nodes[u].Position;
            for (int v = u + 1; v < _n; v++)
            {
                Vector3 posV = nodes[v].Position;
                float w_uv = edges[u, v].Weight;
                Vector3 delta = posV - posU; // Vector from u to v
                Vector3 direction = delta.normalized;
                float d_uv = delta.magnitude; // Euclidean distance
                
                // force for "to idea distance", with consideration of edge's weight
                float f_to_idea = (1 - math.abs(w_uv)) * ideaDistance - d_uv;

                // aggregate forces
                _netForces[u] += direction * (-f_to_idea * ideaDistCoef);
                _netForces[v] += direction * (f_to_idea * ideaDistCoef);
            }
        }
        
        // apply l2 reg
        for (int u = 0; u < _n; u++)
        {
            _netForces[u] += nodes[u].Position * (-l2_coef);
        }
        
        // Apply net forces to rigidbodies
        for (int u = 0; u < _n; u++)
        {
            nodes[u].rb.AddForce(_netForces[u], ForceMode.Force);
        }
    }
}