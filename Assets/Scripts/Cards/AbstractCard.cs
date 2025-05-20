using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]

public abstract class AbstractCard : MonoBehaviour
{
    public static event Action<AbstractCard> abstractCardUsed;
    private BaseCardData cardData;
    private EnergyUsedEvent energyUsedEvent;
    private EnergySystem energySystem;

    private UICard UICardScript;


    private void Start()
    {
        if(cardData.isWorldInteractive)
        {
            gameObject.AddComponent<UICardDragNDropHandler>();
            UICardDragNDropHandler cardDragNDropHandler = GetComponent<UICardDragNDropHandler>();
            cardDragNDropHandler.SetEnergySystem(energySystem);
        }

        energyUsedEvent = cardData.energyUsedEvent;
        energySystem = cardData.energySystem;
    }

    public void InvokeCardUsed(AbstractCard cardUsed)
    {
        abstractCardUsed?.Invoke(cardUsed);
        energyUsedEvent?.Invoke(cardData.cardCost);
    }

    //Getters

    
    public Sprite GetCardImage()
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

    public int GetCardType()
    {
        return (int)cardData.cardType;
    }

    public void SetCardData(BaseCardData cardDataSO)
    {
        cardData = cardDataSO;
    }

    public bool isWolrdInteractive()
    {
        return cardData.isWorldInteractive;
    }

    public EnergySystem GetEnergySystem()
    {
        return energySystem;
    }

    public EnergyUsedEvent GetEnergyEvent()
    {

        return energyUsedEvent;
    }

    public void SetNotInteractable()
    {
        if(TryGetComponent<UICard>(out UICardScript))
        {
            UICardScript.SetNotInteractable();
        }
        
    }
}