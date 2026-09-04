using UnityEngine;

public class Consumable : Holdable
{
    private Collider consumableCollider = null;

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
            Destroy(gameObject);
        }
    }

    private new void OnValidate()
    {
        base.OnValidate();
    }
}
