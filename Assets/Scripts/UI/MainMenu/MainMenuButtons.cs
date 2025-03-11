using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    /// <summary>
    /// Lo script si occupa di gestire i pulsanti del menu principale, viene associato al canvas parent
    /// </summary>
    [Header("Main Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(1).name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OpensOptions()
    {   
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);

    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }
}
