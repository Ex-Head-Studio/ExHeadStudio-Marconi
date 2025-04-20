using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;   
using System;
using Unity.VisualScripting;
using DG.Tweening;
using TMPro;

public class UICard : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler//, IPointerDownHandler, IPointerUpHandler, IDragHandler,
{
    //IMPORTANTE!! Per far funzionare lo script la camera deve avere un Raycaster3D!!!
    //Il prefab della carta è racchiuso in un wrapper, una empty a cui è associato il box collider

    //Lo script si occupa solo della visualizzazione della carta nella UI, riceve i dati dallo script della carta
    

    //questi campi sono da associare una volta che si ha i placeholder corretti
    private Image cardImage;
    private string cardName;
    private string cardDescription;
    private int cardCost;


    //TODO valutare se conviene scrivere un event channel

    public static event Action<AbstractCard> cardSelectedEvent;
    public static event Action<AbstractCard> cardDeselectedEvent;

    private AbstractCard cardScript;

    private Transform cardTransform;

    [Header("Parametri di visualizzazione")]

    [Range(0, 10)]
    [SerializeField] private float cardDistanceFromCameraMultiplayer = 2f;
    [Range(10, 20)]
    [SerializeField] private float minCardOffesetFromCamera = 10f;

    [Range(1,2)]
    [Tooltip("Fattore che aumenta la scale dell'oggetto quando si va in hover")]
    [SerializeField] private float hoverScaleFactor = 1.1f;

    [Header("Visual References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;



    [Header("Events")]
    [SerializeField] private EnergyUsedEvent energyUsedEvent;
    [SerializeField] private EnergySystem energySystem;

    private bool drawGizmos;

    private bool isCardSelected = false;



    private void Start()
    {
        cardTransform = GetComponent<Transform>();
        drawGizmos = true;
    }

    public void SetupUICard(AbstractCard card)
    {
        cardScript = card;
        cardImage = GetComponent<Image>();
        cardName = cardScript.GetCardName();
        gameObject.name = cardName;

        nameText.text = cardName;
        costText.text = "Cost:" + cardScript.GetCardCost().ToString();
        
        cardDescription = cardScript.GetCardDescription();
        cardCost = cardScript.GetCardCost();

        // Set the image of the card
        if (cardImage != null && cardScript.GetCardImage() != null)
        {
            cardImage.sprite = cardScript.GetCardImage().sprite;
        }
    }


    #region Gestione della selezione

    //Qui c'è un bug, non riesce a deselezionare correttamente

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {

        if(energySystem != null && energySystem.currentEnergy < cardScript.GetCardCost())
        {
            transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true);
            //cambiare il colore per un attimo

        }
        else
        {
            if(eventData.pointerClick == this.gameObject)
            {
                cardSelectedEvent?.Invoke(cardScript);
                isCardSelected = true;
                transform.localScale = cardTransform.localScale * hoverScaleFactor;
            }
            else if(eventData.pointerClick != this.gameObject)
            {   
                Debug.Log("Carta deselezionata");
                cardDeselectedEvent?.Invoke(cardScript);
                isCardSelected = false;
                transform.localScale = cardTransform.localScale / hoverScaleFactor;
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isCardSelected)
        {
            transform.localScale = cardTransform.localScale * hoverScaleFactor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isCardSelected)
        {
            transform.localScale = cardTransform.localScale / hoverScaleFactor;
        }
    }

    #endregion
    //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (drawGizmos)
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}