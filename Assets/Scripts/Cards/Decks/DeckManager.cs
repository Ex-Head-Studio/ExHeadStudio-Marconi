using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 
using System;
using UnityEngine.UI;   
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System.Collections;
public enum DeckType
{
    CommandDeck,
    SupportDeck,
    SpyDeck,
}

//permette di aggiornare la mano del giocatore ad ogni turno
[RequireComponent(typeof(StartedTurnEventListener))]
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
    

    //IMPORTANTE!! Aggiungere un raycaster alla camera perchè funzioni, altrimenti non riceve le interazioni

    [Header("Type of deck")]
    [SerializeField] private DeckType deckTypeEnum;
    [SerializeField] private int commandCardAtTurn;

    [Tooltip("Bool che seleziona se la pescata dal mazzo ha un costo")]
    [SerializeField] private bool hasDrawingCost;
    private int drawingCost = 1;
    
    [Header("Cards in the deck")]
    [SerializeField] private List<CardForDeck> cardsInDeck;

    [Header("Deck Parameters")]
    [SerializeField] private float drawTime = 0.1f;
    //[Header("DeckDraw Object")]
    //[SerializeField] private TMP_Text deckCostIcon;

    public static event Action<BaseCardData> cardDrawed;
    public static event Action<DeckType> deckOvering;
    public static event Action<DeckType> deckOvered;

    #region Gestione eventi

    //da associare all'event channel di inzio turno

    public void InstantiateNewPlayerHand()
    {
        Debug.Log("Inizio turno, pesco carte");
        //si può migliorare -> shuffle delle carte
        cardsInDeck = cardsInDeck.OrderBy( x => UnityEngine.Random.value ).ToList();
        StartCoroutine(WaitBeforeDraw(drawTime, commandCardAtTurn));
    }

    #endregion


    //TODO
    #region Gestione del mazzo
    //Inserire logica di mischiata
    //Inserire logica per inserire le carte e moltiplicarla
    #endregion

    #region Gestione del puntatore
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if((int)deckTypeEnum == (int)DeckType.SupportDeck || (int)deckTypeEnum == (int)DeckType.SpyDeck)
        {
            //if ho abbstanza mana
            cardDrawed?.Invoke(cardsInDeck.ElementAt(UnityEngine.Random.Range(0, cardsInDeck.Count)).cardData);
            drawingCost++;
            //aggiornamento UI per il costo della -> deckCostIcon++
        }
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
        //prendi le nuove carte
        cardDrawed?.Invoke(cardsInDeck.ElementAt(UnityEngine.Random.Range(0, cardsInDeck.Count)).cardData);
    }
}