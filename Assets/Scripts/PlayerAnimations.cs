using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ProjectileLauncher projectileLauncher;

    [Header("Animation Settings")]
    [SerializeField] private float animationDampTime = 0.1f;
    [SerializeField] private float aimTransitionSpeed = 5f;

    private float speedGun;

    private static readonly int GunHash =
        Animator.StringToHash("Gun");

    private static readonly int MoveXHash =
        Animator.StringToHash("MovX");

    private static readonly int MoveYHash =
        Animator.StringToHash("MovY");

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int MoveSpeedHash =
        Animator.StringToHash("MoveSpeed");

    private static readonly int GroundedHash =
        Animator.StringToHash("Grounded");

    private static readonly int WaveHash =
        Animator.StringToHash("Wave");

    private static readonly int PickupHash =
        Animator.StringToHash("Pickup");

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (playerController == null)
            playerController = GetComponentInParent<PlayerController>();

        if (anim == null)
            Debug.LogError(
                "PlayerAnimations: No se encontró Animator.",
                this
            );

        if (playerController == null)
            Debug.LogError(
                "PlayerAnimations: No se encontró PlayerController.",
                this
            );
    }

    private void Update()
    {
        if (anim == null || playerController == null)
            return;

        float horizontal = playerController._horizontal;
        float vertical = playerController._vertical;
        float speed = playerController._speed;

        // Mantener el apuntado y el disparo.
        float targetWeight = 0f;

        // 1. Verificamos si al menos estás apretando el clic derecho
        if (playerController.canMove && anim.GetBool(GunHash))
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                targetWeight = 1f;
            }
        }

        speedGun = Mathf.MoveTowards(
            speedGun,
            targetWeight,
            Time.deltaTime * aimTransitionSpeed
        );

        bool hasWeapon = projectileLauncher.HasWeapon;
        // BUSCAMOS LA CAPA POR SU NOMBRE EXACTO (Evita problemas con los números de índice)
        int gunLayerIndex = anim.GetLayerIndex("Gun");
        if (gunLayerIndex != -1 && hasWeapon==true)
        {
            anim.SetLayerWeight(gunLayerIndex, speedGun);
        }
        else
        {
            Debug.LogWarning("No se encontró la capa llamada 'Gun' en el Animator.");
        }
        // Movimiento y Blend Trees existentes.
        anim.SetFloat(MoveXHash, horizontal);
        anim.SetFloat(MoveYHash, vertical);
        anim.SetFloat(SpeedHash, speed);

        float movement = Mathf.Clamp01(
            new Vector2(horizontal, vertical).magnitude
        );

        if (!playerController.canMove)
            movement = 0f;

        anim.SetFloat(
            MoveSpeedHash,
            movement,
            animationDampTime,
            Time.deltaTime
        );

        // Actualizar el estado del suelo.
        anim.SetBool(
            GroundedHash,
            playerController.IsGrounded
        );
    }

    public void PlayWave()
    {
        if (anim == null)
            return;

        anim.ResetTrigger(PickupHash);
        anim.SetTrigger(WaveHash);
    }

    public void PlayPickup()
    {
        if (anim == null)
            return;

        anim.ResetTrigger(WaveHash);
        anim.SetTrigger(PickupHash);
    }
}
