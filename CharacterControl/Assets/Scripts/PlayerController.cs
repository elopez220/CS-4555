// PlayerController.cs - Clean version with aim line (no dart trails)
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerNumber = 1;
    public float rotationSpeed = 50f;
    public GameObject dartPrefab;
    public Transform dartSpawnPoint;
    public float dartForce = 20f;
    
    [Header("Input Keys")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode shootKey = KeyCode.W;

    // Optional second control set (useful for local multiplayer)
    public KeyCode left2Key = KeyCode.J;
    public KeyCode right2Key = KeyCode.L;
    public KeyCode shoot2Key = KeyCode.I;
    
    [Header("Aiming Line")]
    public LineRenderer aimLine;
    public float aimRange = 25f;
    public bool showAimLine = true;
    
    [Header("Shooting Cooldown")]
    public float shootCooldown = 0.5f;
    
    private float currentRotation = 0f;
    private float initialRotation = 0f;
    private float lastShootTime = -999f;
    public ObjectPool dartPool;

    void Start()
    {
        currentRotation = transform.rotation.eulerAngles.y;
        initialRotation = currentRotation;

        if (dartSpawnPoint != null && dartSpawnPoint.parent != transform)
        {
            dartSpawnPoint.SetParent(transform);
        }

        SetupAimingLine();
    }

    void SetupAimingLine()
    {
        // Try to reuse existing LineRenderer
        if (aimLine == null)
        {
            aimLine = GetComponent<LineRenderer>();
        }

        if (aimLine == null)
        {
            aimLine = gameObject.AddComponent<LineRenderer>();
        }

        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startWidth = 0.12f;
        aimLine.endWidth = 0.04f;
        aimLine.positionCount = 2;
        aimLine.useWorldSpace = true;

        // Color by player
        Color aimColor = playerNumber == 1 ? new Color(0.3f, 0.5f, 1f, 0.9f) : new Color(1f, 0.4f, 0.3f, 0.9f);
        aimLine.startColor = aimColor;
        aimLine.endColor = aimColor;

        aimLine.enabled = showAimLine;
    }

    void Update()
    {
        HandleInput();
        UpdateAimingLine();
    }

    void HandleInput()
    {
        // Rotation (either control set)
        if (Input.GetKey(leftKey) || Input.GetKey(left2Key))
        {
            currentRotation -= rotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Clamp(currentRotation, -45f, 45f);
        }
        if (Input.GetKey(rightKey) || Input.GetKey(right2Key))
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Clamp(currentRotation, -45f, 45f);
        }

        transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        // Shooting with cooldown (either shoot key)
        if ((Input.GetKeyDown(shootKey) || Input.GetKeyDown(shoot2Key)) && Time.time - lastShootTime >= shootCooldown)
        {
            ShootDart();
            lastShootTime = Time.time;
        }
    }

    void UpdateAimingLine()
    {
        if (aimLine != null && dartSpawnPoint != null && showAimLine)
        {
            Vector3 startPos = dartSpawnPoint.position;
            Vector3 shootDirection = transform.forward;
            Vector3 endPos = startPos + shootDirection * aimRange;

            aimLine.SetPosition(0, startPos);
            aimLine.SetPosition(1, endPos);
        }
    }

    void ShootDart()
    {
        if (dartSpawnPoint == null)
        {
            Debug.LogWarning("PlayerController: dartSpawnPoint is null. Cannot shoot.");
            return;
        }

        GameObject dart = null;

        // Prefer pool if available
        if (dartPool != null)
        {
            dart = dartPool.GetObject();
            if (dart == null)
            {
                Debug.LogWarning("PlayerController: dartPool returned null. Falling back to Instantiate.");
            }
        }

        if (dart == null)
        {
            if (dartPrefab == null)
            {
                Debug.LogWarning("PlayerController: dartPrefab is null and no pooled object available.");
                return;
            }
            dart = Instantiate(dartPrefab);
        }

        dart.transform.position = dartSpawnPoint.position;
        dart.transform.rotation = dartSpawnPoint.rotation;
        dart.SetActive(true);

        Rigidbody rb = dart.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Use forward of spawn point if available otherwise use transform.forward
            Vector3 forceDir = dartSpawnPoint.forward != Vector3.zero ? dartSpawnPoint.forward : transform.forward;
            // Resetting velocity may use different API across Unity versions; avoid explicit reset to minimize compatibility issues.
            rb.AddForce(forceDir * dartForce, ForceMode.Impulse);
        }

        AddTrailToDart(dart);

        // Schedule deactivation: if pooled return to pool, else destroy
        StartCoroutine(DeactivateDart(dart, dartPool != null));
    }

    IEnumerator DeactivateDart(GameObject dart, bool isPooled)
    {
        yield return new WaitForSeconds(3f);
        if (dart == null) yield break;

        if (isPooled && dartPool != null)
        {
            dartPool.ReturnObject(dart);
        }
        else
        {
            Destroy(dart);
        }
    }

    void AddTrailToDart(GameObject dart)
    {
        if (dart == null) return;

        TrailRenderer trail = dart.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = dart.AddComponent<TrailRenderer>();
        }

        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startWidth = 0.08f;
        trail.endWidth = 0.02f;
        trail.time = 0.8f;

        // Color trails by player
        Color startCol = playerNumber == 1 ? Color.cyan : Color.yellow;
        Color endCol = new Color(startCol.r, startCol.g, startCol.b, 0f);

        // Use Gradient if available
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startCol, 0.0f), new GradientColorKey(endCol, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        trail.colorGradient = g;
    }
}