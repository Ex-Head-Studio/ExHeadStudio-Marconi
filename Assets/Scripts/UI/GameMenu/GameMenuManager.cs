using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{

    //TODO provare ad inserire un contextmenu per le funzioni dei bottoni


    [Header("Game Menu Panels")]
    [Tooltip("Panello di fine partita, mostra il vincitore")]
    [SerializeField] private GameObject endGamePanel;
    [Tooltip("Panello di pausa, mostra le opzioni di pausa")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Panello delle opzioni, mostra le opzioni di gioco")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject confirmPanel;

    [Header("Input Actions")]
    [Tooltip("Input Actions Asset, permette la mappatura degli input da altri dispositivi")]
    [SerializeField] private InputActionAsset inputs;





    
    private void Start()
    {
        //TODO verificare se può essere fatto meglio e senza stringhe
        //TODO verificare se è possibile usare ancora esc per muoversi tra i menu
        inputs.FindActionMap("UI").FindAction("PauseGame").performed += ctx => OnPause();
    }


    private void Awake()
    {
        endGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        displayPanel.SetActive(true);
        confirmPanel.SetActive(true);
    }


    //TODO migliorare questo evento e il parametro che passa
    public void OnEndGame(int loser)
    {
        endGamePanel.SetActive(true);
        displayPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        displayPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }




//BOTTONI

    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        displayPanel.SetActive(true);
        confirmPanel.SetActive(true);
    }

    public void RestartGame()
    {
        //questo forse va modificato
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void BackToPauseMenu()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
