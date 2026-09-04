using UnityEngine;

public class Consumable : Holdable
{
    private Collider selfCollider = null;

    private void Awake()
    {
        if (!selfCollider)
        {
            selfCollider = GetComponent<Collider>();
        }

        base.SetCurrentHitboxValuesAsDefault(selfCollider);
        base.AdjustColliderHitbox(selfCollider);
    }

    private void Update()
    {
        if (hitTransform_)
        {
            Destroy(gameObject);
        }
    }

    private new void OnValidate()
    {
        base.OnValidate();
    }
}
