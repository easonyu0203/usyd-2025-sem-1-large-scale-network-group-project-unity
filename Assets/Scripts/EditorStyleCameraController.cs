using UnityEngine;

/// <summary>
/// Camera controller that mimics Unity editor camera controls.
/// Supports orbiting around origin with Alt+LMB and zooming with scroll wheel.
/// </summary>
public class EditorStyleCameraController : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitSpeed = 5f;
    [SerializeField] private Vector3 targetPoint = Vector3.zero;
    
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoomDistance = 10f;
    [SerializeField] private float maxZoomDistance = 100f;
    
    // Internal variables for camera movement
    private float currentDistance;
    private Vector3 lastMousePosition;
    private float xRotation;
    private float yRotation;
    
    private void Start()
    {
        // Initialize rotations based on current camera orientation
        Vector3 direction = transform.position - targetPoint;
        currentDistance = direction.magnitude;
        
        // Calculate current rotation angles
        yRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        xRotation = Mathf.Asin(direction.y / currentDistance) * Mathf.Rad2Deg;
        
        // Ensure we're starting at the correct distance
        UpdateCameraPosition();
    }
    
    private void LateUpdate()
    {
        HandleOrbit();
        HandleZoom();
    }
    
    private void HandleOrbit()
    {
        // Check if Alt key is held and left mouse button is pressed
        bool isOrbiting = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(0);
        
        if (isOrbiting)
        {
            // Get mouse delta
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            
            // Apply rotations based on mouse movement
            yRotation += mouseDelta.x * orbitSpeed * Time.deltaTime;
            xRotation -= mouseDelta.y * orbitSpeed * Time.deltaTime;
            
            // Clamp vertical rotation to prevent flipping
            xRotation = Mathf.Clamp(xRotation, -89f, 89f);
            
            // Update camera position based on new rotation
            UpdateCameraPosition();
        }
        
        // Store current mouse position for next frame
        lastMousePosition = Input.mousePosition;
    }
    
    private void HandleZoom()
    {
        // Get scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollInput != 0)
        {
            // Adjust distance based on scroll
            currentDistance -= scrollInput * zoomSpeed;
            
            // Clamp distance to min/max values
            currentDistance = Mathf.Clamp(currentDistance, minZoomDistance, maxZoomDistance);
            
            // Update camera position with new distance
            UpdateCameraPosition();
        }
    }
    
    private void UpdateCameraPosition()
    {
        // Convert spherical coordinates to Cartesian
        float x = currentDistance * Mathf.Sin(yRotation * Mathf.Deg2Rad) * Mathf.Cos(xRotation * Mathf.Deg2Rad);
        float y = currentDistance * Mathf.Sin(xRotation * Mathf.Deg2Rad);
        float z = currentDistance * Mathf.Cos(yRotation * Mathf.Deg2Rad) * Mathf.Cos(xRotation * Mathf.Deg2Rad);
        
        // Set camera position
        transform.position = new Vector3(x, y, z) + targetPoint;
        
        // Make camera look at the target point
        transform.LookAt(targetPoint);
    }
}