using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;

public class MainMenuButtons : MonoBehaviour
{
    /// <summary>
    /// Lo script si occupa di gestire i pulsanti del menu principale, viene associato al canvas parent
    /// </summary>
    [Header("Main Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;

    [field: Header("FMOD Events")]
    [field: SerializeField] public EventReference startButtonSound { get; private set; }
    [field: SerializeField] public EventReference menuButtonSound { get; private set; }

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        AudioManager.PlayOneShot(startButtonSound, this.transform.position);
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
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);
    }

    public void OpensOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);

    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);
    }
}
