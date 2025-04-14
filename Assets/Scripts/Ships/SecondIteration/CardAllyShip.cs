using UnityEngine;
using System.Collections.Generic;

public class CardAllyShip : CardShipAbstract, ICardDropArea
{

    private List<AbstractEffectSO> cardEffects = null;
    void ICardDropArea.CardDrop(AbstractCard card)
    {
        if(card.GetCardEffects() == null)
        {
            Debug.Log("La carta non ha effetti da applicare.");
            return;
        }
        cardEffects = card.GetCardEffects();
        foreach (AbstractEffectSO effect in cardEffects)
        {
            Debug.Log("L'effetto" + effect.name + "è stato applicato correttamente.");
            effect.PerformEffect(card, this.gameObject);
        }
    }

    protected override void OnCardDropped(AbstractCard card)
    {
        base.OnCardDropped(card);

    }

    protected override void OnCardSelected(AbstractCard card)
    {
        base.OnCardSelected(card);
        //effetto di risposta alla selezione della carta 
    }
}
