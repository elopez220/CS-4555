// AttackTroop.cs - Monkey troops that attack enemy base (with troop types)
using UnityEngine;

public class AttackTroop : MonoBehaviour
{
    [Header("Troop Settings")]
    public int ownerPlayerNumber = 1;
    public TroopType troopType = TroopType.Regular;
    public float moveSpeed = 2.5f;
    public int health = 2;
    public int damage = 5;
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    
    [Header("Visual")]
    public TrailRenderer trail;
    
    private Transform targetBase;
    private BaseHealth targetBaseHealth;
    private float lastAttackTime = 0f;
    private bool hasReachedTarget = false;
    
    public enum TroopType
    {
        Regular,    // Balanced
        Tank,       // High health, slow
        LongRange   // Attacks from distance
    }

    void Start()
    {
        SetupTroopStats();
        SetupVisuals();
        FindEnemyBase();
    }
    
    void SetupTroopStats()
    {
        switch (troopType)
        {
            case TroopType.Regular:
                moveSpeed = 2.5f;
                health = 2;
                damage = 5;
                attackRange = 2f;
                attackCooldown = 2f;
                break;
            case TroopType.Tank:
                moveSpeed = 1.5f;
                health = 5;
                damage = 7;
                attackRange = 2f;
                attackCooldown = 2.5f;
                break;
            case TroopType.LongRange:
                moveSpeed = 2f;
                health = 1;
                damage = 4;
                attackRange = 5f;
                attackCooldown = 1.5f;
                break;
        }
    }
    
    void SetupVisuals()
    {
        // Set color based on owner and type
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Color baseColor = ownerPlayerNumber == 1 ? 
                new Color(0.2f, 0.2f, 1f) : new Color(1f, 0.2f, 0.2f);
            
            // Modify color based on type
            switch (troopType)
            {
                case TroopType.Tank:
                    baseColor = Color.Lerp(baseColor, Color.black, 0.3f);
                    break;
                case TroopType.LongRange:
                    baseColor = Color.Lerp(baseColor, Color.yellow, 0.2f);
                    break;
            }
            
            renderer.material.color = baseColor;
        }
        
        // Setup trail
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }
        
        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trail.material = trailMaterial;
        trail.startWidth = 0.2f;
        trail.endWidth = 0.05f;
        trail.time = 0.8f;
        
        // Darker trail colors
        Color troopColor = ownerPlayerNumber == 1 ? 
            new Color(0.3f, 0.3f, 1f, 0.9f) : new Color(1f, 0.3f, 0.3f, 0.9f);
        trail.startColor = troopColor;
        trail.endColor = troopColor;
    }

    void FindEnemyBase()
    {
        // Find enemy base (opposite player number)
        BaseHealth[] bases = FindObjectsByType<BaseHealth>(FindObjectsSortMode.None);
        foreach (BaseHealth baseHealth in bases)
        {
            if (baseHealth.playerNumber != ownerPlayerNumber)
            {
                targetBase = baseHealth.transform;
                targetBaseHealth = baseHealth;
                Debug.Log("Attack troop found enemy base at: " + targetBase.position);
                break;
            }
        }
        
        if (targetBase == null)
        {
            Debug.LogError("AttackTroop could not find enemy base!");
        }
    }
    
    void Update()
    {
        if (!hasReachedTarget)
        {
            MoveToTarget();
        }
        else
        {
            AttackBase();
        }
    }
    
    void MoveToTarget()
    {
        if (targetBase == null) return;
        
        // Move toward enemy base
        Vector3 direction = (targetBase.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Check if reached base (using attack range)
        if (Vector3.Distance(transform.position, targetBase.position) < attackRange)
        {
            hasReachedTarget = true;
            Debug.Log("Attack troop reached enemy base!");
        }
    }
    
    void AttackBase()
    {
        if (targetBaseHealth == null) return;
        
        // Attack on cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            targetBaseHealth.TakeDamage(damage);
            lastAttackTime = Time.time;
            Debug.Log("Player " + ownerPlayerNumber + " troop attacked enemy base for " + damage + " damage!");
            
            // Visual feedback for long range
            if (troopType == TroopType.LongRange)
            {
                // Could add projectile effect here
            }
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        
        if (health <= 0)
        {
            Debug.Log("Attack troop destroyed!");
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dart"))
        {
            TakeDamage(1);
        }
    }
}