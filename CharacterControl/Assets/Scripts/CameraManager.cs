// CameraManager.cs - Handles split-screen setup
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera player1Camera;
    public Camera player2Camera;
    public Transform player1Transform;
    public Transform player2Transform;
    
    [Header("Camera Positioning")]
    public Vector3 cameraOffset = new Vector3(0, 5, -8);
    public float cameraHeight = 3f;
    public float cameraDistance = 8f;
    
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
        
        // Position cameras
        PositionCamera(player1Camera, player1Transform);
        PositionCamera(player2Camera, player2Transform);
        
        // Add camera labels (optional)
        AddCameraLabels();
    }
    
    void PositionCamera(Camera cam, Transform player)
    {
        if (player != null)
        {
            // Position camera behind and above player
            Vector3 desiredPosition = player.position + new Vector3(0, cameraHeight, -cameraDistance);
            cam.transform.position = desiredPosition;
            
            // Look at player
            cam.transform.LookAt(player.position + Vector3.up);
        }
    }
    
    void Update()
    {
        // Keep cameras positioned relative to players
        if (player1Transform != null)
            PositionCamera(player1Camera, player1Transform);
        if (player2Transform != null)
            PositionCamera(player2Camera, player2Transform);
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
