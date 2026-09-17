using UnityEngine;

/// <summary>
/// Ejemplo simple de objeto interactivo: al recibir Interact() desde
/// PlayerInteraction, cambia de color y muestra un mensaje en pantalla a
/// través del UIManager. Demuestra el sistema de Raycast e interacción
/// básica con tecla pedido en la Clase 7.
/// </summary>
public class InteractableObject : MonoBehaviour, IInteractable
{
    [Tooltip("Mensaje mostrado mientras el jugador mira este objeto (ej. \"Presiona E para...\").")]
    [SerializeField] private string interactionPrompt = "Presiona E para interactuar";
    [Tooltip("Mensaje mostrado en pantalla al interactuar con este objeto.")]
    [SerializeField] private string interactionMessage = "¡Interactuaste con el objeto!";
    [Tooltip("Segundos que el mensaje permanece visible en pantalla.")]
    [SerializeField] private float messageDisplaySeconds = 2f;
    [Tooltip("Color al que cambia el objeto para dar una respuesta visual observable.")]
    [SerializeField] private Color activatedColor = Color.yellow;
    [Tooltip("Renderer cuyo color cambia al interactuar. Si se deja vacío, se busca uno en este mismo GameObject.")]
    [SerializeField] private Renderer targetRenderer;

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    /// <summary>Respuesta observable del objeto: cambia de color y muestra un mensaje de interacción.</summary>
    public void Interact(GameObject interactor)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.color = activatedColor;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTooltip(interactionMessage, messageDisplaySeconds);
        }
    }
}
