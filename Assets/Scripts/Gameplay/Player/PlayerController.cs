using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("- Movement -")]
    [SerializeField] private float baseMoveSpeed = 4f;
    [SerializeField] private float runSpeedMultiplier = 1.75f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private bool toggleToRun = true;

    [Header("- Rotation -")]
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private bool invertHAxis = false;

    [Header("- Ground detection -")]
    [SerializeField] private float maxSlopeAngle = 30f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private LayerMask notJumpableLayer = 3;

    private Rigidbody rb = null;
    private CapsuleCollider capsule = null;
    private Ray groundRay = new Ray();
    private Vector3 movement = Vector3.zero;
    private bool jumpTrigger = false;
    private bool isAirborne = false;
    private bool isRunning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        groundRay.direction = Vector3.down;
    }

    private void FixedUpdate()
    {
        CheckGrounding();

        if (jumpTrigger && !isAirborne)
        {
            Jump();
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

        Vector2 rotationAxis = InputManager.MouseDelta;
        float horizontal = rotationAxis.x * ((invertHAxis) ? -1f : 1f);

        rotationAxis.x = 0f;
        rotationAxis.y = horizontal;

        transform.Rotate(Time.deltaTime * rotationSpeed * rotationAxis);
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

        //// When moving
        //// Avoid moving faster diagonally
        //movement = Vector3.ClampMagnitude(movement, 1f);

        float moveSpeed =
            baseMoveSpeed *
                ((isRunning)
                    ? runSpeedMultiplier
                    : 1f);
        // Convert move direction from local space to
        // global space for accurate Rigidbody movement
        Vector3 movementVelocity =
            moveSpeed * transform.TransformDirection(movement);
        Vector3 velocity = rb.linearVelocity;
        velocity.x = movementVelocity.x;
        velocity.z = movementVelocity.z;

        rb.linearVelocity = velocity;
    }

    private void CheckGrounding()
    {
        Vector3 center = transform.TransformPoint(capsule.center);
        float halfHeight = capsule.height * 0.5f;
        float bottomOffset = Mathf.Max(halfHeight - capsule.radius, 0f);
        Vector3 bottom = center + Vector3.down * bottomOffset;

        bool isGrounded = Physics.SphereCast(
            bottom + Vector3.up * 0.05f,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            ~notJumpableLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!isGrounded)
        {
            isAirborne = true;
            return;
        }

        isAirborne = IsTooSteep(hit.normal, maxSlopeAngle);
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;
    }

    private bool IsTooSteep(in Vector3 _groundNormal, in float _referenceAngle)
    {
        float groundDot = Vector3.Dot(_groundNormal, Vector3.up);
        float minGroundDot = Mathf.Cos(_referenceAngle * Mathf.Deg2Rad);

        return (groundDot < minGroundDot);
    }


    private void OnMove_Template(in InputAction.CallbackContext _context, ref float _axis)
    {
        if (_context.canceled)
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

        jumpTrigger = (_context.performed);
    }

    public void OnRun(InputAction.CallbackContext _context)
    {
        if (toggleToRun)
        {
            if (_context.started)
            {
                isRunning = !isRunning;
            }

            return;
        }

        isRunning = (_context.performed);
    }
}
