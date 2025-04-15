using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]

public abstract class AbstractCard : MonoBehaviour
{
    [SerializeField] private BaseCardData cardData;

    private void Start()
    {
        gameObject.name = cardData.cardName;
    }   


    //Getters

    public Image GetCardImage()
    {
        return cardData.cardImage;
    }
    public string GetCardName()
    {
        return cardData.cardName;
    }
    public string GetCardDescription()
    {
        return cardData.cardDescription;
    }   
    public int GetCardCost()
    {
        return cardData.cardCost;
    }
    public List<AbstractEffectSO> GetCardEffects()
    {
        return cardData.cardEffects;
    }

    public int GetCardEntity()
    {
        return (int)cardData.cardEntityType;
    }
}