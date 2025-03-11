using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuButtons : MonoBehaviour
{
    /// <summary>
    /// Lo script si occupa di gestire i pulsanti del menu di pausa, viene associato al canvas parent
    /// </summary>
    [Header("Pause Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        pauseMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenuPanel.SetActive(false);
    }

    public void RestartGame()
    {
        //questo forse va modificato
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(0).name);
    }
    public void BackToPauseMenu()
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
