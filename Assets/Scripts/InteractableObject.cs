
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Interacción")]
    [SerializeField] private string interactionPrompt = "Presiona E para interactuar";
    [SerializeField] private string interactionMessage = "¡Interactuaste con el objeto!";
    [SerializeField] private float messageDisplaySeconds = 2f;

    [Header("Apariencia")]
    [SerializeField] private Color activatedColor = Color.yellow;
    [SerializeField] private Renderer targetRenderer;

    [Header("Arma")]
    [SerializeField] private bool grantsWeapon = false;

    public string InteractionPrompt => interactionPrompt;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

public void Interact(GameObject interactor)
{
    Debug.Log("Interact() ejecutado en: " + gameObject.name);

    if (grantsWeapon)
    {
        ProjectileLauncher launcher = interactor != null
            ? interactor.GetComponentInChildren<ProjectileLauncher>()
            : null;

        if (launcher == null)
        {
            Debug.LogError(
                "No se encontró ProjectileLauncher en el jugador ni en sus hijos.",
                this
            );
            return;
        }

        Debug.Log("ProjectileLauncher encontrado. Habilitando arma.");

        launcher.EnableWeapon();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTooltip(
                interactionMessage,
                messageDisplaySeconds
            );
        }

        Debug.Log("Destruyendo objeto: " + gameObject.name);

        Destroy(gameObject);
        return;
    }

    if (targetRenderer != null)
    {
        targetRenderer.material.color = activatedColor;
    }

    if (UIManager.Instance != null)
    {
        UIManager.Instance.ShowTooltip(
            interactionMessage,
            messageDisplaySeconds
        );
    }
}

}
