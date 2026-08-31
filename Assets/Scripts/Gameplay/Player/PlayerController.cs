using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float deceleration = 5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool invertHAxis = false;

    [Header("Collision")]
    [SerializeField] private float sphereCastRadius = 0.5f;
    [SerializeField] private float sphereCastDistance = 0.25f;
    [SerializeField] private LayerMask notJumpableLayer;

    private Mouse mouse = null;
    private Rigidbody rb = null;
    private Ray ray = new Ray();
    private Vector3 movement = Vector3.zero;
    private bool jumpTrigger = false;
    private bool isAirborne = false;
    private bool canJump = false;

    private void Awake()
    {
        mouse = Mouse.current;
        rb = GetComponent<Rigidbody>();

        ray.direction = Vector3.down;
    }

    private void FixedUpdate()
    {
        CheckGrounding();

        if (canJump)
        {
            Jump();
            canJump = false;
        }

        Move();
    }

    private void Update()
    {
        Rotate();
    }


    private void Rotate()
    {
        if (mouse == null)
        {
            Debug.LogWarning("Cannot rotate: 'mouse' is null.");
            return;
        }

        Vector2 rotationAxes = mouse.delta.ReadValue();
        float horizontal = rotationAxes.x * ((invertHAxis) ? -1f : 1f);

        rotationAxes.x = 0f;
        rotationAxes.y = horizontal;

        transform.Rotate(Time.deltaTime * rotationSpeed * rotationAxes);
    }

    private void Move()
    {
        // When not moving
        if (Mathf.Approximately(movement.x, 0f) &&
            Mathf.Approximately(movement.z, 0f))
        {
            // Apply drag horizontally only
            Vector3 linearVel = rb.linearVelocity;
            rb.AddForce(
                new Vector3(
                    -(linearVel.x * deceleration),
                    0f,
                    -(linearVel.z * deceleration)
                )
            );

            return;
        }

        // Avoid moving faster diagonally
        movement = Vector3.ClampMagnitude(movement, 1f);

        // Convert move direction from local space to
        // global space for accurate Rigidbody movement
        Vector3 movementVelocity =
            transform.TransformDirection(movement)
            * moveSpeed;
        Vector3 velocity = rb.linearVelocity;
        velocity.x = movementVelocity.x;
        velocity.z = movementVelocity.z;

        rb.linearVelocity = velocity;
    }

    private void CheckGrounding()
    {
        ray.origin = transform.position;

        // When on ground
        if (Physics.SphereCast(
                ray,
                sphereCastRadius,
                out RaycastHit info,
                sphereCastDistance,
                ~notJumpableLayer
            ))
        {
            isAirborne = false;
        }
        else
        {
            // When in the air
            if (info.collider == null ||
                info.distance == 0f)
            {
                isAirborne = true;
            }
            // When on a non-jumpable surface
            else if (info.transform.gameObject.layer == notJumpableLayer)
            {
                isAirborne = false;
            }
        }

        canJump = (jumpTrigger && !isAirborne);
    }

    private void Jump()
    {
        // When jumping from a jumpable surface
        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;
    }

    private void OnMove_Template(in InputAction.CallbackContext _context, ref float _axis)
    {
        if (_context.phase == InputActionPhase.Canceled)
        {
            _axis = 0f;
            return;
        }

        _axis = _context.ReadValue<float>();
    }


    public void OnMoveX(InputAction.CallbackContext _context)
    {
        OnMove_Template(_context, ref movement.x);
    }
    public void OnMoveZ(InputAction.CallbackContext _context)
    {
        OnMove_Template(_context, ref movement.z);
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (!rb)
        {
            Debug.LogWarning("Cannot jump: 'rb' is null.");
            return;
        }

        jumpTrigger = (_context.phase == InputActionPhase.Performed);
    }
}
