using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;   
using System;

public class UICard : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerEnterHandler, IPointerExitHandler
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
    public static event Action<AbstractCard> cardDroppedEvent;
    public static event Action<AbstractCard> cardSelectedEvent;

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

    [Tooltip("Associare il collider che possiede la empty-parent")]
    [SerializeField] private Collider cardCollider;

    [Header("Parametri di Drag&Drop")]
    [SerializeField] private LayerMask collisionMask;

    private Vector3 startCardDragPosition;

    private Vector3 mousePos;

    private bool drawGizmos;


    private void Start()
    {
        cardTransform = GetComponent<Transform>();
        drawGizmos = true;
        cardScript = GetComponent<AbstractCard>();
        SetupUICard(cardScript);

        //molto importante, non modificare, evita che le navi debbano avere un rigidbody
        cardCollider.providesContacts = true;
    }

    public void SetupUICard(AbstractCard card)
    {
        cardScript = card;
        cardImage = GetComponent<Image>();
        cardName = cardScript.GetCardName();
        gameObject.name = cardName;
        
        cardDescription = cardScript.GetCardDescription();
        cardCost = cardScript.GetCardCost();

        // Set the image of the card
        if (cardImage != null && cardScript.GetCardImage() != null)
        {
            cardImage.sprite = cardScript.GetCardImage().sprite;
        }
    }


    #region Gestione del Drang&Drop

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        cardSelectedEvent?.Invoke(cardScript);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = cardTransform.localScale * hoverScaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = cardTransform.localScale / hoverScaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startCardDragPosition = transform.position;
        transform.position = GetPointerPositionInWorldSpace();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cardDroppedEvent?.Invoke(cardScript);

        int i = 0;

        //Verificare la riga successiva -> è corretta, non serve modifcarla
        cardCollider.enabled = false;
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity, collisionMask);
        while (i < hitColliders.Length)
        {
            if (hitColliders[i] != null && hitColliders[i].TryGetComponent<ICardDropArea>(out ICardDropArea dropArea))
            {
                dropArea.CardDrop(cardScript);
                //prima di distruggere la carta, bisogna anche eliminarla dalla lista delle carte in mano al giocatore
                Destroy(gameObject);
            }
            else
            {
                transform.position = startCardDragPosition;
            }
            i++;
        }
        cardCollider.enabled = true;

        transform.position = startCardDragPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetPointerPositionInWorldSpace();
    }

    //TODO voglio estenderlo al controller

    private Vector3 GetPointerPositionInWorldSpace()
    {
        //bisogna tenere a mente le dimensioni della finestra. Gli assi dello schermo hanno origine in basso a sx

        if(Input.mousePosition.y >= Screen.height/4)
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.y*(cardDistanceFromCameraMultiplayer * cardDistanceFromCameraMultiplayer));
        }
        else
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, minCardOffesetFromCamera);
        }
      
      Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);
      return objPos;
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