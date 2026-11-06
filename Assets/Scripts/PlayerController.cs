using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fast, precise Rigidbody-based platformer controller inspired by Super Meat Boy:
/// instant acceleration, strong air control, a short burst dash, and snappy jumps.
/// Requires a Rigidbody and a CapsuleCollider on the same GameObject.
/// Uses Unity's new Input System (WASD/Arrows to move, Space to jump, Left Shift to dash).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(AudioSource))] // 馃敟 Garantiza que el GameObject tenga un AudioSource
public class PlayerController : MonoBehaviour
{
    
    public bool IsGrounded => isGrounded;
    private const float GroundCheckRadius = 0.1f;
    private const float GroundCheckExtraDistance = 0.15f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float groundAcceleration = 60f;
    [SerializeField] private float airAcceleration = 40f;
    [SerializeField] private float groundDeceleration = 70f;
    private float horizontal = 0f;
    private float vertical = 0f;
    public float _horizontal => horizontal;
    public float _vertical  => vertical;
    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float extraGravityMultiplier = 2.5f;
    [SerializeField] private float coyoteTime = 0.12f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask = ~0;

    [Header("Step Climbing")]
    [Tooltip("Altura máxima de escalón que el jugador puede subir automáticamente al caminar contra él.")]
    [SerializeField] private float maxStepHeight = 0.65f;
    [Tooltip("Distancia hacia adelante usada para detectar un escalón bajo frente al jugador.")]
    [SerializeField] private float stepCheckDistance = 0.5f;
    [Tooltip("Velocidad vertical aplicada para subir un escalón detectado.")]
    [SerializeField] private float stepClimbSpeed = 6f;

    [Header("Wall Jump")]
    [Tooltip("Fuerza vertical aplicada al saltar desde una pared.")]
    [SerializeField] private float wallJumpUpForce = 8f;
    [Tooltip("Fuerza horizontal con la que el jugador se aleja de la pared al saltar desde ella.")]
    [SerializeField] private float wallJumpPushForce = 7f;
    [Tooltip("Tiempo máximo, tras dejar de tocar una pared, en el que todavía se puede saltar desde ella.")]
    [SerializeField] private float wallJumpBufferSeconds = 0.15f;
    [Tooltip("Componente vertical máxima (valor absoluto) de la normal de una superficie para considerarla una pared en vez de un piso o techo.")]
    [SerializeField] private float wallNormalMaxVerticalComponent = 0.35f;

    [Header("Camera")]
    [Tooltip("Movement input is interpreted relative to this camera's facing direction. Falls back to Camera.main if left empty.")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeedDegrees = 720f;

    [Header("Audio Effects 馃敟")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip wallJumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip walkSound;
    [Tooltip("Tiempo en segundos entre cada sonido de pisada al caminar.")]
    [SerializeField] private float walkStepInterval = 0.35f;

    private AudioSource audioSource; // 馃敟 Referencia al componente de audio

    private float walkStepTimer; // 馃敟 Temporizador para las pisadas recurrentes
    private Rigidbody body;
    private CapsuleCollider capsule;


    private float speed;
    public float _speed=> speed;
    private Vector2 moveInput;
    private bool jumpRequested;
    private bool dashRequested;

    private bool isGrounded;
    private float timeSinceGrounded;

    private Vector3 lastWallNormal;
    private float timeSinceWallContact = float.MaxValue;

    private bool isDashing;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;
    private Vector3 dashDirection;

    public bool canMove = true;
    private Transform PositionTeleport;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>(); // 馃敟 Inicializa el AudioSource
        // Configuraci贸n 贸ptima por defecto para efectos de sonido
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 1f para sonido 3D, cambialo a 0f si es 2D estricto
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        ReadInput();
        if (!canMove) return;

    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            body.linearVelocity = Vector3.zero;
            return;
        }
        UpdateGroundedState();
        TickWallContactTimer();
        TickDashTimers();

        if (dashRequested && CanStartDash())
        {
            StartDash();
        }
        dashRequested = false;

        if (isDashing)
        {
            ApplyDashMovement();
        }
        else
        {
            ApplyPlanarMovement();
            ApplyJump();
            ApplyFallGravity();
            HandleWalkAudio(); // 馃敟 Controla el sonido de las pisadas al caminar
        }
    }

    /// <summary>Reads raw input from the new Input System keyboard state.</summary>
    private void ReadInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        horizontal = 0f;
        vertical = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;

        moveInput = new Vector2(horizontal, vertical);
        speed=moveInput.magnitude*moveSpeed;
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
        if (keyboard.leftShiftKey.wasPressedThisFrame)
        {
            dashRequested = true;
        }
    }

    /// <summary>Casts a sphere below the capsule to determine grounded state and refresh coyote time.</summary>
private void UpdateGroundedState()
{
    Bounds bounds = capsule.bounds;

    float radius = Mathf.Min(
        bounds.extents.x,
        bounds.extents.z
    ) * 0.8f;

    Vector3 origin = new Vector3(
        bounds.center.x,
        bounds.min.y + radius + 0.02f,
        bounds.center.z
    );

    float castDistance = GroundCheckExtraDistance + 0.05f;

    bool wasGrounded = isGrounded;

    isGrounded = Physics.SphereCast(
        origin,
        radius,
        Vector3.down,
        out RaycastHit hit,
        castDistance,
        groundLayerMask,
        QueryTriggerInteraction.Ignore
    );

    if (isGrounded && body.linearVelocity.y > 0.1f)
    {
        isGrounded = false;
    }

    if (isGrounded)
    {
        timeSinceGrounded = 0f;
    }
    else
    {
        timeSinceGrounded += Time.fixedDeltaTime;
    }
}

    private void TickDashTimers()
    {
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining -= Time.fixedDeltaTime;
        }

        if (isDashing)
        {
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
                dashCooldownRemaining = dashCooldown;
            }
        }
    }

    /// <summary>Advances the buffer timer used to allow a jump shortly after leaving a wall.</summary>
    private void TickWallContactTimer()
    {
        timeSinceWallContact += Time.fixedDeltaTime;
    }

    private bool CanStartDash()
    {
        return !isDashing && dashCooldownRemaining <= 0f && moveInput.sqrMagnitude > 0.01f;
    }

    /// <summary>Begins a short, high-speed dash in the current camera-relative move direction.</summary>
    private void StartDash()
    {
        Vector3 direction = GetCameraRelativeMoveDirection();
        dashDirection = direction.sqrMagnitude > 0f ? direction : transform.forward;
        isDashing = true;
        dashTimeRemaining = dashDuration;
        PlaySound(dashSound, true); // 馃敟 Sonido de Dash
    }

    private void ApplyDashMovement()
    {
        Vector3 velocity = dashDirection * dashSpeed;
        velocity.y = body.linearVelocity.y;
        body.linearVelocity = velocity;
    }

    /// <summary>
    /// Converts raw WASD input into a world-space direction relative to the camera's
    /// horizontal facing, so "forward" always means "away from the camera".
    /// </summary>
    private Vector3 GetCameraRelativeMoveDirection()
    {
        Vector3 forward;
        Vector3 right;

        if (cameraTransform != null)
        {
            forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        }
        else
        {
            forward = Vector3.forward;
            right = Vector3.right;
        }

        Vector3 direction = right * moveInput.x + forward * moveInput.y;
        return direction.sqrMagnitude > 1f ? direction.normalized : direction;
    }

    /// <summary>Accelerates the player horizontally toward the desired input direction with separate ground/air rates.</summary>
    private void ApplyPlanarMovement()
    {
        Vector3 desiredDirection = GetCameraRelativeMoveDirection();
        Vector3 targetVelocity = desiredDirection * moveSpeed;

        Vector3 currentVelocity = body.linearVelocity;
        Vector3 currentPlanarVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        float accelerationRate = isGrounded
            ? (desiredDirection.sqrMagnitude > 0f ? groundAcceleration : groundDeceleration)
            : airAcceleration;

        Vector3 newPlanarVelocity = Vector3.MoveTowards(currentPlanarVelocity, targetVelocity, accelerationRate * Time.fixedDeltaTime);
        body.linearVelocity = new Vector3(newPlanarVelocity.x, currentVelocity.y, newPlanarVelocity.z);

        TryStepUp(desiredDirection);

        RotateTowardsMoveDirection(desiredDirection);
    }
    /// <summary>Maneja la repetici贸n r铆tmica del sonido de caminar cuando el personaje se desplaza en el suelo.</summary>
    private void HandleWalkAudio()
    {
        if (isGrounded && moveInput.sqrMagnitude > 0.01f && body.linearVelocity.magnitude > 0.1f)
        {
            walkStepTimer -= Time.fixedDeltaTime;
            if (walkStepTimer <= 0f)
            {
                PlaySound(walkSound, false); // 馃敟 Sonido de pisada
                walkStepTimer = walkStepInterval;
            }
        }
        else
        {
            walkStepTimer = 0f;
        }
    }
    /// <summary>
    /// Rigidbody characters have no built-in step offset (unlike CharacterController), so walking
    /// straight into a low step/stair edge would otherwise just stop the player dead. This detects
    /// a low obstacle right in front of the feet with clear space just above it, and gives a small
    /// upward velocity nudge so the player smoothly climbs onto it instead of getting stuck.
    /// </summary>
    private void TryStepUp(Vector3 moveDirection)
    {
        if (!isGrounded)
        {
            return;
        }

        Vector3 horizontalDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
        if (horizontalDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }
        horizontalDirection.Normalize();

        Vector3 feet = transform.position - new Vector3(0f, capsule.height * 0.5f, 0f);
        Vector3 lowOrigin = feet + Vector3.up * 0.1f;
        bool blockedLow = Physics.Raycast(lowOrigin, horizontalDirection, stepCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
        if (!blockedLow)
        {
            return;
        }

        Vector3 highOrigin = feet + Vector3.up * maxStepHeight;
        bool blockedHigh = Physics.Raycast(highOrigin, horizontalDirection, stepCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore);
        if (blockedHigh)
        {
            return;
        }

        Vector3 velocity = body.linearVelocity;
        velocity.y = Mathf.Max(velocity.y, stepClimbSpeed);
        body.linearVelocity = velocity;
    }

    /// <summary>Smoothly turns the character model to face its current movement direction.</summary>
    private void RotateTowardsMoveDirection(Vector3 desiredDirection)
    {
        if (desiredDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.RotateTowards(body.rotation, targetRotation, rotationSpeedDegrees * Time.fixedDeltaTime);
        body.MoveRotation(nextRotation);
    }

    /// <summary>Applies a jump when requested: an instant ground jump, or a wall jump when airborne near a wall.</summary>
    private void ApplyJump()
    {
        if (!jumpRequested)
        {
            return;
        }
        jumpRequested = false;

        bool canGroundJump = isGrounded || timeSinceGrounded <= coyoteTime;
        if (canGroundJump)
        {
            
            PerformGroundJump();
            return;
        }

        bool canWallJump = timeSinceWallContact <= wallJumpBufferSeconds;
        if (canWallJump)
        {
            PerformWallJump();
        }
    }

    /// <summary>Applies an instant upward impulse for a regular ground/coyote-time jump.</summary>
    private void PerformGroundJump()
    {
        Vector3 velocity = body.linearVelocity;
        velocity.y = jumpForce;
        body.linearVelocity = velocity;
        timeSinceGrounded = coyoteTime + 1f;
        PlaySound(jumpSound, true); // 馃敟 Sonido de salto normal
    }

    /// <summary>Pushes the player up and away from the last touched wall.</summary>
    private void PerformWallJump()
    {
        Vector3 pushDirection = new Vector3(lastWallNormal.x, 0f, lastWallNormal.z).normalized;

        Vector3 velocity = body.linearVelocity;
        velocity.x = pushDirection.x * wallJumpPushForce;
        velocity.z = pushDirection.z * wallJumpPushForce;
        velocity.y = wallJumpUpForce;
        body.linearVelocity = velocity;

        timeSinceWallContact = wallJumpBufferSeconds + 1f;
        PlaySound(wallJumpSound, true); // 馃敟 Sonido de salto en la pared
    }

    /// <summary>Adds extra downward acceleration while falling for a snappier, less floaty arc.</summary>
    private void ApplyFallGravity()
    {
        if (isGrounded || body.linearVelocity.y >= 0f)
        {
            return;
        }
        body.linearVelocity += Vector3.up * Physics.gravity.y * (extraGravityMultiplier - 1f) * Time.fixedDeltaTime;
    }
    /// <summary>馃敟 M茅todo centralizado para reproducir clips de audio sin generar basura en memoria.</summary>
    private void PlaySound(AudioClip clip, bool forceInterruption)
    {
        if (clip == null || audioSource == null) return;

        if (forceInterruption)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }
private void OnCollisionEnter(Collision collision)
{
    IKillZone killZone = collision.gameObject.GetComponent<IKillZone>();

    if (killZone != null)
    {
        if (PositionTeleport == null)
        {
            Debug.LogError(
                "No hay un checkpoint guardado. Revisá si el jugador activó el checkpoint antes de morir.",
                this
            );
            return;
        }

        killZone.teleportCheckPoint(PositionTeleport);
    }

    IChangeScene changeScene = collision.gameObject.GetComponent<IChangeScene>();

    if (changeScene != null)
    {
        changeScene.ChangeScene();
    }
}

    private void OnCollisionStay(Collision collision)
    {
        DetectWallContact(collision);
    }

    /// <summary>Looks for a near-vertical contact normal (a wall) among the collision's contact points.</summary>
    private void DetectWallContact(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) <= wallNormalMaxVerticalComponent)
            {
                lastWallNormal = contact.normal;
                timeSinceWallContact = 0f;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
{
    ICheckPoint checkPoint = collision.GetComponent<ICheckPoint>();

    if (checkPoint != null)
    {
        PositionTeleport = checkPoint.newCheckPoint();

        if (PositionTeleport != null)
        {
            Debug.Log("Checkpoint guardado correctamente: " + PositionTeleport.name);
        }
        else
        {
            Debug.LogError("El checkpoint devolvió null: " + collision.name);
        }
    }
}

    
}
