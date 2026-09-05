using UnityEngine;

public class Gun : Item
{
    [SerializeField] private int shotsPerSecond = 4;
    [SerializeField] private int shotDamage = 5;
    [SerializeField] private int ammosPerRound = 10;
    [SerializeField] private int roundNb = 3;

    private Ray ray = new Ray();
    private float shootCooldown = 0f;
    private float shootTimer = 0f;
    private float shotReach = 0f;
    private bool isShooting = false;

    private void Awake()
    {
        shootCooldown = 1f / shotsPerSecond;
    }

    private void Update()
    {
        if (!isShooting)
        {
            return;
        }

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            Shoot();
            shootTimer = 0f;
        }
    }


    private void Shoot()
    {
        if (Physics.Raycast(
            ray,
            out RaycastHit info,
            shotReach
        ))
        {
            Transform target = info.transform;

            if (!target)
            {
                return;
            }

            Zombie zombie = target.GetComponent<Zombie>();

            if (!zombie)
            {
                return;
            }

            zombie.TakeDamage(shotDamage);
            Debug.Log("<color=yellow>Pew !</color>");
        }
    }


    public void SetIsAiming(bool _isAiming)
    {
        Color colour =
            (_isAiming)
                ? Color.green
                : Color.red;
        string textColour = "<color=" + colour.ToString() + ">";

        Debug.Log(textColour + "isAiming" + "</color>");
    }

    public void StartShooting(float _shotReach, Vector3 _shotOrigin, Vector3 _shotDirection)
    {
        ray.origin = _shotOrigin;
        ray.direction = _shotDirection;

        shotReach = _shotReach;
        shootTimer = shootCooldown;
        isShooting = true;
    }
    public void StopShooting()
    {
        isShooting = false;
    }
}
