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
    [Tooltip("Prefab instanciado en tiempo de ejecución al disparar.")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Punto desde el que se instancian los proyectiles. Si se deja vacío, se calcula frente al jugador para no chocar contra su propio collider.")]
    [SerializeField] private Transform launchPoint;
    [Tooltip("Cámara usada para apuntar el disparo. Si se deja vacío, usa Camera.main.")]
    [SerializeField] private Transform aimCamera;
    [SerializeField] private float projectileSpeed = 14f;
    [SerializeField] private float launchCooldownSeconds = 0.35f;
    [Tooltip("Distancia hacia adelante, desde el jugador, a la que se genera el proyectil cuando no hay launchPoint asignado.")]
    [SerializeField] private float spawnForwardOffset = 1f;
    [Tooltip("Altura, desde la base del jugador, a la que se genera el proyectil cuando no hay launchPoint asignado.")]
    [SerializeField] private float spawnHeightOffset = 1.4f;

    private float cooldownRemaining;
    private bool hasWeapon;
    private Collider ownCollider;

    private void Awake()
    {
        if (aimCamera == null && Camera.main != null)
        {
            aimCamera = Camera.main.transform;
        }
        ownCollider = GetComponent<Collider>();
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

        if (mouse.leftButton.wasPressedThisFrame && cooldownRemaining <= 0f)
        {
            Launch();
            cooldownRemaining = launchCooldownSeconds;
        }
    }

    /// <summary>
    /// Instancia un proyectil delante del jugador (o en launchPoint, si se asignó uno) apuntando
    /// hacia donde mira la cámara, y lo impulsa en esa dirección. Ignora la colisión contra el
    /// propio jugador para evitar que se autodestruya o lo empuje al aparecer.
    /// </summary>
    private void Launch()
    {
        Vector3 aimDirection = aimCamera != null ? aimCamera.forward : transform.forward;

        Vector3 spawnPosition = launchPoint != null
            ? launchPoint.position
            : transform.position + Vector3.up * spawnHeightOffset + aimDirection * spawnForwardOffset;

        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(aimDirection));

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
