using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la pantalla de victoria que se muestra al completar el último
/// nivel del prototipo. Permite reintentar desde el inicio o volver al menú.
/// </summary>
public class VictoryController : MonoBehaviour
{
    private const string FirstLevelSceneName = "Tutorial";
    private const string MainMenuSceneName = "MainMenu";

    /// <summary>Llamado por el botón "Reintentar" para volver a jugar desde el primer nivel.</summary>
    public void RestartButtonClicked()
    {
        SceneManager.LoadScene(FirstLevelSceneName);
    }

    /// <summary>Llamado por el botón "Menú principal" para volver a la pantalla de inicio.</summary>
    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}
