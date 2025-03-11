using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private InputActionAsset inputs;

    private void Start()
    {
        inputs.FindActionMap("UI").FindAction("PauseGame").performed += ctx => OnPause();
    }


    private void Awake()
    {
        endGamePanel.SetActive(false);
        pauseCanvas.SetActive(false);
    }


    public void OnEndGame(int loser)
    {
        endGamePanel.SetActive(true);
        Debug.Log("End Game, GameMenuManager, loser: " + loser);
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        pauseCanvas.SetActive(true);
        Debug.Log("Pause, GameMenuManager");
    }


}
