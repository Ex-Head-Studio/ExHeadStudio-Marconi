using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    //tenere a mente che nel tutorial usa delle sprite per le carte in UI
    
    private Image cardImage;
    private string cardName;
    private string cardDescription;
    private int cardCost;

    private AbstractCard cardScript;


    
    [SerializeField] private Collider2D cardCollider;
    private Vector3 startCardDragPosition;

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

    private void OnPointerDrag()
    {
        transform.position = GetPointerPositionInWorldSpace();
    }


    //TODO voglio estenderlo al controller

    private Vector3 GetPointerPositionInWorldSpace()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Set the distance from the camera
        return mousePos;
    }
}