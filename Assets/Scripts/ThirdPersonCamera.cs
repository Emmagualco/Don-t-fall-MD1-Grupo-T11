using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Orbiting third-person camera that smoothly follows a target and lets the player
/// look around it with the mouse. Attach to the scene's camera and assign the
/// player's Transform as the target.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    private const float LookSensitivityScale = 0.15f;

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Orbit")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float initialPitch = 20f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private bool lockCursor = true;

    [Header("Collision")]
    [Tooltip("Capas contra las que la cámara detecta obstáculos para no atravesar paredes.")]
    [SerializeField] private LayerMask obstructionLayerMask = ~0;
    [Tooltip("Radio del chequeo esférico usado para detectar obstáculos entre el pivote y la cámara.")]
    [SerializeField] private float collisionRadius = 0.25f;
    [Tooltip("Margen que se deja entre la cámara y la pared detectada, para no quedar pegada a ella.")]
    [SerializeField] private float collisionPadding = 0.15f;

    private float yaw;
    private float pitch;
    private Vector3 currentVelocity;
    private Vector3 smoothedPivot;

    private void Awake()
    {
        pitch = initialPitch;
        if (target != null)
        {
            yaw = target.eulerAngles.y;
            smoothedPivot = target.position + targetOffset;
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        ReadLookInput();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        FollowTarget();
        OrbitAroundTarget();
    }

    /// <summary>Reads mouse delta from the new Input System and accumulates yaw/pitch.</summary>
    private void ReadLookInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        Vector2 lookDelta = mouse.delta.ReadValue() * LookSensitivityScale * mouseSensitivity;
        yaw += lookDelta.x;
        pitch -= lookDelta.y;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    /// <summary>Smoothly moves the camera's pivot point toward the target's position.</summary>
    private void FollowTarget()
    {
        Vector3 desiredPivot = target.position + targetOffset;
        smoothedPivot = Vector3.SmoothDamp(smoothedPivot, desiredPivot, ref currentVelocity, positionSmoothTime);
    }

    /// <summary>
    /// Positions the camera on a sphere around the pivot based on current yaw/pitch and looks at it.
    /// Pulls the camera closer along that same direction if level geometry is in the way, so it never
    /// clips through walls or corners.
    /// </summary>
    private void OrbitAroundTarget()
    {
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 orbitDirection = orbitRotation * new Vector3(0f, 0f, -1f);

        float clampedDistance = distance;
        if (Physics.SphereCast(smoothedPivot, collisionRadius, orbitDirection, out RaycastHit hit, distance, obstructionLayerMask, QueryTriggerInteraction.Ignore))
        {
            clampedDistance = Mathf.Max(hit.distance - collisionPadding, 0f);
        }

        transform.position = smoothedPivot + orbitDirection * clampedDistance;
        transform.rotation = orbitRotation;
    }
}
