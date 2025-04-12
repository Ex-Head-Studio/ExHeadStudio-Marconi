using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UICard : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerEnterHandler, IPointerExitHandler
{


    //IMPORTANTE!! Per far funzionare lo script la camera deve avere un Raycaster2D!!!
    //Ho anche associato un layer alle carte, UI
    //tenere a mente che nel tutorial usa delle sprite per le carte in UI
    
    private Image cardImage;
    private string cardName;
    private string cardDescription;
    private int cardCost;

    private AbstractCard cardScript;

    private Transform cardTransform;

    [Range(0, 10)]
    [SerializeField] private float cardDistanceFromCameraMultiplayer = 2f;
    [Range(10, 20)]
    [SerializeField] private float minCardOffesetFromCamera = 10f;

    [Range(1,2)]
    [Tooltip("Fattore che aumenta la scale dell'oggetto quando si va in hover")]
    [SerializeField] private float hoverScaleFactor = 1.1f;

    
    [SerializeField] private Collider2D cardCollider;
    private Vector3 startCardDragPosition;

    private Vector3 mousePos;


    private void Start()
    {
        cardTransform = GetComponent<Transform>();
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

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        // Handle the click event on the card
        // You can implement your logic here, such as showing card details or playing a sound
        Debug.Log($"Card clicked: {cardName}");

        transform.localScale = cardTransform.localScale * hoverScaleFactor;
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
        // Store the initial position of the card when the pointer is pressed down
        startCardDragPosition = transform.position;
        Debug.Log($"Card dragged: {cardName}");
        transform.position = GetPointerPositionInWorldSpace();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cardCollider.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        cardCollider.enabled = true;

        if(hitCollider != null && hitCollider.TryGetComponent<ICardDropArea>(out ICardDropArea dropArea))
        {
            dropArea.OnCardDropped(cardScript);
        }
        else
        {
            // If the card is not dropped on a valid area, return it to the original position
            transform.position = startCardDragPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Call the method to handle dragging the card
        transform.position = GetPointerPositionInWorldSpace();
        Debug.Log("Mouse position: " + Input.mousePosition);
    }

    //TODO voglio estenderlo al controller

    private Vector3 GetPointerPositionInWorldSpace()
    {
        //bisogna tenere a mente le dimensioni della finestra. Gli assi dello schermo hanno origine in basso a sx

        if(Input.mousePosition.y >= Screen.height/2)
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
}