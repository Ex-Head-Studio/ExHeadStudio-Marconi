using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
public class GameMenuManager2 : MonoBehaviour
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
#endregion
#region Messages panels
    [Header("Messages panels")]
    [SerializeField] private GameObject confirmPanel;
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

    //TODO verificare se può essere fatto meglio e senza stringhe        
    //TODO verificare se è possibile usare ancora esc per muoversi tra i menu (edit: non tanto)


    
    //eventi di servizio per gestire il ciclo di vita del singleton.
    //prendono un parametro intero perchè non posso farne di void.
    public static event Action<int> GameRestarted;
    public static event Action<int> GameQuitted;

    private void OnEnable()
    {
        inputs.FindActionMap("UI").FindAction("PauseGame").performed += ctx => OnPause();
        inputs.FindActionMap("UI").FindAction("ShowObjectsMenu").performed += ctx => OnObjectDisplay();
        inputs.FindActionMap("UI").FindAction("ExitMenu").performed += ctx => OnObjectHide();
    }

    private void OnDisable()
    {
        inputs.FindActionMap("UI").FindAction("PauseGame").performed -= ctx => OnPause();
        inputs.FindActionMap("UI").FindAction("ShowObjectsMenu").performed -= ctx => OnObjectDisplay();  
        inputs.FindActionMap("UI").FindAction("ExitMenu").performed -= ctx => OnObjectHide();
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
        confirmPanel.SetActive(true);
    }


    [Header("End Game Settings")]
    [Tooltip("Valore finale dell'opacità del pannello di sfondo")]
    [SerializeField] [Range(0, 1)] private float endGamePanelAlpha = 0.5f;
    [Tooltip("Durata dell'animazione di fade in")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [Tooltip("Tipo di easing per l'animazione")]
    [SerializeField] private Ease fadeEase = Ease.InOutSine;
    [Tooltip("Testo da mostrare in caso di vittoria")]
    [SerializeField] private GameObject victoryText;
    [Tooltip("Testo da mostrare in caso di sconfitta")]
    [SerializeField] private GameObject defeatText;
    [Tooltip("Ritardo prima di mostrare il testo del risultato")]
    [SerializeField] private float resultTextDelay = 0.5f;

    /// <summary>
    /// Gestisce la fine del gioco e mostra il pannello appropriato con animazione
    /// </summary>
    /// <param name="loser">0 se ha perso l'alleato, 1 se ha perso il nemico</param>
    public void OnEndGame(int loser)
    {
        // Attiva il pannello ma imposta l'opacità a 0
        endGamePanel.SetActive(true);
        CanvasGroup panelCanvasGroup = endGamePanel.GetComponent<CanvasGroup>();
        
        // Se non c'è un CanvasGroup, aggiungilo
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = endGamePanel.AddComponent<CanvasGroup>();
        }
        
        // Imposta l'opacità iniziale a 0
        panelCanvasGroup.alpha = 0f;
        
        // Nascondi entrambi i testi di risultato
        if (victoryText != null) victoryText.SetActive(false);
        if (defeatText != null) defeatText.SetActive(false);
        
        // Crea la sequenza di animazione
        Sequence endGameSequence = DOTween.Sequence();
        
        // Animazione di fade in del pannello
        endGameSequence.Append(
            panelCanvasGroup.DOFade(endGamePanelAlpha, fadeInDuration)
            .SetEase(fadeEase)
        );
        
        // Dopo un ritardo, mostra il testo appropriato
        endGameSequence.InsertCallback(fadeInDuration + resultTextDelay, () => {
            // Determina se è vittoria o sconfitta
            if (loser == (int)Entity.enemy && victoryText != null)
            {
                // Il nemico ha perso = vittoria
                victoryText.SetActive(true);
                
                // Animazione opzionale per il testo di vittoria
                AnimateResultText(victoryText);
            }
            else if (loser == (int)Entity.ally && defeatText != null)
            {
                // L'alleato ha perso = sconfitta
                defeatText.SetActive(true);
                
                // Animazione opzionale per il testo di sconfitta
                AnimateResultText(defeatText);
            }
        });
        
        confirmPanel.SetActive(false);
    }

    /// <summary>
    /// Animazione per il testo del risultato
    /// </summary>
    private void AnimateResultText(GameObject textObject)
    {
        // Ottiene il componente RectTransform
        RectTransform textTransform = textObject.GetComponent<RectTransform>();
        
        if (textTransform != null)
        {
            // Imposta la scala iniziale a zero
            textTransform.localScale = Vector3.zero;
            
            // Animazione di scala con rimbalzo
            textTransform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack);
        }
    }

    public void OnPause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        confirmPanel.SetActive(false);
    }

    public void OnObjectDisplay()
    {
        confirmPanel.SetActive(false);
    }

    public void OnObjectHide()
    {
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
        yield return new WaitForSeconds(0.5f);
        OnEndGame(enemyShips == 0 ? (int)Entity.enemy : (int)Entity.ally);
    }

#region Bottoni


//BOTTONI

    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        confirmPanel.SetActive(true);
    }

    public void RestartGame()
    {
        //questo forse va modificato
        Time.timeScale = 1;
        GameRestarted?.Invoke(0);
        SceneManager.LoadScene(this.gameObject.scene.buildIndex);
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

    #region Next Scene Transition
    [Header("Next Scene Transition")]
    [Tooltip("Camera che verrà attivata durante la transizione alla prossima scena")]
    [SerializeField] private GameObject transitionCamera;
    [Tooltip("Indice della scena da caricare")]
    [SerializeField] private int nextSceneIndex = 0;
    [Tooltip("Ritardo prima di caricare la scena successiva")]
    [SerializeField] private float sceneLoadDelay = 1.5f;

    /// <summary>
    /// Metodo da assegnare al bottone nell'UI per transizione alla scena successiva
    /// </summary>
    public void OnNextSceneButtonClick()
    {
        // Verifica se la camera di transizione esiste
        if (transitionCamera == null)
        {
            Debug.LogWarning("Camera di transizione non assegnata! Caricamento scena immediato.");
            SceneManager.LoadScene(nextSceneIndex);
            return;
        }
        
        // Disattiva il pannello di fine gioco con fade out
        if (endGamePanel != null)
        {
            CanvasGroup panelCanvasGroup = endGamePanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup != null)
            {
                // Fade out del pannello
                panelCanvasGroup.DOFade(0f, fadeInDuration / 2)
                    .SetEase(fadeEase)
                    .OnComplete(() => endGamePanel.SetActive(false));
            }
            else
            {
                // Se non c'è CanvasGroup, disattiva subito
                endGamePanel.SetActive(false);
            }
        }
        
        // Attiva la camera di transizione
        transitionCamera.SetActive(true);
        
        // Carica la nuova scena dopo un ritardo
        DOVirtual.DelayedCall(sceneLoadDelay, () => {
            SceneManager.LoadScene(nextSceneIndex);
        });
    }
    #endregion
}
