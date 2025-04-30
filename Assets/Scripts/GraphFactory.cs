using UnityEngine;

public class GraphFactory : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;

    // Create a Node
    public Node CreateNode(string id, Vector3 position)
    {
        GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity);
        Node node = nodeObj.GetComponent<Node>();
        node.Initialize(id, position);
        return node;
    }

    // Create an Edge between two Nodes
    public Edge CreateEdge(Node startNode, Node endNode)
    {
        GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
        Edge edge = edgeObj.GetComponent<Edge>();
        edge.Initialize(startNode, endNode);
        return edge;
    }
}