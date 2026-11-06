
using System.Collections;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Bridge Movement")]
    [SerializeField] private Transform openPoint;
    [SerializeField] private float openingDuration = 2f;

    [Header("Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private ParticleSystem activationParticles;

    private Vector3 closedPosition;
    private Quaternion closedRotation;

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private bool isActivated;

    public bool IsActivated => isActivated;

    private void Awake()
    {
        // Guarda la posición inicial del puente.
        closedPosition = transform.position;
        closedRotation = transform.rotation;

        if (openPoint != null)
        {
            // Guarda el destino antes de comenzar el movimiento.
            targetPosition = openPoint.position;
            targetRotation = openPoint.rotation;
        }
    }

    public void Activate()
    {
        if (isActivated)
            return;

        if (openPoint == null)
        {
            Debug.LogWarning(
                "BridgeController: No se asignó Open Point.",
                this
            );
            return;
        }

        isActivated = true;

        if (audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        if (activationParticles != null)
            activationParticles.Play();

        StartCoroutine(OpenBridge());
    }

    private IEnumerator OpenBridge()
    {
        float elapsed = 0f;

        while (elapsed < openingDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / openingDuration
            );

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            transform.position = Vector3.Lerp(
                closedPosition,
                targetPosition,
                smoothProgress
            );

            transform.rotation = Quaternion.Slerp(
                closedRotation,
                targetRotation,
                smoothProgress
            );

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        Debug.Log(
            "Puente activado correctamente: " + gameObject.name
        );
    }
}

