using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 
using System;
using UnityEngine.UI;   
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;
using UnityEngine.Assertions;

/// <summary>
/// Enumerazione che definisce i tipi di mazzi disponibili nel gioco
/// </summary>
public enum DeckType 
{
    CommandDeck,       // Carte comando (usate nella fase Action)
    SupportDeck,       // Carte supporto
    SpyDeck,           // Carte spia
    SpyAndSupportDeck, // Mazzo combinato di carte spia e supporto (usate nella fase Planning)
}

/// <summary>
/// Gestisce la logica dei mazzi di carte, la distribuzione e la pescata
/// </summary>
[RequireComponent(typeof(PlanningPhaseStartListener))]
[RequireComponent(typeof(ActionPhaseStartListener))]
[RequireComponent(typeof(EndedTurnEventListener))]
public class DeckManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Singleton per accesso facile da qualsiasi script
    public static DeckManager Instance { get; private set; }
    
    // Struttura per definire una carta e la sua quantità nel mazzo
    [Serializable]
    private struct CardForDeck
    {
        [SerializeField] public BaseCardData cardData;
        [SerializeField] public int quantity;
    }

    // Modalità di pescata delle carte
    private enum DrawMode
    {
        [Tooltip("Pesca una carta a caso dal mazzo")]
        Random,
        [Tooltip("Pesca una carta in base alla probabilità")]
        Probability,
        [Tooltip("Pesca carte da una lista predefinita")]
        ScriptedList,
        [Tooltip("Pesca una carta fissa, le altre a random e senza ripetizioni")]
        OneFixedAndRandom,
    }
    
    //IMPORTANTE!! Aggiungere un raycaster alla camera perché funzioni, altrimenti non riceve le interazioni

    [Header("Configurazione del mazzo")]
    [SerializeField] private DeckType deckTypeEnum;
    [SerializeField] private DrawMode drawModeEnum;
    
    [Tooltip("Numero di carte da dare in fase Planning")]
    [SerializeField] private int cardsGivenInPlanningPhase = 3;
    [Tooltip("Numero di carte da dare in fase Action")]
    [SerializeField] private int cardsGivenInActionPhase = 3;

    [Header("Riferimenti ai mazzi")]
    [Tooltip("Riferimento al GameObject con il mazzo Planning (Spy&Support)")]
    [SerializeField] private GameObject planningDeckObject;
    [Tooltip("Riferimento al GameObject con il mazzo Action (Command)")]
    [SerializeField] private GameObject actionDeckObject;

    [Tooltip("Se true, pescare una carta ha un costo in energia")]
    [SerializeField] private bool hasDrawingCost;
    private int drawingCost = 1;
    
    [Header("Carte nel mazzo")]
    [Tooltip("Puoi usare questa lista anche per la modalità \"1 Fixed and Random\", la prima è quella fissa")]
    [SerializeField] private List<CardForDeck> cardsInDeck;

    [Tooltip("Lista di carte da pescare in modo scriptato")]
    [SerializeField] private List<CardForDeck> scriptedCards;
    private List<BaseCardData> localScriptedCards;

    [Tooltip("Indice della carta fissa per la modalità OneFixedAndRandom")]
    private int fixIndex = 0;
    private List<int> alreadyDrawedIndexes = new List<int>();

    [Header("Parametri del mazzo")]
    [SerializeField] private float drawTime = 0.1f;

    [Header("Sistema energetico")]
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private EnergyUsedEvent energyUsedEvent;

    [Header("UI")]
    [SerializeField] private TMP_Text deckCostIcon;

    // Riferimenti ai componenti DeckManager dei mazzi secondari
    private DeckManager planningDeck;
    private DeckManager actionDeck;
    
    // Fase corrente di gioco
    private GamePhase currentPhase = GamePhase.None;

    // Eventi per comunicare con altri sistemi
    public static event Action<BaseCardData> cardDrawed;
    public static event Action<DeckType> deckOvering;
    public static event Action<DeckType> deckOvered;

    /// <summary>
    /// Inizializzazione del DeckManager
    /// </summary>
    private void Awake()
    {
        // Configura il singleton (solo per il mazzo principale)
        if (transform.parent == null || transform.parent.GetComponent<DeckManager>() == null)
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("DeckManager: Singleton inizializzato");
                
                // Cerca e inizializza i riferimenti ai mazzi secondari
                InitializeDeckReferences();
            }
            else if (Instance != this)
            {
                Debug.LogWarning("DeckManager: Tentativo di creare più di un'istanza del singleton!");
                Destroy(gameObject);
            }
        }
    }
    
    /// <summary>
    /// Inizializza i riferimenti ai mazzi Planning e Action
    /// </summary>
    private void InitializeDeckReferences()
    {
        // Ottieni il riferimento al mazzo Planning
        if (planningDeckObject != null)
        {
            planningDeck = planningDeckObject.GetComponent<DeckManager>();
            if (planningDeck == null)
            {
                Debug.LogError("Planning Deck Object non ha un componente DeckManager!");
            }
        }
        else
        {
            Debug.LogError("Planning Deck Object non assegnato!");
        }
        
        // Ottieni il riferimento al mazzo Action
        if (actionDeckObject != null)
        {
            actionDeck = actionDeckObject.GetComponent<DeckManager>();
            if (actionDeck == null)
            {
                Debug.LogError("Action Deck Object non ha un componente DeckManager!");
            }
        }
        else
        {
            Debug.LogError("Action Deck Object non assegnato!");
        }
    }

    /// <summary>
    /// Inizializzazione del mazzo
    /// </summary>
    private void Start()
    {
        // Inizializza il costo di pesca
        if(!hasDrawingCost)
        {
            drawingCost = 0;
        }
        
        // Aggiorna l'UI del costo
        if (deckCostIcon != null)
        {
            deckCostIcon.text = "Cost: " + drawingCost.ToString();
        }
        
        // Mescola il mazzo all'inizio
        ShuffleDeck();
    }

    #region Gestione delle fasi di gioco

    /// <summary>
    /// Distribuisce le carte per la fase Planning (Spy&Support)
    /// </summary>
    public void DrawPlanningCards()
    {
        Debug.Log("Distribuzione carte per fase Planning");
        
        // Verifica che questo sia il DeckManager principale
        if (this != Instance)
        {
            Debug.LogError("DrawPlanningCards chiamato su un DeckManager non principale!");
            return;
        }
        
        // Imposta la fase corrente
        currentPhase = GamePhase.PlanningPrepare;
        
        // Usa il mazzo Planning per distribuire le carte
        if (planningDeck != null)
        {
            // Comunica quante carte ci aspettiamo in questa fase
            PlayerHandManagerScript.Instance.SetExpectedCardCount(cardsGivenInPlanningPhase);
            
            // Distribuisci le carte dal mazzo Planning
            planningDeck.InstantiateNewPlayerHand(cardsGivenInPlanningPhase);
        }
        else
        {
            Debug.LogError("Planning Deck non inizializzato!");
        }
    }
    
    /// <summary>
    /// Distribuisce le carte per la fase Action (Command)
    /// </summary>
    public void DrawActionCards()
    {
        Debug.Log("Distribuzione carte per fase Action");
        
        // Verifica che questo sia il DeckManager principale
        if (this != Instance)
        {
            Debug.LogError("DrawActionCards chiamato su un DeckManager non principale!");
            return;
        }
        
        // Imposta la fase corrente
        currentPhase = GamePhase.ActionPrepare;
        
        // Usa il mazzo Action per distribuire le carte
        if (actionDeck != null)
        {
            // Comunica quante carte ci aspettiamo in questa fase
            PlayerHandManagerScript.Instance.SetExpectedCardCount(cardsGivenInActionPhase);
            
            // Distribuisci le carte dal mazzo Action
            actionDeck.InstantiateNewPlayerHand(cardsGivenInActionPhase);
        }
        else
        {
            Debug.LogError("Action Deck non inizializzato!");
        }
    }

    // Questi metodi sono collegati agli eventi di inizio fase,
    // ma non fanno nulla perché ora la distribuzione è gestita dal GameManager
    public void OnPlanningPhaseStart() { }
    public void OnActionPhaseStart() { }

    /// <summary>
    /// Distribuisce un numero specifico di carte dalla cima del mazzo
    /// </summary>
    /// <param name="cardCount">Numero di carte da distribuire</param>
    public void InstantiateNewPlayerHand(int cardCount)
    {
        // Inizializza le carte scriptate se necessario
        if(drawModeEnum == DrawMode.ScriptedList)
        {
            localScriptedCards = scriptedCards.Select(card => card.cardData).ToList();
        }
        else if(drawModeEnum == DrawMode.OneFixedAndRandom)
        {
            alreadyDrawedIndexes.Clear();
            alreadyDrawedIndexes.Add(fixIndex);
        }
        
        // Avvia la coroutine per distribuire le carte con un ritardo
        StartCoroutine(WaitBeforeDraw(drawTime, cardCount-1));
    }
    
    /// <summary>
    /// Versione del metodo compatibile con l'API esistente
    /// </summary>
    public void InstantiateNewPlayerHand()
    {
        InstantiateNewPlayerHand(cardsGivenInPlanningPhase);
    }
    
    #endregion

    #region Gestione del mazzo

    /// <summary>
    /// Mescola casualmente le carte nel mazzo
    /// </summary>
    private void ShuffleDeck() 
    {
        for (int i = cardsInDeck.Count - 1; i > 0; i--) 
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            var temp = cardsInDeck[i];
            cardsInDeck[i] = cardsInDeck[randomIndex];
            cardsInDeck[randomIndex] = temp;
        }
    }   

    /// <summary>
    /// Pesca una carta in base alla probabilità definita dalle quantità
    /// </summary>
    private BaseCardData DrawCardWithProbability()
    {
        // Calcola il totale delle quantità
        int totalQuantity = cardsInDeck.Sum(card => card.quantity);

        // Genera un numero casuale tra 0 e il totale
        int randomValue = UnityEngine.Random.Range(0, totalQuantity);

        // Scorri le carte e seleziona in base alla probabilità
        int cumulativeQuantity = 0;
        foreach (var card in cardsInDeck)
        {
            cumulativeQuantity += card.quantity;
            if (randomValue < cumulativeQuantity)
            {
                return card.cardData;
            }
        }

        Debug.LogError("Errore nella selezione della carta.");
        return null;
    }

    /// <summary>
    /// Pesca una carta casuale dal mazzo
    /// </summary>
    private BaseCardData DrawCardRandom()
    {
        if (cardsInDeck.Count == 0)
        {
            Debug.LogError("Il mazzo è vuoto.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, cardsInDeck.Count);
        return cardsInDeck[randomIndex].cardData;
    }

    /// <summary>
    /// Pesca una carta da una lista predefinita
    /// </summary>
    private BaseCardData DrawCardByList()
    {
        BaseCardData drawnCard = localScriptedCards[localScriptedCards.Count - 1];
        localScriptedCards.RemoveAt(localScriptedCards.Count - 1);
        return drawnCard;
    }

    /// <summary>
    /// Pesca una carta fissa (la prima) e poi carte casuali senza ripetizioni
    /// </summary>
    private BaseCardData DrawFixedAndRandom(int fixedIndex, int cardIndex)
    {
        // Per la prima carta, usa quella all'indice fisso
        if (cardIndex == 0)
        {
            return cardsInDeck[fixedIndex].cardData;
        }
        else
        {
            // Per le altre carte, pesca casualmente senza ripetizioni
            int randomIndex;
            do {
                randomIndex = UnityEngine.Random.Range(fixedIndex, cardsInDeck.Count);
            } while (alreadyDrawedIndexes.Contains(randomIndex));

            alreadyDrawedIndexes.Add(randomIndex);
            return cardsInDeck[randomIndex].cardData;
        }
    }
    #endregion

    #region Gestione dell'input del giocatore

    /// <summary>
    /// Gestisce il click sul mazzo (per pescare una carta)
    /// </summary>
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // Debug delle carte nel mazzo
        Debug.Log($"Carte nel mazzo: {cardsInDeck.Count}");
        Debug.Log($"Nomi carte: {string.Join(", ", cardsInDeck.Select(card => card.cardData.name))}");

        // Verifica se siamo in una fase attiva (non di transizione)
        if (Instance != null && 
            (Instance.currentPhase != GamePhase.Planning && Instance.currentPhase != GamePhase.Action))
        {
            Debug.Log("Non è possibile pescare carte durante le fasi di transizione.");
            return;
        }

        // Pesca con costo energetico
        if (hasDrawingCost && energySystem.currentEnergy >= drawingCost)
        {
            // Rimuovi energia
            energyUsedEvent?.Invoke(drawingCost);
            
            // Pesca carta e incrementa costo
            var drawnCard = DrawCardWithProbability();
            cardDrawed?.Invoke(drawnCard);
            drawingCost++;
        }
        // Pesca senza costo energetico
        else if (!hasDrawingCost && cardsInDeck.Count > 0)
        {
            cardDrawed?.Invoke(cardsInDeck[0].cardData);
            cardsInDeck.RemoveAt(0);
        }

        // Limita il costo massimo
        if (drawingCost >= energySystem.defaultEnergy)
        {
            drawingCost = energySystem.defaultEnergy;
        }
        
        // Aggiorna UI
        if (deckCostIcon != null)
        {
            deckCostIcon.text = "Cost: " + drawingCost.ToString();
        }
    }
    
    /// <summary>
    /// Gestisce quando il puntatore entra sul mazzo
    /// </summary>
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        deckOvering?.Invoke(deckTypeEnum);
    }
    
    /// <summary>
    /// Gestisce quando il puntatore esce dal mazzo
    /// </summary>
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        deckOvered?.Invoke(deckTypeEnum);
    }

    #endregion

    /// <summary>
    /// Coroutine ricorsiva per pescare le carte con un ritardo
    /// </summary>
    private IEnumerator WaitBeforeDraw(float drawTime, int cardIndex)
    {
        // Caso base: nessuna carta da pescare
        if(cardIndex < 0)
        {
            yield break;
        }
        
        // Richiama ricorsivamente per pescare la carta precedente
        yield return WaitBeforeDraw(drawTime, cardIndex-1);
        
        // Attendi il tempo specificato
        yield return new WaitForSeconds(drawTime);

        // Pesca la carta usando la modalità configurata
        switch (drawModeEnum)
        {
            case DrawMode.Random:
                cardDrawed?.Invoke(DrawCardRandom());
                break;
            case DrawMode.Probability:
                cardDrawed?.Invoke(DrawCardWithProbability());
                break;
            case DrawMode.ScriptedList:
                cardDrawed?.Invoke(DrawCardByList());
                break;
            case DrawMode.OneFixedAndRandom:
                cardDrawed?.Invoke(DrawFixedAndRandom(fixIndex, cardIndex));
                break;
        }
    }
    
    /// <summary>
    /// Imposta la fase corrente (chiamato dal GameManager)
    /// </summary>
    public void SetCurrentPhase(GamePhase phase)
    {
        currentPhase = phase;
    }
    
    /// <summary>
    /// Ritorna la fase corrente
    /// </summary>
    public GamePhase GetCurrentPhase()
    {
        return currentPhase;
    }
}