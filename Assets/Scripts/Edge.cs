using System;
using UnityEngine;

public class Edge : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Node _startNode; // Store references to nodes for potential updates
    private Node _endNode;
    private bool _isInitialized;
    public float Weight { get; set; }

    private void Update()
    {
        if (_isInitialized == false)
        {
            return;
        }
        
        // update position
        _lineRenderer.SetPosition(0, _startNode.Position);
        _lineRenderer.SetPosition(1, _endNode.Position);
    }

    public void Initialize(Node startNode, Node endNode, float weight)
    {
        _startNode = startNode;
        _endNode = endNode;
        Weight = weight;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startNode.transform.position);
        _lineRenderer.SetPosition(1, endNode.transform.position);
        // Additional LineRenderer settings (e.g., width, material)
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _isInitialized = true;
    }

    public void UpdatePosition(Vector3 startPos, Vector3 endPos)
    {
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }

    // Set the visibility of the edge
    public void SetVisibility(bool isVisible)
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = isVisible;
        }
    }
}
