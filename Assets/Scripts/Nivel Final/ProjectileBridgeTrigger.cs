using UnityEngine;

using UnityEngine;

public class ProjectileBridgeTrigger : MonoBehaviour, IProjectileHittable
{
    [Header("Bridge")]
    [SerializeField] private BridgeController bridge;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private bool activated;

    public void OnProjectileHit(GameObject projectile)
    {
        if (activated)
            return;

        if (bridge == null)
        {
            Debug.LogWarning(
                "ProjectileBridgeTrigger: No se asignó el puente.",
                this
            );

            return;
        }

        activated = true;

        bridge.Activate();

        // Efectos visuales.
        if (hitParticles != null)
            hitParticles.Play();

        // Efecto sonoro.
        if (audioSource != null && hitSound != null)
            audioSource.PlayOneShot(hitSound);

        Debug.Log(
            "Mecanismo activado mediante proyectil: " + gameObject.name
        );
    }
}

