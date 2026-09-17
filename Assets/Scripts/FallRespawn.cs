using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Vigila la altura del jugador y, si cae por debajo de un umbral letal
/// (cae al vacío), notifica al GameManager para mostrar la pantalla de
/// muerte y reiniciar el nivel actual.
/// </summary>
public class FallRespawn : MonoBehaviour
{
    [Tooltip("Si la posición Y del jugador cae por debajo de este valor, se considera caída al vacío.")]
    [SerializeField] private float fallHeightThreshold = -10f;

    private bool hasTriggeredVoidDeath;

    private void Update()
    {
        if (!hasTriggeredVoidDeath && transform.position.y < fallHeightThreshold)
        {
            TriggerVoidDeath();
        }
    }

    /// <summary>Dispara la secuencia de muerte por caída al vacío a través del GameManager.</summary>
    private void TriggerVoidDeath()
    {
        hasTriggeredVoidDeath = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            Debug.LogWarning("GameManager no encontrado en la escena; reiniciando nivel directamente.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
