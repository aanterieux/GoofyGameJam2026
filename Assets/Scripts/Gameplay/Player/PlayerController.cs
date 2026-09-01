using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("- Movement -")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float deceleration = 5f;

    [Header("- Rotation -")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private bool invertHAxis = false;

    [Header("- Collisions -")]
    [Header("- Ground")]
    [SerializeField] private float maxSlopeAngle = 30f;
    [SerializeField] private float groundSphereRadius = 0.5f;
    [SerializeField] private float groundRayMaxDistance = 0.25f;
    [SerializeField] private LayerMask notJumpableLayer;

    [Header("- Stairs")]
    [SerializeField] [Tooltip(
        "Represents the X and Z components respectively.\n" +
        "Y component will be equal to half of stepSize.")]    
     private Vector2 stairsBoxHalfExtents = Vector2.one;
    [SerializeField] [Range(0f, 0.75f)]
     private float stepSize = 0.3f;
    [SerializeField] private float stairsRayMaxDistance = 0.75f;
    [SerializeField] private LayerMask notClimbableLayer;

    private Rigidbody rb = null;
    private Ray groundRay = new Ray();
    private Ray stairsRay = new Ray();
    private Vector3 movement = Vector3.zero;
    private bool jumpTrigger = false;
    private bool isAirborne = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        groundRay.direction = Vector3.down;
    }

    private void FixedUpdate()
    {
        CheckGrounding();
        CheckStairs();
        
        if (jumpTrigger && !isAirborne)
        {
            Jump();
            jumpTrigger = false;
        }

        Move();
    }

    private void Update()
    {
        Rotate();
    }


    private void Rotate()
    {
        if (!InputManager.MouseConnected)
        {
            Debug.LogWarning("Cannot rotate: 'mouse' is null.");
            return;
        }

        Vector2 rotationAxes = InputManager.MouseDelta;
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
        groundRay.origin = transform.position;

        // When in the air or
        // on a non-jumpable surface
        if (!Physics.SphereCast(
                groundRay,
                groundSphereRadius,
                out RaycastHit info,
                groundRayMaxDistance,
                ~notJumpableLayer
            ))
        {
            isAirborne = true;
            return;
        }

        // When on ground
        float groundDot = Vector3.Dot(info.normal, Vector3.up);
        float minGroundDot = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);

        isAirborne = (groundDot < minGroundDot);
    }

    private void CheckStairs()
    {
        Vector3 feetPos =
            transform.position
            - 0.5f * Vector3.up;

        stairsRay.origin =
            feetPos
            + stepSize * Vector3.up;
        stairsRay.direction = transform.forward;
        stairsBoxHalfExtents.y = 0.5f * stepSize;

        if (Physics.BoxCast(
                stairsRay.origin,
                stairsBoxHalfExtents,
                stairsRay.direction,
                out RaycastHit info,
                Quaternion.identity,
                stairsRayMaxDistance,
                ~notJumpableLayer
            ))
        {
            Debug.Log(info.transform.gameObject.name);
        }
    }

    private void Jump()
    {
        // When jumping from a jumpable surface
        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;

        jumpTrigger = false;
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

        if (_context.phase == InputActionPhase.Performed)
        {
            jumpTrigger = true;
        }
    }
}
