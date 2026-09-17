using UnityEngine;

/// <summary>
/// Zona invisible que muestra un mensaje de ayuda en pantalla cuando el
/// jugador la atraviesa: por ejemplo, cómo moverse, cómo saltar o que puede
/// usar una pared inclinada para llegar a ciertos lugares. Requiere un
/// Collider configurado como Trigger sobre el mismo GameObject.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TooltipZone : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [Header("Mensaje")]
    [TextArea]
    [SerializeField] private string message = "Usa WASD para moverte";
    [SerializeField] private float displaySeconds = 4f;

    [Header("Repetición")]
    [Tooltip("Si está activo, el mensaje vuelve a mostrarse cada vez que el jugador entra en la zona.")]
    [SerializeField] private bool showEveryTime = false;

    private bool hasBeenShownOnce;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider != null && !zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
        {
            return;
        }
        if (hasBeenShownOnce && !showEveryTime)
        {
            return;
        }

        hasBeenShownOnce = true;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTooltip(message, displaySeconds);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.35f);
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            return;
        }
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
    }
}
