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

    [Header("- Health -")]
    [SerializeField] private int health = 25;
    [SerializeField] private int maxHealth = 25;

    [Header("- Attack -")]
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private int damage = 15;

    [Header("- Misc -")]
    [SerializeField] private ZombieState state = ZombieState.CHASE;
    [SerializeField] private uint destinationUpdateNbPerSecond = 16U;
    [SerializeField] private float burySpeed = 5f;

    private NavMeshAgent agent = null;
    private Transform playerTransform = null;
    private float attackTimer = 0f;
    private float destinationUpdateTimer = 0f;

    public bool IsAttacking
    {
        get => (state == ZombieState.ATTACK);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        switch (state)
        {
            case ZombieState.IDLE:
                {

                }
                break;
            case ZombieState.CHASE:
                {
                    if (agent && agent.isOnNavMesh)
                    {
                        UpdateDestination();
                    }
                }
                break;
            case ZombieState.ATTACK:
                {

                }
                break;
            case ZombieState.DEAD:
                {
                    agent.enabled = false;

                    if (transform.position.y + 1f > -0.5f)
                    {
                        transform.Translate(Time.deltaTime * burySpeed * Vector3.down);
                    }
                    else
                    {
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

    private void OnValidate()
    {
        if (state != ZombieState.DEAD)
        {
            if (health > maxHealth) // <=> (maxHealth < health)
            {
                health = maxHealth;
            }
        }
    }


    private void UpdateDestination()
    {
        destinationUpdateTimer += Time.deltaTime;

        if (destinationUpdateTimer >= 1f / destinationUpdateNbPerSecond)
        {
            agent.SetDestination(playerTransform.position);
            destinationUpdateTimer = 0f;
        }
    }

    private void Die()
    {
        state = ZombieState.DEAD;
    }


    public void TriggerAttack()
    {

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
}
