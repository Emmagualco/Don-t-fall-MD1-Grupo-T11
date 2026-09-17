using UnityEngine;

/// <summary>
/// Objeto interactivo que le otorga al jugador la capacidad de disparar
/// proyectiles. Al presionar E frente a este objeto (ver PlayerInteraction),
/// habilita el ProjectileLauncher del jugador y se destruye a sí mismo.
/// </summary>
public class WeaponPickup : MonoBehaviour, IInteractable
{
    [Tooltip("Mensaje mostrado mientras el jugador mira el arma, antes de recogerla.")]
    [SerializeField] private string interactionPrompt = "Aprieta E para levantar el arma";
    [Tooltip("Mensaje mostrado en pantalla al recoger el arma.")]
    [SerializeField] private string pickupMessage = "¡Recogiste el arma! Click izquierdo para disparar.";
    [Tooltip("Segundos que el mensaje de recolección permanece visible en pantalla.")]
    [SerializeField] private float messageDisplaySeconds = 2.5f;

    public string InteractionPrompt => interactionPrompt;

    /// <summary>Habilita el disparo en el jugador que recogió el arma y hace desaparecer el objeto.</summary>
    public void Interact(GameObject interactor)
    {
        ProjectileLauncher launcher = interactor != null ? interactor.GetComponent<ProjectileLauncher>() : null;
        if (launcher != null)
        {
            launcher.EnableWeapon();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTooltip(pickupMessage, messageDisplaySeconds);
        }

        Destroy(gameObject);
    }
}
