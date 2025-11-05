// AnimatedAttackTroop.cs - Attack troops with animation support
using UnityEngine;

public class AnimatedAttackTroop : MonoBehaviour
{
    [Header("Troop Settings")]
    public int ownerPlayerNumber = 1;
    public TroopType troopType = TroopType.Regular;
    public float moveSpeed = 2.5f;
    public int health = 2;
    public int damage = 5;
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    
    [Header("Animation")]
    public Animator animator;
    
    [Header("Animation Parameters (set these to match your animator)")]
    public string walkAnimationName = "Walk";
    public string attackAnimationName = "Attack";
    public string deathAnimationName = "Death";
    public string idleAnimationName = "Idle";
    
    [Header("Visual")]
    public TrailRenderer trail;
    public GameObject troopModel; // The downloaded asset
    
    private Transform targetBase;
    private BaseHealth targetBaseHealth;
    private float lastAttackTime = 0f;
    private bool hasReachedTarget = false;
    private bool isDead = false;
    
    public enum TroopType
    {
        Regular,
        Tank,
        LongRange
    }

    void Start()
    {
        SetupTroopStats();
        SetupAnimator();
        SetupVisuals();
        FindEnemyBase();
        PlayWalkAnimation();
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
    
    void SetupAnimator()
    {
        // Try to find animator if not assigned
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        
        if (animator == null && troopModel != null)
        {
            animator = troopModel.GetComponent<Animator>();
        }
        
        if (animator == null)
        {
            Debug.LogWarning("No animator found on " + gameObject.name + ". Animations will not play.");
        }
    }
    
    void SetupVisuals()
    {
        // Color the model based on player
        if (troopModel != null)
        {
            Renderer[] renderers = troopModel.GetComponentsInChildren<Renderer>();
            Color playerColor = ownerPlayerNumber == 1 ? 
                new Color(0.3f, 0.3f, 1f) : new Color(1f, 0.3f, 0.3f);
            
            foreach (Renderer rend in renderers)
            {
                // Create new material instance to avoid changing original
                Material[] mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = new Material(mats[i]);
                    mats[i].color = Color.Lerp(mats[i].color, playerColor, 0.5f);
                }
                rend.materials = mats;
            }
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
        
        Color troopColor = ownerPlayerNumber == 1 ? 
            new Color(0.3f, 0.3f, 1f, 0.9f) : new Color(1f, 0.3f, 0.3f, 0.9f);
        trail.startColor = troopColor;
        trail.endColor = troopColor;
    }

    void FindEnemyBase()
    {
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
        if (isDead) return;
        
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
        
        // Face movement direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Check if reached base
        if (Vector3.Distance(transform.position, targetBase.position) < attackRange)
        {
            hasReachedTarget = true;
            PlayIdleAnimation();
            Debug.Log("Attack troop reached enemy base!");
        }
    }
    
    void AttackBase()
    {
        if (targetBaseHealth == null) return;
        
        // Face the base
        Vector3 direction = (targetBase.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Attack on cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayAttackAnimation();
            targetBaseHealth.TakeDamage(damage);
            lastAttackTime = Time.time;
            Debug.Log("Player " + ownerPlayerNumber + " troop attacked enemy base for " + damage + " damage!");
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        health -= damageAmount;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("Attack troop destroyed!");
        
        PlayDeathAnimation();
        
        // Destroy after animation completes (adjust time based on your death animation length)
        Destroy(gameObject, 2f);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dart"))
        {
            Dart dart = collision.gameObject.GetComponent<Dart>();
            if (dart != null && dart.ownerPlayerNumber != ownerPlayerNumber)
            {
                TakeDamage(1);
            }
        }
    }
    
    // ===== ANIMATION METHODS =====
    
    void PlayWalkAnimation()
    {
        if (animator == null) return;
        
        // Try different methods to play walk animation
        if (animator.runtimeAnimatorController != null)
        {
            // Method 1: Using Play()
            animator.Play(walkAnimationName);
        }
        else
        {
            Debug.LogWarning("Animator has no controller assigned!");
        }
    }
    
    void PlayAttackAnimation()
    {
        if (animator == null) return;
        
        // Play attack animation
        animator.Play(attackAnimationName);
        
        // Return to idle after attack
        Invoke("PlayIdleAnimation", attackCooldown * 0.5f);
    }
    
    void PlayDeathAnimation()
    {
        if (animator == null) return;
        
        animator.Play(deathAnimationName);
    }
    
    void PlayIdleAnimation()
    {
        if (animator == null) return;
        
        animator.Play(idleAnimationName);
    }
}