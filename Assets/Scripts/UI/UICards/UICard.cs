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
using UnityEngine.Rendering;



public class UICard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler//, IPointerDownHandler, IPointerUpHandler, IDragHandler,
{
    //IMPORTANTE!! Per far funzionare lo script la camera deve avere un Raycaster3D!!!
    //Il prefab della carta è racchiuso in un wrapper, una empty a cui è associato il box collider

    //Lo script si occupa solo della visualizzazione della carta nella UI, riceve i dati dallo script della carta

    public static event Action<AbstractCard> cardSelectedEvent;
    public static event Action<AbstractCard> cardDeselectedEvent;

    private AbstractCard cardScript;

    private Transform cardTransform;

    [Header("Visual References")]

    [Range(1, 2)]
    [Tooltip("Fattore che aumenta la scale dell'oggetto quando si va in hover")]
    [SerializeField] private float hoverScaleFactor = 1.7f;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject visualImage;
    [SerializeField] private SpriteRenderer spriteRendererBorder;
    [SerializeField] private SpriteRenderer spriteRendererCost;
    [SerializeField] private Color highlightColor;

    private Vector3 selectionScale;
    private Vector3 startingScale;
    private Vector3 hoverScale;

    [Header("Energy System")]
    [SerializeField] private EnergySystem energySystem;

    [Header("Colori Carta")]
    [SerializeField] private Color colorCommandCard;
    [SerializeField] private Color colorSupportCard;
    [SerializeField] private Color colorSpySupportCard;
    private Color tempColor;

    private bool drawGizmos;

    private bool isCardSelected = false;

    private SortingGroup sortingGroup;

    private void OnDestroy()
    {
        cardDeselectedEvent?.Invoke(cardScript);
        isCardSelected = false;
    }
    void Awake()
    {
        selectionScale = transform.localScale * (hoverScaleFactor + 0.1f);
        hoverScale = transform.localScale * hoverScaleFactor;
        startingScale = transform.localScale;  
    }

    private void Start()
    {
        cardTransform = GetComponent<Transform>();
        drawGizmos = true;
        sortingGroup = GetComponent<SortingGroup>();
    }

    public void SetupUICard(AbstractCard card)
    {
        //Assegno lo script
        cardScript = card;

        switch (card.GetCardType())
        {
            case (int)DeckType.CommandDeck:
                spriteRendererBorder.color = colorCommandCard;
                spriteRendererCost.color = colorCommandCard;
                break;
            case (int)DeckType.SupportDeck:
                spriteRendererBorder.color = colorSupportCard;
                spriteRendererCost.color = colorSupportCard;
                break;
            case (int)DeckType.SpyAndSupportDeck:
                spriteRendererBorder.color = colorSpySupportCard;
                spriteRendererCost.color = colorSpySupportCard;
                break;
            default:
                Assert.IsTrue(false, "Card type not supported");
                break;
            
        }

        //Compilo il display
        nameText.text = cardScript.GetCardName();
        gameObject.name = cardScript.GetCardName();

        costText.text = cardScript.GetCardCost().ToString();

        descriptionText.text = cardScript.GetCardDescription();

        visualImage.GetComponent<SpriteRenderer>().sprite = cardScript.GetCardImage();

       
    }


    #region Gestione della selezione

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (energySystem != null && energySystem.currentEnergy < cardScript.GetCardCost())
        {
            transform.DOShakePosition(1f, 0.5f, 10, 90, false, true);
            //cambiare colore


        }
        else
        {
            if (!isCardSelected)
            {
                SelectCard();
                PlayCardSelection();
            }
            else if (isCardSelected)
            {
                DeselectCard(cardScript);
                PlayCardDrop();
            }
        }
    }

    private void SelectCard()
    {
        transform.DOLocalMoveY(transform.localPosition.y + 0.3f, 0.5f);

        tempColor = spriteRendererBorder.color;

        sortingGroup.sortingOrder = 10;

        spriteRendererBorder.color = highlightColor;
        spriteRendererCost.color = highlightColor;

        cardSelectedEvent?.Invoke(cardScript);
        transform.localScale = selectionScale;
        isCardSelected = true;
    }
    public void DeselectCard(AbstractCard cardScript)
    {
        if (this.cardScript.gameObject != null)
        {
            sortingGroup.sortingOrder = 0;

            spriteRendererBorder.color = tempColor;
            spriteRendererCost.color = tempColor;

            cardDeselectedEvent?.Invoke(cardScript);
            transform.localScale = startingScale;
            isCardSelected = false;
            transform.DOLocalMoveY(transform.localPosition.y - 0.3f, 0.5f);
        }
    }
    public void SetInteractable(bool interactable)
    {
        // Disabilita/abilita il componente Button o EventTrigger
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = interactable;
        }
        
        // Se usi EventTrigger, disabilita/abilita quello
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            eventTrigger.enabled = interactable;
        }
        
        // Modifica l'aspetto visivo delle carte
        // per indicare che non sono selezionabili
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = interactable ? 1f : 0.6f;  // Rendi la carta semi-trasparente quando non è interattiva
        }
    }
    public bool IsCardSelected()
    {
        return isCardSelected;
    }

    //funzioni di hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCardSelected)
        {
            transform.localScale = hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCardSelected)
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
        //PlayCardError();
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

    #region Sound Effects
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

    #endregion

}