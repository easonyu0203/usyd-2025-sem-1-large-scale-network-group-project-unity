using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Transform _transform;
    
    public Rigidbody rb;
    public Vector3 Position => _transform.position;
    
    public string Ticker { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    public void Initialize(string ticker, Vector3 position)
    {
        Ticker = ticker;
        transform.position = position;
    }
}
