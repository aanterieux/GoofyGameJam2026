using UnityEngine;

public class Gun : Item
{
    [SerializeField] private int shotsPerSecond = 4;
    [SerializeField] private int ammosPerRound = 20;
    [SerializeField] private int roundNb = 3;

    private float shootCooldown = 0f;
    private float shootTimer = 0f;
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
            // Somehow link shoot logic to Player
            // without linking Gun with Player
            shootTimer = 0f;
        }
    }

    public void StartShooting()
    {
        shootTimer = shootCooldown;
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }
}
