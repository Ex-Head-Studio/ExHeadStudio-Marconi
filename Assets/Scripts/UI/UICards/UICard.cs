using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;   
using System;
using Unity.VisualScripting;
using DG.Tweening;
using TMPro;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using System.Runtime.CompilerServices;



public class UICard : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler//, IPointerDownHandler, IPointerUpHandler, IDragHandler,
{
    //IMPORTANTE!! Per far funzionare lo script la camera deve avere un Raycaster3D!!!
    //Il prefab della carta è racchiuso in un wrapper, una empty a cui è associato il box collider

    //Lo script si occupa solo della visualizzazione della carta nella UI, riceve i dati dallo script della carta
    

    //questi campi sono da associare una volta che si ha i placeholder corretti
    private UnityEngine.UI.Image cardImage;
    private string cardName;
    private string cardDescription;
    private int cardCost;

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

    private Vector3 selectionScale;
    private Vector3 startingScale;
    private Vector3 hoverScale;

    [Header("Energy System")]
    [SerializeField] private EnergySystem energySystem;

    private bool drawGizmos;

    private bool isCardSelected = false;

    private void OnDestroy()
    {
        //cardDeselectedEvent?.Invoke(cardScript);
        isCardSelected = false;        
    }
    void Awake()
    {
        selectionScale = transform.localScale * (hoverScaleFactor+0.1f);
        hoverScale = transform.localScale * hoverScaleFactor;
        startingScale = transform.localScale;
    }
    private void Start()
    {
        cardTransform = GetComponent<Transform>();
        drawGizmos = true;
    }

    public void SetupUICard(AbstractCard card)
    {
        cardScript = card;
        cardImage = GetComponent<UnityEngine.UI.Image>();
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

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(energySystem != null && energySystem.currentEnergy < cardScript.GetCardCost())
        {
            transform.DOShakePosition(1f, 0.5f, 10, 90, false, true);
            //cambiare colore
            

        }
        else
        {
            if(!isCardSelected)
            {   
                SelectCard();
                PlayCardSelection();
            }
            else if(isCardSelected)
            {   
                DeselectCard(cardScript);
                PlayCardDrop();
            }
        }
    }

    private void SelectCard()
    {       
        transform.DOLocalMoveY(transform.localPosition.y + 0.3f, 0.5f);
        cardSelectedEvent?.Invoke(cardScript);
        transform.localScale = selectionScale;
        isCardSelected = true;
    }
    public void DeselectCard(AbstractCard cardScript)
    {
        if(this.cardScript.gameObject != null)
        {

                cardDeselectedEvent?.Invoke(cardScript);
                transform.localScale = startingScale;
                isCardSelected = false;
                transform.DOLocalMoveY(transform.localPosition.y - 0.3f, 0.5f);
        }
    }
    public bool IsCardSelected()
    {
        return isCardSelected;
    }

    //funzioni di hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isCardSelected)
        {
            transform.localScale = hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isCardSelected)
        {
            transform.localScale = startingScale;
        }
    }

    /// <summary>
    /// <remarks>Set the card to be not interactable, when an effect is being used</remarks>
    /// </summary>
    public void SetNotInteractable()
    {
        //cardImage.raycastTarget = false;
        //gameObject.raycastTarget = false;
        PlayCardError();
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

    // CARD SOUND EFFECTS
    private FMOD.Studio.EventInstance cardSelection;

    public void PlayCardSelection()
    {
        cardSelection = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardSelection");
        cardSelection.start();
        cardSelection.release();
    }

    private FMOD.Studio.EventInstance cardDrop;

    public void PlayCardDrop()
    {
        cardDrop = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardDrop");
        cardDrop.start();
        cardDrop.release();
    }

    private FMOD.Studio.EventInstance cardError;

    public void PlayCardError()
    {
        cardError = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardError");
        cardError.start();
        cardError.release();
    }
}