using UnityEngine;

public class Edge : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Node _startNode; // Store references to nodes for potential updates
    private Node _endNode;
    public float Weight { get; set; }

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
