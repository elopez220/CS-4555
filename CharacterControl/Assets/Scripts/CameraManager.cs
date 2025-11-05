// CameraManager.cs - Handles split-screen setup with fixed camera POV
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera player1Camera;
    public Camera player2Camera;
    public Transform player1Transform;
    public Transform player2Transform;
    
    [Header("Camera Positioning")]
    public float cameraHeight = 3f;
    public float cameraDistance = 8f;
    public float cameraAngle = 15f; // Look down angle
    
    // Store initial forward directions
    private Vector3 player1InitialForward;
    private Vector3 player2InitialForward;
    private bool initialized = false;
    
    void Start()
    {
        SetupSplitScreenCameras();
    }
    
    void SetupSplitScreenCameras()
    {
        // Create Player 1 Camera if it doesn't exist
        if (player1Camera == null)
        {
            GameObject cam1 = new GameObject("Player1Camera");
            player1Camera = cam1.AddComponent<Camera>();
        }
        
        // Create Player 2 Camera if it doesn't exist
        if (player2Camera == null)
        {
            GameObject cam2 = new GameObject("Player2Camera");
            player2Camera = cam2.AddComponent<Camera>();
        }
        
        // Set up Player 1 Camera (Top half of screen)
        player1Camera.rect = new Rect(0, 0.5f, 1, 0.5f);
        player1Camera.depth = 0;
        
        // Set up Player 2 Camera (Bottom half of screen)
        player2Camera.rect = new Rect(0, 0, 1, 0.5f);
        player2Camera.depth = 1;
        
        // Store initial facing directions
        if (player1Transform != null)
            player1InitialForward = player1Transform.forward;
        if (player2Transform != null)
            player2InitialForward = player2Transform.forward;
        
        initialized = true;
        
        // Position cameras based on player's initial forward direction
        PositionCamera(player1Camera, player1Transform, player1InitialForward);
        PositionCamera(player2Camera, player2Transform, player2InitialForward);
        
        // Add camera labels (optional)
        AddCameraLabels();
    }
    
    void PositionCamera(Camera cam, Transform player, Vector3 facingDirection)
    {
        if (player != null)
        {
            // Position camera behind the player based on their initial facing direction
            Vector3 behindOffset = -facingDirection * cameraDistance;
            behindOffset.y = cameraHeight; // Add height
            
            Vector3 desiredPosition = player.position + behindOffset;
            cam.transform.position = desiredPosition;
            
            // Look at a point in front of the player based on initial facing direction
            Vector3 lookTarget = player.position + facingDirection * 5f + Vector3.up * 1f;
            cam.transform.LookAt(lookTarget);
            
            // Apply additional downward tilt
            cam.transform.rotation *= Quaternion.Euler(cameraAngle, 0, 0);
        }
    }
    
    void Update()
    {
        if (!initialized) return;
        
        // Keep cameras positioned relative to players but maintain initial facing direction
        if (player1Transform != null)
            PositionCamera(player1Camera, player1Transform, player1InitialForward);
        if (player2Transform != null)
            PositionCamera(player2Camera, player2Transform, player2InitialForward);
    }
    
    void AddCameraLabels()
    {
        // Add UI text to identify which camera is which
        CreateCameraLabel(player1Camera, "Player 1", new Vector2(10, -10));
        CreateCameraLabel(player2Camera, "Player 2", new Vector2(10, 10));
    }
    
    void CreateCameraLabel(Camera cam, string text, Vector2 offset)
    {
        GameObject canvas = new GameObject(text + "Canvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
        canvasComponent.worldCamera = cam;
        
        GameObject textObj = new GameObject(text + "Text");
        textObj.transform.SetParent(canvas.transform);
        
        UnityEngine.UI.Text textComponent = textObj.AddComponent<UnityEngine.UI.Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.white;
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = offset;
    }
}