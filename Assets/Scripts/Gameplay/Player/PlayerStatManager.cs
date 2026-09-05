using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManager : MonoBehaviour
{
    [Header("- Health -")]
    [SerializeField] [Min(0)]
     private int health = 100;
    [SerializeField] [Min(1)]
     private int maxHealth = 100;
    [SerializeField] [Min(0f)]
    private float regenerationTriggerDelay = 2f;
    [SerializeField] [Min(0f)]
     private float regenerationsPerSec = 10f;
    [SerializeField] [Range(0, 100)]
    private int healthPerStep = 20;

    [Header("- Attack -")]
    [SerializeField] [Min(1.5f)]
     private float rangedAttackReach = 15f;
    [SerializeField] [Min(0.5f)]
     private float meleeAttackReach = 0.75f;
    [SerializeField] [Min(0f)]
     private float meleeAttackCooldown = 0.5f;
    [SerializeField] [Min(0)]
     private int meleeAttackDamage = 2;

    [Header("- Misc -")]
    [SerializeField] private Image damageOverlay = null;
    [SerializeField] [Min(0f)]
     private float pickupReach = 5f;
    [SerializeField] [Min(0f)]
     private float throwForce = 10f;
    [SerializeField] private bool isDead = false;

    private float regenerationTimer = 0f;
    private int healthCpy = 0;
    private int healthBuffer = 0;
    private bool regenerationDelayTrigger = true;
    private bool isDeadCpy = false;

    public float RangedAttackReach
    {
        get => rangedAttackReach;
    }
    public float MeleeAttackReach
    {
        get => meleeAttackReach;
    }
    public float MeleeAttackCooldown
    {
        get => meleeAttackCooldown;
    }
    public int MeleeAttackDamage
    {
        get => meleeAttackDamage;
    }

    public float PickupReach
    {
        get => pickupReach;
    }
    public float ThrowForce
    {
        get => throwForce;
    }


    private void Awake()
    {
        healthBuffer = health;
    }

    private void Update()
    {
        //if (isDead)
        //{
        //    return;
        //}

        if (health < maxHealth)
        {
            if (regenerationDelayTrigger)
            {
                regenerationTimer = -regenerationTriggerDelay;
                regenerationDelayTrigger = false;
            }

            RegenerateHealth();
        }

        if (healthCpy != health &&
            damageOverlay)
        {
            AdaptDamageOverlayAlpha();
            healthCpy = health;
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        Zombie zombie = _collider.GetComponent<Zombie>();

        if (zombie && !zombie.IsAttacking)
        {
            zombie.TriggerAttack();
        }
    }

    private void OnValidate()
    {
        if (isDeadCpy != isDead)
        {
            health =
                (isDead)
                    ? 0
                    : healthBuffer;

            isDeadCpy = isDead;
        }
        else if (healthBuffer != health)
        {
            healthBuffer = health;
        }

        if (maxHealth < health)
        {
            // When game is running
            // => Clamp health normally
            if (Application.isPlaying)
            {
                health = maxHealth;
            }
            // When game is not running
            // => Adapt maxHealth to health
            else
            {
                maxHealth = health;
            }
        }
    }


    private void RegenerateHealth()
    {
        regenerationTimer += Time.deltaTime;

        if (regenerationTimer >= 1f / regenerationsPerSec)
        {
            health += healthPerStep;
            regenerationTimer = 0f;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthBuffer = health;
    }

    private void AdaptDamageOverlayAlpha()
    {
        Color overlayColour = damageOverlay.color;
        overlayColour.a = (1f - (float)(health) / maxHealth);
        damageOverlay.color = overlayColour;
    }


    public void TakeDamage(int _damage)
    {
        if (isDead)
        {
            return;
        }

        health -= _damage;
        regenerationDelayTrigger = true;

        if (health <= 0)
        {
            isDead = true;
        }

        healthBuffer = health;
    }
}
