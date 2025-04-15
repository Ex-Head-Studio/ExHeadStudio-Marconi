using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

//[RequireComponent(typeof(AllyShip))]
public class CardAllyShip : CardShipAbstract, ICardDropArea
{

    private AllyShip allyShip;
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
            transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1);
        }
    }

    protected override void OnCardDeselected(AbstractCard card)
    {
        base.OnCardDeselected(card);
        transform.DOKill(true);
    }
}
