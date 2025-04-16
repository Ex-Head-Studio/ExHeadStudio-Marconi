using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(AllyShip))]
public class CardAllyShip : CardShipAbstract, ICardDropArea, IShipSelectable, IPointerClickHandler
{

    private AllyShip allyShip;
    private AbstractCard cardToUse;

    private bool isShipSelectable = false;

    private void Start()
    {
        allyShip = GetComponent<AllyShip>();
    }

    void ICardDropArea.CardDrop(AbstractCard card)
    {
        if(card.GetCardEntity() == (int)CardEntityType.AllyShip)
        {
            if (card.GetCardEffects() == null)
            {
                Debug.Log("La carta non ha effetti da applicare.");
                return;
            }

            foreach (AbstractEffectSO effect in card.GetCardEffects())
            {
                Debug.Log("L'effetto" + effect.name + "è stato applicato correttamente.");
                effect.PerformEffect(card, this.gameObject);
            }
        }

    }

    //implementare funzione di hover

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(isShipSelectable)
        {

                if (cardToUse.GetCardEffects() == null)
                {
                    Debug.Log("La carta non ha effetti da applicare.");
                    return;
                }

                foreach (AbstractEffectSO effect in cardToUse.GetCardEffects())
                {
                    Debug.Log("L'effetto" + effect.name + "è stato applicato correttamente.");
                    effect.PerformEffect(cardToUse, this.gameObject);
                }
        }
    }

    public void OnShipSelected(PointerEventData pointerEventData)
    {

        if(isShipSelectable)
        {

                if (cardToUse.GetCardEffects() == null)
                {
                    Debug.Log("La carta non ha effetti da applicare.");
                    return;
                }

                foreach (AbstractEffectSO effect in cardToUse.GetCardEffects())
                {
                    Debug.Log("L'effetto" + effect.name + "è stato applicato correttamente.");
                    effect.PerformEffect(cardToUse, this.gameObject);
                }
        }
    }

    protected override void OnCardDropped(AbstractCard card)
    {
        base.OnCardDropped(card);
    }

    protected override void OnCardSelected(AbstractCard card)
    {
        base.OnCardSelected(card);
        if(card.GetCardEntity() == (int)CardEntityType.AllyShip)
        {
            //animazione che risponde se la carta è selezionata funziona sugli alleati
            transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1).SetLoops(-1, LoopType.Yoyo);
        }

        cardToUse = card;
        isShipSelectable = true;
    }

    protected override void OnCardDeselected(AbstractCard card)
    {
        base.OnCardDeselected(card);
        transform.DOKill(true);

        isShipSelectable = false;
    }
}
