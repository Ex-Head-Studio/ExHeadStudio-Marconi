using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Definisce le varie fasi di gioco
/// </summary>
public enum GamePhase
{
    None,             // Nessuna fase attiva
    PlanningPrepare,  // Preparazione prima del Planning (distribuzione carte)
    Planning,         // Fase Planning attiva (giocatore può usare carte)
    ActionPrepare,    // Preparazione prima dell'Action (distribuzione carte)
    Action,           // Fase Action attiva (giocatore può usare carte)
    EnemyTurn         // Turno del nemico
}

/// <summary>
/// Gestisce il flusso di gioco, le fasi e i turni
/// </summary>
[RequireComponent(typeof(StartedGameListener))]
[RequireComponent(typeof(StartedTurnEventListener))]
[RequireComponent(typeof(PlanningPhaseEndListener))]
[RequireComponent(typeof(ActionPhaseEndListener))]
public class GameManager2 : MonoBehaviour
{
    // Singleton per accesso globale
    public static GameManager2 Instance { get; private set; }

    [Header("Timing")]
    [SerializeField] private float timeBeforeStartGame = 1f;
    [SerializeField] private float timeBetweenRounds = 1f;
    
    [Header("Eventi di gioco")]
    [Tooltip("Evento di inzio partita/gioco")]
    [SerializeField] private StartedGameEvent startedGameEvent;
    [Tooltip("Evento di inzio turno")]
    [SerializeField] private StartedTurnEvent startedTurnEvent;
    [Tooltip("Evento di fine turno")]
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [Tooltip("Eventi per le fasi del turno del giocatore")]
    [SerializeField] private PlanningPhaseStartEvent planningPhaseStartEvent;
    [SerializeField] private ActionPhaseStartEvent actionPhaseStartEvent;

    // Eventi per comunicare con altri sistemi
    public static event Action OnAllCardsDealt;  // Tutte le carte sono state distribuite
    public static event Action OnAllCardsRemoved; // Tutte le carte sono state rimosse

    // Stato del gioco
    private int numberOfRounds = 0;
    private GamePhase currentPhase = GamePhase.None;
    private bool isInitialized = false;

    /// <summary>
    /// Inizializzazione del GameManager
    /// </summary>
    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
            
        StartCoroutine(StartGame());
    }

    /// <summary>
    /// Registrazione agli eventi
    /// </summary>
    private void OnEnable()
    {
        // Ascolta gli eventi del gestore della mano
        PlayerHandManagerScript.OnHandClearedEvent += OnHandCleared;
        PlayerHandManagerScript.OnHandFullEvent += OnHandFull;
    }

    /// <summary>
    /// Rimozione degli eventi
    /// </summary>
    private void OnDisable()
    {
        PlayerHandManagerScript.OnHandClearedEvent -= OnHandCleared;
        PlayerHandManagerScript.OnHandFullEvent -= OnHandFull;
    }

    /// <summary>
    /// Gestisce l'evento di mano vuota (tutte le carte rimosse)
    /// </summary>
    private void OnHandCleared()
    {
        Debug.Log("GameManager: Tutte le carte sono state rimosse");
        OnAllCardsRemoved?.Invoke();
        
        // Verifica che DeckManager sia disponibile
        if (DeckManager.Instance == null)
        {
            Debug.LogError("DeckManager.Instance è null in OnHandCleared!");
            StartCoroutine(WaitForDeckManager());
            return;
        }
        
        // Gestisci la transizione in base alla fase corrente
        ContinueAfterHandCleared();
    }
    
    /// <summary>
    /// Continua il flusso dopo che la mano è stata svuotata
    /// </summary>
    private void ContinueAfterHandCleared()
    {
        switch (currentPhase)
        {
            case GamePhase.PlanningPrepare:
                // Distribuisci le carte per la fase Planning
                DeckManager.Instance.DrawPlanningCards();
                break;
                
            case GamePhase.Planning:
                // Passa alla preparazione della fase Action
                currentPhase = GamePhase.ActionPrepare;
                DeckManager.Instance.DrawActionCards();
                break;
                
            case GamePhase.Action:
                // Passa al turno nemico
                currentPhase = GamePhase.EnemyTurn;
                ExecuteEnemyTurn();
                break;
        }
    }
    
    /// <summary>
    /// Attende l'inizializzazione del DeckManager
    /// </summary>
    private IEnumerator WaitForDeckManager()
    {
        Debug.Log("In attesa dell'inizializzazione di DeckManager...");
        
        float timeoutTime = Time.time + 3.0f;
        
        while (DeckManager.Instance == null && Time.time < timeoutTime)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        if (DeckManager.Instance == null)
        {
            Debug.LogError("Timeout: DeckManager.Instance non inizializzato!");
        }
        else
        {
            Debug.Log("DeckManager.Instance inizializzato con successo");
            ContinueAfterHandCleared();
        }
    }
    
    /// <summary>
    /// Gestisce l'evento di mano piena (tutte le carte distribuite)
    /// </summary>
    private void OnHandFull()
    {
        Debug.Log("GameManager: Tutte le carte sono state distribuite");
        OnAllCardsDealt?.Invoke();
        
        // Gestisci la transizione in base alla fase corrente
        if (currentPhase == GamePhase.PlanningPrepare)
        {
            // Inizia la fase Planning
            currentPhase = GamePhase.Planning;
            
            // Aggiorna la fase nel DeckManager
            if (DeckManager.Instance != null)
                DeckManager.Instance.SetCurrentPhase(currentPhase);
                
            // Emetti l'evento di inizio fase Planning
            planningPhaseStartEvent?.Invoke(new VoidEvent(0));
        }
        else if (currentPhase == GamePhase.ActionPrepare)
        {
            // Inizia la fase Action
            currentPhase = GamePhase.Action;
            
            // Aggiorna la fase nel DeckManager
            if (DeckManager.Instance != null)
                DeckManager.Instance.SetCurrentPhase(currentPhase);
                
            // Emetti l'evento di inizio fase Action
            actionPhaseStartEvent?.Invoke(new VoidEvent(0));
        }
    }

    #region Gestione degli eventi di gioco
    
    /// <summary>
    /// Chiamato quando il gioco viene avviato
    /// </summary>
    public void OnGameStarted()
    {
        Debug.Log("Game started");
        
        // Verifica che il gioco sia inizializzato
        if (!isInitialized)
        {
            StartCoroutine(DelayedStartTurn());
            return;
        }
        
        // Avvia il primo turno
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
    }

    /// <summary>
    /// Avvia il turno con un ritardo per assicurarsi che tutto sia inizializzato
    /// </summary>
    private IEnumerator DelayedStartTurn()
    {
        yield return new WaitForSeconds(0.5f);
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
    }

    /// <summary>
    /// Chiamato all'inizio di ogni turno
    /// </summary>
    public void OnTurnStarted()
    {
        numberOfRounds++;
        Debug.Log($"Nuovo turno iniziato: {numberOfRounds}");
        
        // Inizia con la fase di preparazione Planning
        currentPhase = GamePhase.PlanningPrepare;
        
        // Svuota la mano del giocatore
        if (PlayerHandManagerScript.Instance != null)
        {
            PlayerHandManagerScript.Instance.DestroyPlayerHand();
        }
        else
        {
            Debug.LogError("PlayerHandManagerScript.Instance è null!");
            OnHandCleared(); // Fallback
        }
    }

    /// <summary>
    /// Chiamato alla fine della fase Planning
    /// </summary>
    public void OnPlanningPhaseEnded()
    {
        Debug.Log("Fase Planning terminata");
        PlayerHandManagerScript.Instance.DestroyPlayerHand();
    }

    /// <summary>
    /// Chiamato alla fine della fase Action
    /// </summary>
    public void OnActionPhaseEnded()
    {
        Debug.Log("Fase Action terminata");
        PlayerHandManagerScript.Instance.DestroyPlayerHand();
    }

    /// <summary>
    /// Esegue il turno delle navi nemiche
    /// </summary>
    private void ExecuteEnemyTurn()
    {
        Debug.Log("Inizia il turno nemico");
        
        // Cerca lo ShipManager
        ShipManager2 shipManager = FindObjectOfType<ShipManager2>();
        
        if (shipManager != null)
        {
            // Esegui le mosse nemiche
            shipManager.EnemyMovesExecution();
        }
        else
        {
            Debug.LogError("ShipManager2 non trovato!");
            StartCoroutine(SimulateEnemyTurn());
        }
    }

    /// <summary>
    /// Simulazione semplice del turno nemico (usato solo come fallback)
    /// </summary>
    private IEnumerator SimulateEnemyTurn()
    {
        Debug.Log("Simulazione turno nemico (fallback)");
        yield return new WaitForSeconds(2f);
        OnEnemyTurnEnded();
    }

    /// <summary>
    /// Chiamato alla fine del turno nemico
    /// </summary>
    public void OnEnemyTurnEnded()
    {
        Debug.Log("Turno nemico terminato, inizio nuovo turno");
        StartCoroutine(WaitNextRound());
    }
    #endregion

    /// <summary>
    /// Inizializza e avvia il gioco
    /// </summary>
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(timeBeforeStartGame);
        
        // Verifica che DeckManager sia inizializzato
        if (DeckManager.Instance == null)
        {
            Debug.LogWarning("DeckManager non inizializzato, attendo...");
            
            float timeout = Time.time + 3.0f;
            while (DeckManager.Instance == null && Time.time < timeout)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            if (DeckManager.Instance == null)
            {
                Debug.LogError("DeckManager non inizializzato dopo l'attesa!");
            }
        }
        
        isInitialized = true;
        startedGameEvent?.Invoke(new VoidEvent(0));
    }

    /// <summary>
    /// Attende prima di iniziare un nuovo turno
    /// </summary>
    private IEnumerator WaitNextRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
    }

    /// <summary>
    /// Restituisce la fase corrente di gioco
    /// </summary>
    public GamePhase GetCurrentPhase()
    {
        return currentPhase;
    }
}