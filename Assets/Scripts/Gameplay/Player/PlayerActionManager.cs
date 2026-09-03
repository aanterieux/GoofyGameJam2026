using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private Transform camTransform = null;

    private PlayerStatManager statManager = null;
    private PlayerInventory inventory = null;
    private Ray ray = new Ray();
    private float meleeAttackTimer = 0f;
    private float meleeAttackCooldown = 0f;
    private float meleeAttackDamage = 0f;

    private void Awake()
    {
        statManager = GetComponent<PlayerStatManager>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        
    }


    private void TriggerMeleeAttack()
    {
        meleeAttackCooldown = statManager.MeleeAttackCooldown;
        meleeAttackDamage = statManager.MeleeAttackDamage;
    }


    private void PickupItem(Holdable _pickableItem)
    {
        inventory.SetCurrentItem(_pickableItem);
        _pickableItem.NotifyHold(camTransform);
    }

    private void DropItem(Holdable _pickableItem)
    {
        inventory.SetCurrentItem(null);
        _pickableItem.NotifyDrop();
    }

    private void ConsumeItem(Consumable _consumableItem)
    {

    }

    private void ThrowItem(Holdable _throwableItem, float _throwForce)
    {
        _throwableItem.NotifyThrow(_throwForce);
    }


    private void UpdateRayOriginAndDirection()
    {
        ray.origin = camTransform.position;
        ray.direction = camTransform.forward;
    }


    // Primary action:
    //   - Throw fists
    //   - Shoot
    //   - Throw picked up object
    public void OnPrimaryAction(InputAction.CallbackContext _context)
    {
        UpdateRayOriginAndDirection();

        bool nothingFound =
            !Physics.Raycast(
                ray,
                out RaycastHit info,
                statManager.PickupReach
            );
        Transform itemTransform = info.transform;

        if (nothingFound || itemTransform == null)
        {
            return;
        }

        Item item = itemTransform.GetComponent<Item>();

        // When not holding anything (bare hands)
        if (item == null)
        {
            if (_context.started)
            {
                TriggerMeleeAttack();
            }

            return;
        }

        // When holding a gun
        if (item is Gun)
        {
            if (_context.performed)
            {
                (item as Gun).StartShooting();
            }

            return;
        }

        // When holding a consumable object
        if (item is Consumable)
        {
            if (_context.started)
            {
                ConsumeItem(item as Consumable);
            }

            return;
        }

        // When holding any other object
        if (item is Holdable)
        {
            Holdable throwable = (item as Holdable);

            if (_context.started && throwable.IsHeld)
            {
                ThrowItem(throwable, statManager.ThrowForce);
            }

            return;
        }
    }

    // Secondary action:
    //   - Aim
    //   - Pick object up
    //   - Let go of picked up object
    public void OnSecondaryAction(InputAction.CallbackContext _context)
    {
        UpdateRayOriginAndDirection();

        bool nothingFound =
            !Physics.Raycast(
                ray,
                out RaycastHit info,
                statManager.PickupReach
            );
        Transform itemTransform = info.transform;

        if (nothingFound || itemTransform == null)
        {
            return;
        }

        Item item = itemTransform.GetComponent<Item>();

        inventory.SetCurrentItem(item);

        // When looking at a gun
        if (item is Gun)
        {
            (item as Gun).SetIsAiming(_context.performed);
            return;
        }

        // When looking at any other item
        if (item is Holdable)
        {
            if (_context.started)
            {
                Holdable throwable = item as Holdable;

                if (!throwable.IsHeld)
                {
                    PickupItem(throwable);
                }
                else
                {
                    DropItem(throwable);
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext _context)
    {

    }

    public void OnSwitchWeapon(InputAction.CallbackContext _context)
    {

    }
}
