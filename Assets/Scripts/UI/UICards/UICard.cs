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
    
    [Range(1, 2.5f)]
    [Tooltip("Fattore che aumenta la scale dell'oggetto quando viene selezionato")]
    [SerializeField] private float selectionScaleFactor = 1.8f;
    
    [Tooltip("Quanto la carta si sposta verso l'alto durante l'hover")]
    [SerializeField] private float hoverYOffset = 0.15f;
    
    [Tooltip("Quanto la carta si sposta verso l'alto quando selezionata")]
    [SerializeField] private float selectionYOffset = 0.3f;
    
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
    private Vector3 startingPosition;
    private Vector3 hoverPosition;
    private Vector3 selectionPosition;
    
    [Header("Energy System")]
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private GameObject canBeUsedBorder;

    [Header("Colori Carta")]
    [SerializeField] private Color colorCommandCard;
    [SerializeField] private Color colorSupportCard;
    [SerializeField] private Color colorSpySupportCard;
    private Color tempColor;

    private bool drawGizmos;

    private bool isCardSelected = false;

    private bool isSelectable = false;

    private SortingGroup sortingGroup;

    private void OnDestroy()
    {
        cardDeselectedEvent?.Invoke(cardScript);
        isCardSelected = false;
    }
    void Awake()
    {
        startingScale = transform.localScale;
        hoverScale = startingScale * hoverScaleFactor;
        selectionScale = startingScale * selectionScaleFactor;
        
        startingPosition = transform.localPosition;
        // Modifica solo la componente Y mantenendo X e Z originali
        hoverPosition = new Vector3(
            startingPosition.x,
            startingPosition.y + hoverYOffset,
            startingPosition.z
        );
        selectionPosition = new Vector3(
            startingPosition.x,
            startingPosition.y + selectionYOffset,
            startingPosition.z
        );
    }

    // Modifica il metodo Start per memorizzare la posizione iniziale dopo un piccolo ritardo
    private void Start()
    {
        cardTransform = GetComponent<Transform>();
        drawGizmos = true;
        sortingGroup = GetComponent<SortingGroup>();
        
        // Aggiungi un ritardo per assicurare che la prima carta sia correttamente posizionata
        Invoke("UpdateStartingPosition", 0.1f);
    }

    private void Update()
    {
        if (energySystem != null && energySystem.currentEnergy < cardScript.GetCardCost())
        {
            canBeUsedBorder.SetActive(false);
        }
        else
        {
            canBeUsedBorder.SetActive(true);
        }
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

    // Metodo pubblico per impostare la selezionabilità
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        
        // Se la carta non è più selezionabile e attualmente è selezionata, deselezionala
        if (!selectable && isCardSelected)
        {
            DeselectCard(cardScript);
        }
    }
    
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        // Se la carta non è selezionabile, non fare nulla
        if (!isSelectable) return;
        
        if (energySystem != null && energySystem.currentEnergy < cardScript.GetCardCost())
        {
            // Riproduci il suono di errore
            PlayCardError();
            
            // Esegui lo shake e poi torna alla posizione iniziale
            transform.DOShakePosition(0.5f, 0.2f, 10, 90, false, true)
                .OnComplete(() => {
                    // Se per caso la carta era già selezionata, deselezionala
                    if (isCardSelected)
                    {
                        DeselectCard(cardScript);
                    }
                    else
                    {
                        // Altrimenti assicurati solo che torni alla posizione iniziale
                        transform.DOScale(startingScale, 0.3f);
                        sortingGroup.sortingOrder = 0;
                        
                        DOTween.To(() => transform.localPosition,
                            position => transform.localPosition = position,
                            new Vector3(transform.localPosition.x, startingPosition.y, transform.localPosition.z),
                            0.3f);
                    }
                });
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
        // Animazione solo dell'asse Y per selezione
        DOTween.To(() => transform.localPosition,
            position => transform.localPosition = position,
            new Vector3(transform.localPosition.x, startingPosition.y + selectionYOffset, transform.localPosition.z),
            0.5f);
        
        // Resto del codice invariato
        tempColor = spriteRendererBorder.color;
        
        // Imposta il sorting order a 2 (massima priorità)
        sortingGroup.sortingOrder = 2;
        
        spriteRendererBorder.color = highlightColor;
        spriteRendererCost.color = highlightColor;

        cardSelectedEvent?.Invoke(cardScript);
        transform.DOScale(selectionScale, 0.5f);
        isCardSelected = true;
    }
    
    public void DeselectCard(AbstractCard cardScript)
    {
        // Ripristina il sorting order a 0
        sortingGroup.sortingOrder = 0;
        spriteRendererBorder.color = tempColor;
        spriteRendererCost.color = tempColor;

        cardDeselectedEvent?.Invoke(cardScript);
        
        transform.DOScale(startingScale, 0.5f);
        
        DOTween.To(() => transform.localPosition,
            position => transform.localPosition = position,
            new Vector3(transform.localPosition.x, startingPosition.y, transform.localPosition.z),
            0.5f);
        
        isCardSelected = false;
    }
    public bool IsCardSelected()
    {
        return isCardSelected;
    }

    //funzioni di hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Se la carta non è selezionabile, non fare effetto hover
        if (!isSelectable) return;
        
        if (!isCardSelected)
        {
            transform.DOScale(hoverScale, 0.3f);
            
            // Imposta il sorting order a 1 durante l'hover
            sortingGroup.sortingOrder = 1;
            
            DOTween.To(() => transform.localPosition,
                position => transform.localPosition = position,
                new Vector3(transform.localPosition.x, startingPosition.y + hoverYOffset, transform.localPosition.z),
                0.3f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Se la carta non è selezionabile, non fare nulla
        if (!isSelectable) return;
        
        if (!isCardSelected)
        {
            transform.DOScale(startingScale, 0.3f);
            
            // Ripristina il sorting order a 0
            sortingGroup.sortingOrder = 0;
            
            DOTween.To(() => transform.localPosition,
                position => transform.localPosition = position,
                new Vector3(transform.localPosition.x, startingPosition.y, transform.localPosition.z),
                0.3f);
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

    public void UpdateStartingPosition()
    {
        // Aggiorna la posizione di partenza dopo il posizionamento della carta
        startingPosition = transform.localPosition;
        
        // Aggiorna anche le posizioni di hover e selezione
        hoverPosition = new Vector3(
            startingPosition.x,
            startingPosition.y + hoverYOffset,
            startingPosition.z
        );
        selectionPosition = new Vector3(
            startingPosition.x,
            startingPosition.y + selectionYOffset,
            startingPosition.z
        );
    }
}