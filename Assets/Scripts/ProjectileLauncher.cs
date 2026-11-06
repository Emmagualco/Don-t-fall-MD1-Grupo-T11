using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Genera proyectiles en tiempo de ejecución con el click izquierdo del
/// mouse, demostrando el uso de Instantiate con un prefab reutilizable (ver
/// Projectile). El disparo permanece deshabilitado hasta que el jugador
/// recoge el arma (ver WeaponPickup.EnableWeapon). Cada proyectil se
/// destruye a sí mismo tras impactar algo o expirar su tiempo de vida.
/// Usa el nuevo Input System, igual que PlayerController.
/// </summary>
public class ProjectileLauncher : MonoBehaviour
{
    [Header("Muzzle Flash Settings")]
    [Tooltip("Prefab del efecto visual del fogonazo.")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [Header("GameObjects Gun")]
    [SerializeField] private GameObject Gun;

    [Header("Projectile Settings")]
    [Tooltip("Prefab instanciado en tiempo de ejecución al disparar.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Punto desde el que se instancian los proyectiles. Si se deja vacío, se calcula frente al jugador para no chocar contra su propio collider.")]
    [SerializeField] private Transform launchPoint;
    [Tooltip("Cámara usada para apuntar el disparo. Si se deja vacío, usa Camera.main.")]
    [SerializeField] private Transform aimCamera;
    [SerializeField] private float projectileSpeed = 14f;
    [SerializeField] private float launchCooldownSeconds = 0.35f;
    [SerializeField] private float rotationSpeed = 15f;
    [Header("Spawn Offsets (If No Launch Point)")]
    [Tooltip("Distancia hacia adelante, desde el jugador, a la que se genera el proyectil cuando no hay launchPoint asignado.")]
    [SerializeField] private float spawnForwardOffset = 1f;
    [Tooltip("Altura, desde la base del jugador, a la que se genera el proyectil cuando no hay launchPoint asignado.")]
    [SerializeField] private float spawnHeightOffset = 1.4f;

    [Header("Audio Settings")]
    [Tooltip("El sonido que se reproducirá al disparar.")]
    [SerializeField] private AudioClip shootSound;
    [Tooltip("Volumen del disparo (entre 0.0 y 1.0).")]
    [SerializeField] private float shootVolume = 0.7f;

    private float cooldownRemaining;
    private bool hasWeapon;
    public bool HasWeapon => hasWeapon;
    private Collider ownCollider;
    private AudioSource audioSource; // Referencia al componente de audio

    private void Awake()
    {
        if (aimCamera == null && Camera.main != null)
        {
            aimCamera = Camera.main.transform;
        }
        ownCollider = GetComponent<Collider>();

        // Obtenemos el componente AudioSource adjunto al objeto
        audioSource = GetComponent<AudioSource>();

        // Si olvidaste agregarlo en el Inspector, este código lo creará automáticamente por ti
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= Time.deltaTime;
        }
        ReadLaunchInput();
    }

    /// <summary>Otorga la capacidad de disparar. Se llama desde WeaponPickup al recoger el arma.</summary>
    public void EnableWeapon()
    {
        hasWeapon = true;
    }

    /// <summary>Lee el click izquierdo del mouse (nuevo Input System) y dispara si ya se tiene el arma y el cooldown lo permite.</summary>
    private void ReadLaunchInput()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || projectilePrefab == null || !hasWeapon)
        {
            return;
        }
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Gun.SetActive(true);
            if (aimCamera != null)
            {
                // Obtenemos la dirección de la cámara pero aplanamos el eje Y para que no rote hacia el cielo/suelo
                Vector3 camForward = aimCamera.forward;
                camForward.y = 0f;

                if (camForward.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(camForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            if (mouse.leftButton.wasPressedThisFrame && cooldownRemaining <= 0f)
            {
                Launch();
                cooldownRemaining = launchCooldownSeconds;
            }
        }
        else
        {
            Gun.SetActive(false);
        }
    }

    /// <summary>
    /// Instancia un proyectil delante del jugador (o en launchPoint, si se asignó uno) apuntando
    /// hacia donde mira la cámara, lo impulsa en esa dirección y reproduce el sonido de disparo.
    /// Ignora la colisión contra el propio jugador para evitar que se autodestruya o lo empuje al aparecer.
    /// </summary>
    private void Launch()
    {
        Vector3 aimDirection = aimCamera != null ? aimCamera.forward : transform.forward;
        Vector3 spawnPosition = launchPoint != null ? launchPoint.position : transform.position + Vector3.up * spawnHeightOffset + aimDirection * spawnForwardOffset;

        // 1. Instanciar el proyectil
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(aimDirection));
        if (muzzleFlashPrefab != null && launchPoint != null)
        {
            // Instanciamos el fogonazo en la punta del cañón, alineado con su rotación
            GameObject flashInstance = Instantiate(muzzleFlashPrefab, launchPoint.position, launchPoint.rotation, launchPoint);

            // Opcional: Destruir el fogonazo después de un breve instante (ej. 0.1 o 0.2 segundos) para que no sature la jerarquía
            Destroy(flashInstance, 0.2f);
        }
        // 2. Reproducir el sonido de disparo
        if (audioSource != null && shootSound != null)
        {
            // Usamos PlayOneShot para que los sonidos puedan superponerse si disparas rápido
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        // 3. Configurar físicas del proyectil
        Collider projectileCollider = projectileInstance.GetComponent<Collider>();
        if (projectileCollider != null && ownCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, ownCollider);
        }

        Rigidbody projectileBody = projectileInstance.GetComponent<Rigidbody>();
        if (projectileBody != null)
        {
            projectileBody.linearVelocity = aimDirection * projectileSpeed;
        }
    }
}