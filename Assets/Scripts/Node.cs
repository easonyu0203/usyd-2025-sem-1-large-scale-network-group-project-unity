using UnityEngine;

public class Node : MonoBehaviour
{
    // Unique identifier or other properties
    public string NodeId { get; private set; }

    public void Initialize(string id, Vector3 position)
    {
        NodeId = id;
        transform.position = position;
    }
}
