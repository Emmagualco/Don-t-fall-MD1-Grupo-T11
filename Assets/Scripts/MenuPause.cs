using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    private bool pause = false;
    [SerializeField] private GameObject panelUI;


    void Start()
    {
        
    }

   
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause)
            {
                ResumeGame();

            }
            else
            {
                PauseGame();
                Debug.Log("Se pauso el juego...");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }

    }

    private void PauseGame()
    {
        pause = true;
        panelUI.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        pause = false;
        panelUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}

