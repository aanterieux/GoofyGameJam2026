using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionManager : MonoBehaviour
{
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


    private void PickupItem(Throwable _pickableItem)
    {

    }

    private void LetGoOfItem(Throwable _pickableItem)
    {

    }

    private void ConsumeItem(Consumable _consumableItem)
    {

    }

    private void ThrowItem(Throwable _throwableItem, in float _throwForce)
    {

    }


    // Primary action:
    //   - Throw fists
    //   - Shoot
    //   - Throw picked up object
    public void OnPrimaryAction(InputAction.CallbackContext _context)
    {
        Item heldItem = inventory.CurrentItem;

        // When not holding anything (bare hands)
        // => Throw fists
        if (heldItem == null)
        {
            if (_context.started)
            {
                TriggerMeleeAttack();
            }

            return;
        }

        // When holding a gun
        // => Shoot
        if (heldItem is Gun)
        {
            if (_context.performed)
            {
                (heldItem as Gun).StartShooting();
            }
            
            return;
        }

        // When holding a consumable object
        // => Consume it
        if (heldItem is Consumable)
        {
            if (_context.started)
            {
                ConsumeItem(heldItem as Consumable);
            }

            return;
        }

        // When holding any other object
        // => Throw it
        if (heldItem is Throwable)
        {
            if (_context.started)
            {
                ThrowItem(heldItem as Throwable, statManager.ThrowForce);
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

    }

    public void OnInteract(InputAction.CallbackContext _context)
    {

    }

    public void OnSwitchWeapon(InputAction.CallbackContext _context)
    {

    }
}
