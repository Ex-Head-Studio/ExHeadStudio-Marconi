using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]

public abstract class AbstractCard : MonoBehaviour
{
    [SerializeField] private BaseCardData cardData;

    [SerializeField] private List<AbstractEffectSO> cardEffects = new List<AbstractEffectSO>();
    
    private Image cardImage;    
    private string cardName;
    private string cardDescription;
    private int cardCost;


    private void Start()
    {
        cardImage = GetComponent<Image>();
        cardName = cardData.cardName;
        gameObject.name = cardName;
        

        cardDescription = cardData.cardDescription;
        cardCost = cardData.cardCost;

        // Set the image of the card
        if (cardImage != null && cardData.cardImage != null)
        {
            cardImage.sprite = cardData.cardImage.sprite;
        }

    }   


    //Getters

    public Image GetCardImage()
    {
        return cardImage;
    }
    public string GetCardName()
    {
        return cardName;
    }
    public string GetCardDescription()
    {
        return cardDescription;
    }   
    public int GetCardCost()
    {
        return cardCost;
    }
}