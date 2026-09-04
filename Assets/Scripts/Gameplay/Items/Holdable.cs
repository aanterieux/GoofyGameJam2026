using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Holdable : Item
{
    private Rigidbody rb = null;
    private Transform holder = null;
    private Collider selfCollider = null;
    private float distanceWithHolder = 0f;
    private bool isHeld = false;

    protected Transform hitTransform_ = null;

    public bool IsHeld
    {
        get => isHeld;
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!selfCollider)
        {
            selfCollider = GetComponent<Collider>();
        }

        base.SetCurrentHitboxValuesAsDefault(selfCollider);
        base.AdjustColliderHitbox(selfCollider);
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

    protected new void OnValidate()
    {
        base.OnValidate();

        if (base.HitboxAdjustmentTrigger_)
        {
            if (!selfCollider)
            {
                selfCollider = GetComponent<Collider>();
            }

            base.AdjustColliderHitbox(selfCollider);
        }
    }

    private void OnCollisionEnter(Collision _collision)
    {
        Transform collisionTransform = _collision.transform;

        if (isHeld || collisionTransform.GetComponent<Item>())
        {
            return;
        }

        hitTransform_ = collisionTransform;
    }

    private void OnCollisionExit(Collision _collision)
    {
        hitTransform_ = null;
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
