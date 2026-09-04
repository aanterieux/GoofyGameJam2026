using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1000)]
     private int maxZombieCount = 50;
    [SerializeField] private bool dontDestroyOnLoad = true;

    private int zombieCount = 0;

    public int ZombieCount
    {
        get => zombieCount;
    }
    public bool IsSpawnAllowed
    {
        get => (zombieCount < maxZombieCount);
    }

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void IncrementZombieCount()
    {
        zombieCount++;
    }
    public void DecrementZombieCount()
    {
        if (zombieCount == 0)
        {
            return;
        }

        zombieCount--;
    }
}
