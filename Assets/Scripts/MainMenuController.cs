using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla la pantalla de inicio del prototipo. Se asocia al botón "Jugar"
/// para comenzar la partida desde el primer nivel.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    private const string FirstLevelSceneName = "Tutorial";

    /// <summary>Llamado por el botón "Jugar" para iniciar el primer nivel.</summary>
    public void PlayButtonClicked()
    {
        SceneManager.LoadScene(FirstLevelSceneName);
    }
}
