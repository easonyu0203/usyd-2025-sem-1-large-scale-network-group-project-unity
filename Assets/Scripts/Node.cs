using UnityEngine;

public class Node : MonoBehaviour
{
    // Unique identifier or other properties
    public string Ticker { get; private set; }

    public void Initialize(string ticker, Vector3 position)
    {
        Ticker = ticker;
        transform.position = position;
    }
}
