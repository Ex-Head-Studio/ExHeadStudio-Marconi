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

    [Header("If entity == tile")]
    [Tooltip("If true, the card will be placed on a ship-range")]
    public bool hasToPlaceSomethingOnTile = false;

    private static AbstractCard selectedCard = null;

    private void Start()
    {
        if (cardData.isWorldInteractive)
        {
            gameObject.AddComponent<UICardDragNDropHandler>();
            UICardDragNDropHandler cardDragNDropHandler = GetComponent<UICardDragNDropHandler>();
            cardDragNDropHandler.SetEnergySystem(energySystem);
        }

        hasToPlaceSomethingOnTile = cardData;
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

    // Metodo chiamato quando una carta viene selezionata
    public void OnCardSelected()
    {
        // Deseleziona la carta precedente se presente
        if (selectedCard != null && selectedCard != this)
        {
            selectedCard.DeselectCard();
        }
        
        // Imposta questa carta come selezionata
        selectedCard = this;
        
        // Verifica se la carta ha effetti
        if (cardData.cardEffects != null && cardData.cardEffects.Count > 0)
        {
            // Applica il primo effetto della lista
            AbstractEffectSO effect = cardData.cardEffects[0];
            
            if (effect != null)
            {
                // Imposta l'effetto corrente nel GridManager
                if (GridManager.Instance != null)
                {
                    GridManager.Instance.SetCurrentEffect(effect);
                }
                
                EffectStruct effectStruct = new EffectStruct();
                effectStruct.obj = gameObject; // Inizialmente, l'oggetto è la carta stessa
                effect.PerformEffect(effectStruct);
            }
        }
    }

    // Metodo chiamato quando una carta viene deselezionata
    public void DeselectCard()
    {
        if (selectedCard == this)
        {
            selectedCard = null;
            
            // Disabilita qualsiasi sistema di targeting attivo
            if (GridManager.Instance != null)
            {
                GridManager.Instance.DisableTileSelection();
            }
        }
    }

    // Metodo statico per ottenere la carta selezionata
    public static AbstractCard GetSelectedCard()
    {
        return selectedCard;
    }
}