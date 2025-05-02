using System.Collections.Generic;
using UnityEngine;

public class GraphFactory : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;
    [Tooltip("Each sector's node material")]
    [SerializeField] private List<Material> sectorMaterials;

    // Create a Node
    public Node CreateNode(string id, Vector3 position, int sectorIdx)
    {
        GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity);
        Node node = nodeObj.GetComponent<Node>();
        node.Initialize(id, position);

        // Set the node's material based on sectorIdx
        if (sectorIdx >= 0 && sectorIdx < sectorMaterials.Count && sectorMaterials[sectorIdx] != null)
        {
            Renderer renderer = nodeObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = sectorMaterials[sectorIdx];
            }
            else
            {
                Debug.LogWarning($"Node {id} has no Renderer component to apply sector material");
            }
        }
        else
        {
            Debug.LogWarning($"Invalid sector index {sectorIdx} or no material defined for node {id}");
        }

        return node;
    }

    // Create an Edge between two Nodes
    public Edge CreateEdge(Node startNode, Node endNode)
    {
        GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity);
        Edge edge = edgeObj.GetComponent<Edge>();
        edge.Initialize(startNode, endNode, 0);
        return edge;
    }
}