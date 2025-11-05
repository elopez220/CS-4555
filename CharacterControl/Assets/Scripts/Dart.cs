// Dart.cs - FIXED version with proper physics
using UnityEngine;

public class Dart : MonoBehaviour
{
    public float damage = 1f;
    public GameObject popEffect;
    public int ownerPlayerNumber = 1;
    
    private Rigidbody rb;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // CRITICAL: Ensure proper physics settings
        rb.useGravity = true;
        rb.mass = 0.1f;
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    void OnEnable()
    {
        hasHit = false;
        
        // Reset physics when dart is reused from pool
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;
        
        // Check for balloons
        if (collision.gameObject.CompareTag("Balloon"))
        {
            Balloon balloon = collision.gameObject.GetComponent<Balloon>();
            if (balloon != null)
            {
                balloon.SetOwner(ownerPlayerNumber);
                balloon.PopBalloon();
            }
            Destroy(gameObject);
            return;
        }
        
        // Check for balloon troops
        BalloonTroop balloonTroop = collision.gameObject.GetComponent<BalloonTroop>();
        if (balloonTroop != null && balloonTroop.ownerPlayerNumber != ownerPlayerNumber)
        {
            balloonTroop.TakeDamage(1);
            Debug.Log("Dart hit enemy balloon troop!");
            Destroy(gameObject);
            return;
        }
        
        // Check for attack troops
        AttackTroop attackTroop = collision.gameObject.GetComponent<AttackTroop>();
        if (attackTroop != null && attackTroop.ownerPlayerNumber != ownerPlayerNumber)
        {
            attackTroop.TakeDamage(1);
            Debug.Log("Dart hit enemy attack troop!");
            Destroy(gameObject);
            return;
        }
        
        // Check for animated attack troops
        AnimatedAttackTroop animatedTroop = collision.gameObject.GetComponent<AnimatedAttackTroop>();
        if (animatedTroop != null && animatedTroop.ownerPlayerNumber != ownerPlayerNumber)
        {
            animatedTroop.TakeDamage(1);
            Debug.Log("Dart hit enemy animated troop!");
            Destroy(gameObject);
            return;
        }
        
        // Hit ground or wall
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject, 0.1f);
        }
    }
}