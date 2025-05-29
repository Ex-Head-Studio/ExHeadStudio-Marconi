using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

///<summary>
/// The script manages all the interaction of a ship with a card, driving the responses from the ship script
/// <summary>
public class CardAllyShip : CardShipAbstract
{

   protected override void OnCardSelected(AbstractCard card)
    {
        base.OnCardSelected(card);
        if (card.GetCardEntity() == (int)CardEntityType.AllyShip)
        {
            //animazione che risponde se la carta selezionata funziona sugli alleati
            transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1).SetLoops(-1, LoopType.Yoyo);

            isShipSelectable = true;
            shipCollider.enabled = true;
            
            actualTile = shipScript.GetTileFromShipPosition(); 
            actualTile.SetTileHighlight(true);
        }
    }


    protected override void OnCardDropped(AbstractCard card)
    {
        if(card.GetCardEntity() == (int)CardEntityType.AllyShip && !shipScript.IsMoving())
        {
            if (card.GetCardEffects() == null)
            {
                Debug.Log("La carta non ha effetti da applicare.");
                return;
            }

            foreach (AbstractEffectSO effect in card.GetCardEffects())
            {
                //aggiungo gli effetti alla coda
                effectQueue.Enqueue(effect);
            }

            ResolveEffectQueue();
        }
    }
}
