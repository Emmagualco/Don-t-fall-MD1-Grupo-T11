using UnityEngine;

public class WingPickUp : MonoBehaviour, IInteractable
{
    [SerializeField] private string InteractionPromptAla = "Aprieta E para levantar el ala";

    [SerializeField] private string pickupMessage = "¡Recogiste un ala! acercate a la nave y preciona E para repararla.";

    [SerializeField] private float messageDisplaySeconds = 2.5f;
    [SerializeField] private GameObject objectWing;


    [SerializeField] private PlaneInterac Wing;
    
    
    public string InteractionPrompt => InteractionPromptAla;

    public void Interact(GameObject interactor)
    {
        objectWing.SetActive(false);
        Wing.count++;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTooltip(pickupMessage, messageDisplaySeconds);
        }

    }

}
