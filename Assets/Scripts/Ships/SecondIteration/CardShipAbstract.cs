using UnityEngine;


/// <summary>
///Classe astratta da cui derivano tutte le navi che possono interagire con le carte
/// </summary>
[RequireComponent(typeof(Collider))]

public abstract class CardShipAbstract : MonoBehaviour
{
    private void OnEnable()
    {
        UICard.cardDroppedEvent += OnCardDropped;
        UICard.cardSelectedEvent += OnCardSelected;
        UICard.cardDeselectedEvent += OnCardDeselected;
    }

    private void OnDisable()
    {
        UICard.cardDroppedEvent -= OnCardDropped;
        UICard.cardSelectedEvent -= OnCardSelected;
        UICard.cardDeselectedEvent -= OnCardDeselected;
    }

    protected virtual void OnCardDropped(AbstractCard card)
    {
        Debug.Log("Carta " + card.GetCardName() + " è stata droppata su " + gameObject.name);
        // Implementazione di default vuota, può essere sovrascritta dalle classi derivate
    }
    protected virtual void OnCardSelected(AbstractCard card)
    {
        // Implementazione di default vuota, può essere sovrascritta dalle classi derivate
    }

    protected virtual void OnCardDeselected(AbstractCard card)
    {
        // Implementazione di default vuota, può essere sovrascritta dalle classi derivate
    }

}


