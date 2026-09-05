using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    private enum ZombieState
    {
        IDLE,
        CHASE,
        ATTACK,
        DEAD
    }

    public enum ZombieEffect
    {
        NONE,

        SLOWNESS,
        BURN,
        FREEZE,

        COUNT
    }

    [Header("- Health -")]
    [SerializeField] private int health = 25;
    [SerializeField] private int maxHealth = 25;

    [Header("- Attack -")]
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private int damage = 15;

    [Header("- Misc -")]
    [SerializeField] private ZombieState state = ZombieState.CHASE;
    [SerializeField] private uint destinationUpdatesPerSecond = 16U;
    [SerializeField] private float burySpeed = 5f;
    [SerializeField] private float distanceToAttack = 0.5f;
    [SerializeField] private float distanceToChase = 0.75f;

    private const int EFFECT_COUNT = ((int)(ZombieEffect.COUNT)) - 2;

    private NavMeshAgent agent = null;
    private Transform playerTransform = null;
    private CapsuleCollider capsule = null;
    private ZombieSpawner origin = null;
    private float[] effectsStrength = new float[EFFECT_COUNT];
    private float[] effectsDuration = new float[EFFECT_COUNT];
    private float[] effectsTimer = new float[EFFECT_COUNT];
    private bool[] hasEffect = new bool[EFFECT_COUNT];
    private float attackTimer = 0f;
    private float baseMoveSpeed = 0f;
    private float destinationUpdateTimer = 0f;
    private bool isFrozenOrPowerless = false;

    public bool IsAttacking
    {
        get => (state == ZombieState.ATTACK && attackTimer > 0f);
    }


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        capsule = GetComponent<CapsuleCollider>();

        VaryStatsAndSize();

        baseMoveSpeed = agent.speed;
    }

    private void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        ManageStates();
        ManageEffects();
    }

    private void OnValidate()
    {
        if (state != ZombieState.DEAD)
        {
            if (maxHealth < health)
            {
                maxHealth = health;
            }
        }
    }


    private void VaryStatsAndSize()
    {
        if (Random.Range(0, 10001) == 0)
        {
            isFrozenOrPowerless = true;
        }

        int healthChange = Random.Range(-7, 16);
        float speedChange = Random.Range(-1.5f, 1.5f);
        float cooldownChange = Random.Range(-0.05f, 0.05f);
        int damageChange = Random.Range(-5, 6);

        health += healthChange;
        agent.speed += speedChange;
        attackCooldown += cooldownChange;
        damage += damageChange;

        float healthStrength =
            Mathf.InverseLerp(-7f, 15f, healthChange) * 2f - 1f;
        float speedStrength =
            Mathf.InverseLerp(-1.5f, 1.5f, speedChange) * 2f - 1f;
        float cooldownStrength =
            Mathf.InverseLerp(0.1f, -0.1f, cooldownChange) * 2f - 1f;
        float damageStrength =
            Mathf.InverseLerp(-5f, 5f, damageChange) * 2f - 1f;
        float combatStrength = (
                healthStrength +
                speedStrength +
                cooldownStrength +
                damageStrength
            ) / 5f;
        float statMean =
            Mathf.Lerp(0.66f, 1.25f, (combatStrength + 1f) / 2f);

        capsule.radius *= statMean;
        capsule.height *= statMean;
        capsule.transform.localScale *= statMean;
    }

    private void ManageStates()
    {
        if (!agent || !agent.isOnNavMesh)
        {
            return;
        }

        switch (state)
        {
            case ZombieState.IDLE:
                {

                }
                break;
            case ZombieState.CHASE:
                {
                    UpdateDestination();

                    if (GetDistanceToPlayer() <= distanceToAttack)
                    {
                        state = ZombieState.ATTACK;
                    }
                }
                break;
            case ZombieState.ATTACK:
                {
                    attackTimer += Time.deltaTime;

                    if (attackTimer > attackCooldown)
                    {
                        playerTransform
                            .GetComponent<PlayerStatManager>()
                            .TakeDamage(damage);

                        attackTimer = 0f;
                    }

                    if (GetDistanceToPlayer() > distanceToChase)
                    {
                        state = ZombieState.CHASE;
                    }
                }
                break;
            case ZombieState.DEAD:
                {
                    agent.enabled = false;
                    capsule.enabled = false;

                    transform.Translate(Time.deltaTime * burySpeed * Vector3.down);

                    if (transform.position.y + 1f < -0.5f)
                    {
                        origin.NotifyZombieDeath();
                        Destroy(gameObject);
                    }
                }
                break;
            default:
                {
                }
                break;
        }
    }

    private void ManageEffects()
    {
        if (state == ZombieState.DEAD)
        {
            return;
        }

        for (int i = 0; i < hasEffect.Length; ++i)
        {
            if (!hasEffect[i] || ((ZombieEffect)(i)) == ZombieEffect.NONE)
            {
                continue;
            }

            effectsTimer[i] += Time.deltaTime;

            if (effectsTimer[i] > effectsDuration[i])
            {
                hasEffect[i] = false;
                effectsTimer[i] = 0f;

                if (hasEffect[(int)(ZombieEffect.SLOWNESS)])
                {
                    agent.speed = baseMoveSpeed;
                }

                return;
            }
        }

        if (hasEffect[(int)(ZombieEffect.SLOWNESS)])
        {
            float normalisedSlowness =
                1f -
                0.01f * effectsStrength[(int)(ZombieEffect.SLOWNESS)];

            agent.speed = baseMoveSpeed * normalisedSlowness;
        }
    }

    private void UpdateDestination()
    {
        destinationUpdateTimer += Time.deltaTime;

        if (destinationUpdateTimer >= 1f / destinationUpdatesPerSecond)
        {
            agent.SetDestination(playerTransform.position);
            destinationUpdateTimer = 0f;
        }
    }

    public void TriggerAttack()
    {
        state = ZombieState.ATTACK;
    }

    private void Die()
    {
        state = ZombieState.DEAD;
    }

    private float GetDistanceToPlayer()
    {
        return
            Vector3.Distance(
                transform.position,
                playerTransform.position
            );
    }


    public void LinkToSpawner(ZombieSpawner _spawner)
    {
        origin = _spawner;
    }

    public void TakeDamage(int _damage)
    {
        if (state == ZombieState.DEAD)
        {
            return;
        }

        health -= _damage;

        if (health <= 0)
        {
            Die();
        }
    }

    public void GiveEffect(ZombieEffect _effect, float _strengthInPercentage, float _duration)
    {
        if (_effect == ZombieEffect.NONE ||
            _effect == ZombieEffect.COUNT)
        {
            return;
        }

        int effectIndex = (int)(_effect);

        if (hasEffect[effectIndex])
        {
            return;
        }

        hasEffect[effectIndex] = true;
        effectsStrength[effectIndex] = _strengthInPercentage;
        effectsDuration[effectIndex] = _duration;
        effectsTimer[effectIndex] = 0f;
    }
}
