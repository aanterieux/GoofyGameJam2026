using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    [Header("- Health -")]
    [SerializeField] [Min(0)]
     private int health = 100;
    [SerializeField] [Min(1)]
     private int maxHealth = 100;
    [SerializeField] [Min(0f)]
     private float regenerationDelay = 2f;
    [SerializeField] [Min(0f)]
     private float regenerationDuration = 5f;

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
    [SerializeField] [Min(0f)]
     private float pickupReach = 5f;
    [SerializeField] [Min(0f)]
     private float throwForce = 10f;

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
        if (health > maxHealth) // <=> (maxHealth < health)
        {
            health = maxHealth;
        }
    }
}
