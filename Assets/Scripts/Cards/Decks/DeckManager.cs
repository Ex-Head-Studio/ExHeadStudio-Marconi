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

public enum DeckType 
{
    CommandDeck,
    SupportDeck,
    SpyDeck,

    SpyAndSupportDeck,
}

//permette di aggiornare la mano del giocatore ad ogni turno
[RequireComponent(typeof(PlanningPhaseStartListener))]
[RequireComponent(typeof(ActionPhaseStartEvent))]
[RequireComponent(typeof(EndedTurnEventListener))]
public class DeckManager : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    private struct CardForDeck
    {
        [SerializeField] public BaseCardData cardData;
        [SerializeField] public int quantity;

        private CardForDeck(BaseCardData cardData, int quantity)
        {
            this.cardData = cardData;
            this.quantity = quantity;
        }
    }

    private enum DrawMode
    {
        [Tooltip("Pesca una carta a caso dal mazzo")]
        Random,
        [Tooltip("Pesca una carta in base alla probabilità")]
        Probability,
    }
    

    //IMPORTANTE!! Aggiungere un raycaster alla camera perchè funzioni, altrimenti non riceve le interazioni

    [Header("Type of deck")]
    [SerializeField] private DeckType deckTypeEnum;
    [SerializeField] private DrawMode drawModeEnum;
    [SerializeField] private int cardsGivenAtTurn;

    [Tooltip("Bool che seleziona se la pescata dal mazzo ha un costo")]
    [SerializeField] private bool hasDrawingCost;
    private int drawingCost = 1;
    
    [Header("Cards in the deck")]
    [SerializeField] private List<CardForDeck> cardsInDeck;

    [Header("Deck Parameters")]
    [SerializeField] private float drawTime = 0.1f;

    [Header("Energy System")]
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private EnergyUsedEvent energyUsedEvent;

    [Header("DeckDraw Object")]
    [SerializeField] private TMP_Text deckCostIcon;

    public static event Action<BaseCardData> cardDrawed;
    public static event Action<DeckType> deckOvering;
    public static event Action<DeckType> deckOvered;


    private void Start()
    {
        //se non ho costo di pesca, lo imposto a 0
        if(!hasDrawingCost)
        {
            drawingCost = 0;
        }
        deckCostIcon.text = "Cost: " + drawingCost.ToString();
        ShuffleDeck();
    }

    #region Gestione eventi

    //da associare all'event channel di inzio turno
    public void OnPlanningPhaseStart()
    {
        InstantiateNewPlayerHand();
    }

    public void OnActionPhaseStart()
    {
        InstantiateNewPlayerHand();
    }

    public void InstantiateNewPlayerHand()
    {
        StartCoroutine(WaitBeforeDraw(drawTime, cardsGivenAtTurn-1));
    }
    

    #endregion


    #region Gestione del mazzo

    // Funzione per mescolare il mazzo
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

    // Funzione per pescare una carta in base alla probabilità
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

        // In caso di errore (non dovrebbe mai accadere)
        Debug.LogError("Errore nella selezione della carta.");
        return null;
    }

    private BaseCardData DrawCardRandom()
    {
        if (cardsInDeck.Count == 0)
        {
            Debug.LogError("Il mazzo è vuoto.");
            return null;
        }

        // Pesca una carta casuale dal mazzo
        int randomIndex = UnityEngine.Random.Range(0, cardsInDeck.Count);
        BaseCardData drawnCard = cardsInDeck[randomIndex].cardData;
        return drawnCard;
    }
    #endregion

    #region Gestione del puntatore

    // Funzione per gestire cosa fare quando il mazzo viene cliccato
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log(cardsInDeck.Count);
        Debug.Log(string.Join(", ", cardsInDeck.Select(card => card.cardData.name)));

        if (hasDrawingCost && energySystem.currentEnergy >= drawingCost)
        {
            //se ho energia sufficiente per pescare, rimuovo l'energia utilizzata
            energyUsedEvent?.Invoke(drawingCost);
            var drawnCard = DrawCardWithProbability();
            cardDrawed?.Invoke(drawnCard);
            drawingCost++;
            
        }
        //se non ho costo di pesca e mazzo non vuoto
        else if(!hasDrawingCost && cardsInDeck.Count > 0) 
        {
            cardDrawed?.Invoke(cardsInDeck[0].cardData);
            cardsInDeck.RemoveAt(0);
        }


        if(drawingCost >= energySystem.defaultEnergy)
        {
            drawingCost = energySystem.defaultEnergy;
        }
        deckCostIcon.text = "Cost: " + drawingCost.ToString();
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        deckOvering?.Invoke(deckTypeEnum);
        //deckObject.GetComponent<Material>().color = Color.blue;
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        deckOvered?.Invoke(deckTypeEnum);
        //deckObject.GetComponent<Material>().color = Color.green;
    }

    #endregion


    //funzione ricorsiva per pescare le carte
    private IEnumerator WaitBeforeDraw(float drawTime, int cardIndex)
    {
        if(cardIndex <0)
        {
            yield break;
        }
        
        yield return WaitBeforeDraw(drawTime, cardIndex-1);
        yield return new WaitForSeconds(drawTime);

        switch (drawModeEnum)
        {
            case DrawMode.Random:
                cardDrawed?.Invoke(DrawCardRandom());
                break;
            case DrawMode.Probability:
                //prendi le nuove carte
                cardDrawed?.Invoke(DrawCardWithProbability());
                break;
        }

    }
}