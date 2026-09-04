using UnityEngine;

public class Consumable : Holdable
{
    private enum ConsumableType
    {
        CHEESE_SAUCE,
        BBQ_SAUCE,
        MEXICAN_SAUCE
    }

    private Collider consumableCollider = null;
    private ConsumableType type = ConsumableType.CHEESE_SAUCE;

    private void Awake()
    {
        if (!consumableCollider)
        {
            consumableCollider = GetComponent<Collider>();
        }

        base.SetCurrentHitboxValuesAsDefault(consumableCollider);
        base.AdjustColliderHitbox(consumableCollider);
    }

    private new void Update()
    {
        base.Update();

        if (isThrown_ && hitTransform_)
        {
            isThrown_ = false;

            Zombie zombie = hitTransform_.GetComponent<Zombie>();

            if (zombie)
            {
                switch (type)
                {
                    case ConsumableType.CHEESE_SAUCE:
                        {
                            zombie.TakeDamage(4);
                            zombie.GiveEffect(Zombie.ZombieEffect.SLOWNESS, 30f, 5f);
                        }
                        break;
                    case ConsumableType.BBQ_SAUCE:
                        {

                        }
                        break;
                    case ConsumableType.MEXICAN_SAUCE:
                        {

                        }
                        break;
                    default:
                        {
                        }
                        break;
                }
            }

            Destroy(gameObject);
        }
    }

    private new void OnValidate()
    {
        base.OnValidate();
    }
}
