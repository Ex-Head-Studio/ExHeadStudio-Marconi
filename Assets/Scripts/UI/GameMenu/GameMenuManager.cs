using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class GameMenuManager : MonoBehaviour
{

    //TODO provare ad inserire un contextmenu per le funzioni dei bottoni

    #region Tutorial canvas
    [Header("Tutorial canvas")]
    [SerializeField] private GameObject tutorialCanvas;

    #endregion

#region Game Menu Panels
    [Header("Game Menu Panels")]
    [Tooltip("Panello di fine partita, mostra il vincitore")]
    [SerializeField] private GameObject endGamePanel;
    [Tooltip("Panello di pausa, mostra le opzioni di pausa")]
    [SerializeField] private GameObject pausePanel;
    [Tooltip("Panello delle opzioni, mostra le opzioni di gioco")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject notificationPanel;
#endregion
#region Messages panels
    [Header("Messages panels")]
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject allyPanel;
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private GameObject confirmPanel;
#endregion
#region Objects panels
    [Header("Objects panels")]
    [SerializeField] private GameObject objectPanel;
    [SerializeField] private GameObject objectDescriptionPanel;
#endregion
    [Header("Input Actions")]
    [Tooltip("Input Actions Asset, permette la mappatura degli input da altri dispositivi")]
    [SerializeField] private InputActionAsset inputs;

    [Header("Ship number SO")]
    [SerializeField] private ShipManagerSO shipManagerSo;

    [Header("Objects stack")]
    [SerializeField] private ObjectsStack objectsStackSO;

    private int enemyShips;
    private int allyShips;


    //TODO verificare se è possibile usare ancora esc per muoversi tra i menu (edit: non tanto)

    //TODO verificare se può essere fatto meglio e senza stringhe


    //eventi di servizio per gestire il ciclo di vita del singleton.
    //prendono un parametro intero perchè non posso farne di void.
    public static event Action<int> GameRestarted;
    public static event Action<int> GameQuitted;
    private void OnEnable()
    {
        inputs.FindActionMap("UI").FindAction("PauseGame").performed += ctx => OnPause();
        inputs.FindActionMap("UI").FindAction("ShowObjectsMenu").performed += ctx => OnObjectDisplay();
        inputs.FindActionMap("UI").FindAction("ExitMenu").performed += ctx => OnObjectHide();

        ObjectsStack.addedFirstObjEvent += ShowNotificationPanel;
        ObjectsStack.removedLastObjEvent += HideNotificationPanel;
    }

    private void OnDisable()
    {
        inputs.FindActionMap("UI").FindAction("PauseGame").performed -= ctx => OnPause();
        inputs.FindActionMap("UI").FindAction("ShowObjectsMenu").performed -= ctx => OnObjectDisplay();  
        inputs.FindActionMap("UI").FindAction("ExitMenu").performed -= ctx => OnObjectHide();

        ObjectsStack.addedFirstObjEvent -= ShowNotificationPanel;
        ObjectsStack.removedLastObjEvent -= HideNotificationPanel;
    }

    private void Start()
    {
        enemyShips = shipManagerSo.GetEnemyShips();
        allyShips = shipManagerSo.GetAllyShips();
    }


    private void Awake()
    {
        endGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        displayPanel.SetActive(true);
        confirmPanel.SetActive(true);
        objectPanel.SetActive(false);
        objectDescriptionPanel.SetActive(false);   
        notificationPanel.SetActive(false);
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

    public void OnObjectDisplay()
    {
        objectPanel.SetActive(true);
        objectDescriptionPanel.SetActive(true);
        allyPanel.SetActive(false);
        enemyPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void OnObjectHide()
    {
        objectPanel.SetActive(false);
        objectDescriptionPanel.SetActive(false);
        allyPanel.SetActive(true);
        enemyPanel.SetActive(true);
        confirmPanel.SetActive(true);
    }

    public void UpdateShipsCount(ShipDestroyedStruct shipDestroyed)
    {
        if(shipDestroyed.entity == (int)Entity.ally)
        {
            allyShips -= 1;
        }
        else if(shipDestroyed.entity == (int)Entity.enemy)
        {
            enemyShips -= 1;
        }
        if(enemyShips <= 0 || allyShips <= 0)
        {
            StartCoroutine(EndGame());
        }
    }


    public int GetAllyShipsbumber()
    {
        return allyShips;
    }

    public int GetEnemyShipsNumber()
    {
        return enemyShips;
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2);
        OnEndGame(enemyShips == 0 ? (int)Entity.enemy : (int)Entity.ally);
    }



    private void ShowNotificationPanel()
    {
        notificationPanel.SetActive(true);
    }

    private void HideNotificationPanel()
    {
        notificationPanel.SetActive(false);
    }

#region Bottoni


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
        GameRestarted?.Invoke(1);
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
        GameQuitted?.Invoke(1);
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
    #endregion

}
