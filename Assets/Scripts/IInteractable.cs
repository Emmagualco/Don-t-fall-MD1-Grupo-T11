using UnityEngine;

/// <summary>
/// Contrato para objetos de la escena que el jugador puede activar mediante
/// el sistema de interacción por Raycast (ver PlayerInteraction). Sigue el
/// mismo patrón de interfaces ya usado en el proyecto (IChangeScene, ICheckPoint, IKillZone).
/// </summary>
public interface IInteractable
{
    /// <summary>Mensaje que indica al jugador cómo activar este objeto (ej. "Presiona E para levantar el arma").</summary>
    string InteractionPrompt { get; }

    /// <summary>Ejecuta la respuesta del objeto al ser interactuado por el jugador que lanzó el Raycast.</summary>
    void Interact(GameObject interactor);
}
