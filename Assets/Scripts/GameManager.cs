using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Coordina el flujo entre los niveles del prototipo: avanza al siguiente nivel
/// cuando el jugador llega a la meta y reinicia el nivel actual cuando el
/// jugador muere (por ejemplo, al caer al vacío). Debe existir una única
/// instancia de este componente en cada escena jugable.
/// </summary>
public class GameManager : MonoBehaviour
{
    private const string FirstLevelSceneName = "Tutorial";
    private const string SecondLevelSceneName = "Nivel Final";
    private const string VictorySceneName = "Victory";
    private Transform activeCheckpoint;
    private Transform playerTransform;

    /// <summary>Orden de niveles del prototipo. Al completar el último se carga la pantalla de victoria.</summary>
    private static readonly string[] LevelOrder = { FirstLevelSceneName, SecondLevelSceneName };

    /// <summary>Instancia activa del GameManager en la escena actual.</summary>
    public static GameManager Instance { get; private set; }

    private bool isTransitioningLevel;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Debe llamarse cuando el jugador alcanza el objetivo del nivel (el cuadro
    /// blanco / GOAL). Si es el último nivel, carga la pantalla de victoria;
    /// si no, muestra la pantalla de nivel completado y avanza al siguiente nivel.
    /// </summary>
    public void CompleteLevel()
    {
        if (isTransitioningLevel)
        {
            return;
        }
        isTransitioningLevel = true;

        if (IsLastLevel())
        {
            SceneManager.LoadScene(VictorySceneName);
            return;
        }

        string nextLevelSceneName = GetNextLevelSceneName();
        LoadSceneWithOptionalUI(
            uiManager => uiManager.ShowLevelComplete(() => SceneManager.LoadScene(nextLevelSceneName)),
            nextLevelSceneName);
    }

    /// <summary>
    /// Debe llamarse cuando el jugador muere (por ejemplo, al caer al vacío).
    /// Muestra la pantalla de muerte y luego reinicia el nivel actual.
    /// </summary>
    public void PlayerDied()
    {
        if (isTransitioningLevel)
        {
            return;
        }

        isTransitioningLevel = true;

        string currentLevelSceneName = SceneManager.GetActiveScene().name;

        LoadSceneWithOptionalUI(
            uiManager => uiManager.ShowDeathScreen(RespawnPlayer),
            currentLevelSceneName);
    }

    /// <summary>Ejecuta la transición usando el UIManager si existe; si no, cambia de escena directamente.</summary>
    private void LoadSceneWithOptionalUI(Action<UIManager> withUIManager, string fallbackSceneName)
    {
        if (UIManager.Instance != null)
        {
            withUIManager(UIManager.Instance);
        }
        else
        {
            Debug.LogWarning("UIManager no encontrado en la escena; cambiando de nivel sin pantalla intermedia.");
            SceneManager.LoadScene(fallbackSceneName);
        }
    }

    private string GetNextLevelSceneName()
    {
        string currentLevelSceneName = SceneManager.GetActiveScene().name;
        int currentLevelIndex = Array.IndexOf(LevelOrder, currentLevelSceneName);
        int nextLevelIndex = (currentLevelIndex + 1) % LevelOrder.Length;
        return LevelOrder[nextLevelIndex];
    }

    /// <summary>Returns whether the currently active scene is the last level in the level order.</summary>
    private bool IsLastLevel()
    {
        string currentLevelSceneName = SceneManager.GetActiveScene().name;
        int currentLevelIndex = Array.IndexOf(LevelOrder, currentLevelSceneName);
        return currentLevelIndex == LevelOrder.Length - 1;
    }
    public void SetCheckpoint(Transform checkpoint)
{
    if (checkpoint == null)
    {
        Debug.LogWarning("GameManager: Se intentó guardar un checkpoint nulo.");
        return;
    }

    activeCheckpoint = checkpoint;
    Debug.Log("Checkpoint guardado: " + checkpoint.name);
}

private void RespawnPlayer()
{
    isTransitioningLevel = false;

    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

    if (playerObject == null)
    {
        Debug.LogError("GameManager: No se encontró al jugador.");
        return;
    }

    playerTransform = playerObject.transform;

    if (activeCheckpoint == null)
    {
        Debug.LogWarning("No hay un checkpoint guardado. Se reiniciará el nivel.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;
    }

    playerTransform.position = activeCheckpoint.position;

    Rigidbody rb = playerObject.GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    FallRespawn fallRespawn = playerObject.GetComponent<FallRespawn>();

    if (fallRespawn != null)
    {
        fallRespawn.ResetVoidDeath();
    }

    Debug.Log("Jugador reaparecido en el checkpoint: " + activeCheckpoint.name);
}
}
