using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detecta objetos interactivos frente a la cámara mediante un Raycast y,
/// al presionar la tecla de interacción (E), ejecuta su respuesta (ver
/// IInteractable). Usa el nuevo Input System, igual que PlayerController.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private const float PromptDisplaySeconds = 2f;

    [Tooltip("Distancia máxima, en metros, a la que se pueden detectar objetos interactivos. Se mide desde la cámara, así que debe ser mayor que la distancia de órbita de ThirdPersonCamera más el alcance deseado.")]
    [SerializeField] private float interactionDistance = 10f;
    [Tooltip("Cámara desde la que se lanza el Raycast de interacción. Si se deja vacío, usa Camera.main.")]
    [SerializeField] private Transform interactionCamera;
    [SerializeField] private LayerMask interactionLayerMask = ~0;

    private IInteractable currentTarget;
    private IInteractable previousTarget;

    private void Awake()
    {
        if (interactionCamera == null && Camera.main != null)
        {
            interactionCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        DetectInteractable();
        ShowPromptOnNewTarget();
        ReadInteractionInput();
        previousTarget = currentTarget;
    }

    /// <summary>Lanza un Raycast desde la cámara para saber si hay un objeto interactivo a la vista.</summary>
    private void DetectInteractable()
    {
        currentTarget = null;
        if (interactionCamera == null)
        {
            return;
        }

        bool hitSomething = Physics.Raycast(
            interactionCamera.position,
            interactionCamera.forward,
            out RaycastHit hit,
            interactionDistance,
            interactionLayerMask,
            QueryTriggerInteraction.Ignore);

        if (hitSomething)
        {
            currentTarget = hit.collider.GetComponentInParent<IInteractable>();
        }
    }

    /// <summary>Muestra el mensaje de "Presiona E para..." apenas un objeto interactivo entra a la mira.</summary>
    private void ShowPromptOnNewTarget()
    {
        if (currentTarget == null || currentTarget == previousTarget || UIManager.Instance == null)
        {
            return;
        }

        UIManager.Instance.ShowTooltip(currentTarget.InteractionPrompt, PromptDisplaySeconds);
    }

    /// <summary>Ejecuta la interacción cuando se presiona la tecla asignada y hay un objetivo válido a la vista.</summary>
    private void ReadInteractionInput()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || currentTarget == null)
        {
            return;
        }

        if (keyboard.eKey.wasPressedThisFrame)
        {
            currentTarget.Interact(gameObject);
        }
    }
}
