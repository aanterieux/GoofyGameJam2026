using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Item
{
    private Rigidbody rb = null;
    private Transform holder = null;
    private new Collider collider = null;
    private float distanceWithHolder = 0f;
    private bool isHeld = false;

    public bool IsHeld
    {
        get => isHeld;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!collider)
        {
            collider = GetComponent<Collider>();
        }

        base.SetCurrentHitboxValuesAsDefault(collider);
        base.AdjustColliderHitbox(collider);
    }

    private void Update()
    {
        if (!isHeld)
        {
            return;
        }

        transform.position =
            holder.position
            + distanceWithHolder * holder.forward;
    }

    private new void OnValidate()
    {
        base.OnValidate();

        if (base.HitboxAdjustmentTrigger)
        {
            if (!collider)
            {
                collider = GetComponent<Collider>();
            }

            base.AdjustColliderHitbox(collider);
        }
    }


    public void NotifyHold(Transform _holder)
    {
        holder = _holder;
        distanceWithHolder = Vector3.Distance(transform.position, holder.position);
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        isHeld = true;
    }
    
    public void NotifyDrop()
    {
        holder = null;
        distanceWithHolder = 0f;

        rb.useGravity = true;
        isHeld = false;
    }

    public void NotifyThrow(float _throwForce)
    {
        isHeld = false;
        rb.useGravity = true;

        rb.AddForce(
            _throwForce * holder.forward
            + 0.5f * _throwForce * Vector3.up,
            ForceMode.VelocityChange
        );
    }
}
