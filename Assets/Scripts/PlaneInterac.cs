using NUnit.Framework;
using UnityEngine;

public class PlaneInterac : MonoBehaviour, IInteractable
{
    public string InteractionPromptPlane = "Presiona la E para colocar el ala";
    [Header("Si no tenes ningun ala ")]
    [SerializeField] private string pickupMessage = "No tienes ningun ala para colocar.";
    [SerializeField] private float messageDisplaySeconds = 2.5f;
    [Header("Luego de colocar alas")]
    [SerializeField] private string pickupMessage2 = "¡¡Haz colocado el primer ala!!, encuentra la segunda.";
    [Header("Mensaje para completar el nivel ")]
    [SerializeField] private string pickupMessage3 = "¡Felicidades! Has reparado la nave, ya puedes irte.";
    public string InteractionPrompt => InteractionPromptPlane;
    [Header("Cantidad de alas ")]
    [SerializeField] private GameObject[] Wing;

    public int count;

    private int WingCount;
    [Header("Llama a la funcion level 1 q cambia la scene a victoria")]
    [SerializeField] private Level1 level1;
    public void Interact(GameObject interactor)
    {
        
        
        if (count <= 0)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTooltip(pickupMessage, messageDisplaySeconds);
            }
        }
        else if (count <= 2)
        {
            Wing[WingCount].SetActive(true);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTooltip(pickupMessage2, messageDisplaySeconds);
            }
            count--;
            WingCount++;
        }

        if (WingCount == 2)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTooltip(pickupMessage3, messageDisplaySeconds);
            }

            if (level1 != null)
            {
                // Usamos Invoke si querés darle un breve retraso para que se lea el tooltip
                Invoke("EjecutarCambioDeEscena", 1.5f);
            }


        }
    }

    private void EjecutarCambioDeEscena()
    {
        level1.ChangeScene();
    }
}
