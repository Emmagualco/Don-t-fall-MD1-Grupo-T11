
using UnityEngine;

public class BridgeActivator : MonoBehaviour
{
    [Header("Bridge")]
    [SerializeField] private BridgeController bridge;

    [Header("Requirements")]
    [SerializeField] private DialogueNPC requiredDialogue;
    [SerializeField] private bool requireDialogueCompleted = true;

    [Header("Platform Feedback")]
    [SerializeField] private Renderer platformRenderer;
    [SerializeField] private Color activatedColor = Color.green;

    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated || !other.CompareTag("Player"))
            return;

        // Comprueba si el diálogo fue completado.
        if (requireDialogueCompleted)
        {
            if (requiredDialogue == null)
            {
                Debug.LogWarning(
                    "BridgeActivator: No se asignó Required Dialogue.",
                    this
                );

                return;
            }

            if (!requiredDialogue.HasCompletedDialogue)
            {
                Debug.Log("Primero debes hablar con el castor.");
                return;
            }
        }

        ActivatePlatform();
    }

    private void ActivatePlatform()
    {
        if (bridge == null)
        {
            Debug.LogWarning(
                "BridgeActivator: No se asignó el puente.",
                this
            );

            return;
        }

        activated = true;

        bridge.Activate();

        // Cambia el color de la plataforma.
        if (platformRenderer != null)
            platformRenderer.material.color = activatedColor;

        Debug.Log("Plataforma activada correctamente.");
    }
}
