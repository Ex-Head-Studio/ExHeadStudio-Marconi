using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

/// <summary>
/// Gestisce la mano del giocatore e le carte in essa contenute
/// </summary>
[RequireComponent(typeof(PlanningPhaseEndListener))]
[RequireComponent(typeof(ActionPhaseEndEvent))]
[RequireComponent(typeof(EndedTurnEventListener))]
public class PlayerHandManagerScript : MonoBehaviour
{
    // Singleton per accesso globale
    public static PlayerHandManagerScript Instance { get; private set; }
    
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxCardsInHand = 10;

    // Lista delle carte attualmente in mano
    private List<GameObject> cardsInHand = new List<GameObject>();
    
    // Flag per controllare se siamo in transizione tra fasi
    private bool isInPhaseTransition = false;
    
    // Eventi per comunicare lo stato della mano
    public static event Action OnHandClearedEvent;  // Mano vuota
    public static event Action OnHandFullEvent;     // Mano piena
    
    // Quante carte ci aspettiamo in questa fase
    private int expectedCardCount = 0;
    
    /// <summary>
    /// Inizializzazione del gestore della mano
    /// </summary>
    private void Awake()
    {
        // Setup singleton
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("PlayerHandManagerScript: Singleton inizializzato");
        }
        else if (Instance != this)
        {
            Debug.LogWarning("PlayerHandManagerScript: Tentativo di creare più di un'istanza del singleton!");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Registrazione agli eventi
    /// </summary>
    private void OnEnable()
    {
        // Eventi per la gestione delle carte
        DeckManager.cardDrawed += DrawCard;
        AbstractCard.abstractCardUsed += OnCardUsed;
        UICardDragNDropHandler.cardUsedEvent += OnCardUsed;
        UICardDragNDropHandler.droppableCardSelectedEvent += OnCardSelected;
        UICard.cardSelectedEvent += OnCardSelected;
        
        // Eventi per le transizioni di fase
        EndTurnButton2.OnPhaseTransitionStarted += DisableCardSelection;
        GameManager2.OnAllCardsDealt += EnableCardSelection;
    }

    /// <summary>
    /// Rimozione degli eventi
    /// </summary>
    private void OnDisable()
    {
        DeckManager.cardDrawed -= DrawCard;
        AbstractCard.abstractCardUsed -= OnCardUsed;
        UICardDragNDropHandler.cardUsedEvent -= OnCardUsed;
        UICardDragNDropHandler.droppableCardSelectedEvent -= OnCardSelected;
        UICard.cardSelectedEvent -= OnCardSelected;
        
        EndTurnButton2.OnPhaseTransitionStarted -= DisableCardSelection;
        GameManager2.OnAllCardsDealt -= EnableCardSelection;
    }
    
    /// <summary>
    /// Imposta quante carte ci aspettiamo in questa fase
    /// </summary>
    public void SetExpectedCardCount(int count)
    {
        expectedCardCount = count;
    }

    /// <summary>
    /// Crea una nuova carta in mano
    /// </summary>
    private void DrawCard(BaseCardData cardData)
    {
        // Non superare il limite di carte in mano
        if (cardsInHand.Count >= maxCardsInHand) return;
        
        // Crea la carta
        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        PlayCartDraw();
        
        // Configura la carta
        SetUpCard(cardData, newCard);
        cardsInHand.Add(newCard);
        UpdateCardPosition();
        
        // Disabilita l'interazione durante le transizioni
        if (isInPhaseTransition)
        {
            newCard.GetComponent<UICard>().SetInteractable(false);
        }
        
        // Verifica se abbiamo raggiunto il numero atteso di carte
        if (cardsInHand.Count == expectedCardCount)
        {
            OnHandFullEvent?.Invoke();
        }
    }

    /// <summary>
    /// Configura una carta con i dati specificati
    /// </summary>
    private void SetUpCard(BaseCardData cardData, GameObject newCard)
    {
        newCard.AddComponent<CommandCard>();
        AbstractCard cardScript = newCard.GetComponent<AbstractCard>(); 
        cardScript.SetCardData(cardData);
        newCard.GetComponent<UICard>().SetupUICard(cardScript);
    }

    /// <summary>
    /// Gestisce l'utilizzo di una carta
    /// </summary>
    private void OnCardUsed(GameObject cardUsed)
    {
        Destroy(cardUsed);
        cardsInHand.Remove(cardUsed);
        UpdateCardPosition();
    }

    /// <summary>
    /// Gestisce l'utilizzo di una carta (versione alternativa)
    /// </summary>
    private void OnCardUsed(AbstractCard cardUsed)
    {
        Destroy(cardUsed.gameObject);
        cardsInHand.Remove(cardUsed.gameObject);
        UpdateCardPosition();
    }

    /// <summary>
    /// Aggiorna la posizione di tutte le carte nella mano
    /// </summary>
    private void UpdateCardPosition()
    {
        if(cardsInHand.Count == 0) return;
        
        // Calcola lo spazio tra le carte
        float cardSpacing = 1f/maxCardsInHand;
        float firstCardPosition = 0.5f - (cardsInHand.Count-1) * cardSpacing / 2f;
        
        // Posiziona ogni carta lungo la spline
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float t = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = splineContainer.EvaluatePosition(t);
            
            // Anima lo spostamento della carta
            cardsInHand[i].transform.DOMove(splinePosition, 0.25f);
            cardsInHand[i].transform.DOLocalRotateQuaternion(spawnPoint.rotation, 0f);
        }
    }

    /// <summary>
    /// Rimuove tutte le carte dalla mano
    /// </summary>
    public void DestroyPlayerHand()
    {
        // Verifica che la lista delle carte sia valida
        if (cardsInHand == null)
        {
            Debug.LogWarning("DestroyPlayerHand: cardsInHand è null!");
            cardsInHand = new List<GameObject>();
        }
        
        // Se non ci sono carte, notifica subito
        if (cardsInHand.Count == 0)
        {
            OnHandClearedEvent?.Invoke();
            return;
        }
        
        // Disabilita l'interazione con tutte le carte
        DisableCardSelection();
        
        // Rimuovi le carte con animazione
        StartCoroutine(RemoveCardsWithAnimation());
    }
    
    /// <summary>
    /// Rimuove le carte dalla mano con un'animazione
    /// </summary>
    private IEnumerator RemoveCardsWithAnimation()
    {
        // Copia la lista delle carte per evitare modifiche durante l'iterazione
        List<GameObject> cardsToRemove = new List<GameObject>(cardsInHand);
        
        // Anima ogni carta verso il basso
        foreach(GameObject card in cardsToRemove)
        {
            card.transform.DOMove(
                new Vector3(
                    card.transform.position.x, 
                    card.transform.position.y - 20f, 
                    card.transform.position.z
                ), 
                0.4f
            ).SetEase(Ease.InBack)
            .OnComplete(() => {
                // Rimuovi la carta al termine dell'animazione
                if (cardsInHand.Contains(card))
                {
                    cardsInHand.Remove(card);
                    Destroy(card);
                }
            });
            
            // Piccolo ritardo tra ogni carta
            yield return new WaitForSeconds(0.08f);
        }
        
        // Attendi il completamento di tutte le animazioni
        yield return new WaitForSeconds(0.5f);
        
        // Assicurati che tutte le carte siano state rimosse
        foreach(GameObject card in cardsToRemove)
        {
            if (card != null && cardsInHand.Contains(card))
            {
                cardsInHand.Remove(card);
                Destroy(card);
            }
        }
        
        // Pulisci la lista e notifica
        cardsInHand.Clear();
        OnHandClearedEvent?.Invoke();
    }

    /// <summary>
    /// Disabilita la selezione delle carte durante le transizioni
    /// </summary>
    private void DisableCardSelection()
    {
        isInPhaseTransition = true;
        
        foreach(GameObject cardInHand in cardsInHand)
        {
            UICard uiCard = cardInHand.GetComponent<UICard>();
            
            // Deseleziona la carta se selezionata
            if(uiCard.IsCardSelected())
            {
                uiCard.DeselectCard(cardInHand.GetComponent<AbstractCard>());
            }
            
            // Disabilita l'interazione
            uiCard.SetInteractable(false);
        }
    }

    /// <summary>
    /// Abilita la selezione delle carte dopo le transizioni
    /// </summary>
    private void EnableCardSelection()
    {
        isInPhaseTransition = false;
        
        foreach(GameObject cardInHand in cardsInHand)
        {
            cardInHand.GetComponent<UICard>().SetInteractable(true);
        }
    }

    /// <summary>
    /// Gestisce la selezione di una carta
    /// </summary>
    private void OnCardSelected(AbstractCard card)
    {
        // Ignora la selezione durante le transizioni
        if (isInPhaseTransition)
        {
            card.gameObject.GetComponent<UICard>().DeselectCard(card);
            return;
        }
        
        // Deseleziona altre carte selezionate
        foreach(GameObject cardInHand in cardsInHand)
        {
            if(cardInHand.GetComponent<UICard>().IsCardSelected() && cardInHand != card.gameObject)
            {
                cardInHand.GetComponent<UICard>().DeselectCard(cardInHand.GetComponent<AbstractCard>());
            }
        }
    }

    /// <summary>
    /// Riproduce il suono di pescata carta
    /// </summary>
    private FMOD.Studio.EventInstance cartDraw;
    public void PlayCartDraw()
    {
        cartDraw = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardDraw");
        cartDraw.start();
        cartDraw.release();
    }
    
    /// <summary>
    /// Restituisce il numero di carte in mano
    /// </summary>
    public int GetHandCardsCount()
    {
        return cardsInHand.Count;
    }
}