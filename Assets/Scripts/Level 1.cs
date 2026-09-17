using UnityEngine;

/// <summary>
/// Se asocia al objetivo del nivel (el cuadro blanco / GOAL). Al activarse
/// (por colisión, detectada desde PlayerController) notifica al GameManager
/// que el jugador completó el nivel actual para que avance al siguiente.
/// </summary>
public class Level1 : MonoBehaviour, IChangeScene
{
    /// <summary>Llamado por PlayerController cuando el jugador toca este objetivo.</summary>
    public void ChangeScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel();
        }
        else
        {
            Debug.LogWarning("GameManager no encontrado en la escena; no se pudo avanzar de nivel.");
        }
    }
}
