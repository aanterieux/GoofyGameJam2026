using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    [Header("- Health -")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float regenerationDelay = 2f;
    [SerializeField] private float regenerationDuration = 5f;

    [Header("- Attack -")]
    [SerializeField] private float rangedAttackReach = 15f;
    [SerializeField] private float meleeAttackReach = 0.75f;
    [SerializeField] private float meleeAttackCooldown = 0.5f;
    [SerializeField] private int meleeAttackDamage = 2;

    [Header("- Misc -")]
    [SerializeField] private float pickupReach = 5f;
    [SerializeField] private float throwForce = 10f;

    public float DistanceAttackReach
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
}
