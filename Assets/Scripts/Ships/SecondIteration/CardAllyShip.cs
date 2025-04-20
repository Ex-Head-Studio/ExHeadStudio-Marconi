using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

///<summary>
/// The script manages all the interaction of a ship with a card, driving the responses from the ship script
/// <summary>
public class CardAllyShip : CardShipAbstract, ICardDropArea, IPointerClickHandler
{
    private AllyShip allyShip;
    private AbstractCard cardToUse;

    private bool isShipSelectable = false;

    //questo bool permette alla nave di riconoscere se è selezionata o meno, per evitare di selezionarla più volte
    //e serve anche a riconoscere l'ascoltatore degli eventi
    private bool isShipSelected = false;

    private void Start()
    {
        allyShip = GetComponent<AllyShip>();
    }


    //forse si può rimuovere
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
                effect.PerformEffect(new EffectStruct(card, this.gameObject));
            }
        }

    }

    //implementare funzione di hover

    #region Selezione della nave
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(isShipSelectable)
        {
                isShipSelected = true;
                
                if (cardToUse.GetCardEffects() == null)
                {
                    Debug.Log("La carta non ha effetti da applicare.");
                }
                else
                {
                    foreach (AbstractEffectSO effect in cardToUse.GetCardEffects())
                    {
                        Debug.Log("L'effetto" + effect.name + "è stato applicato correttamente.");
                        effect.PerformEffect(new EffectStruct(cardToUse, this.gameObject));
                    }
                }

                cardToUse.InvokeCardUsed(cardToUse);
        }
    }

    #endregion

    /*public void OnShipSelected(PointerEventData pointerEventData)
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
    }*/

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
            cardToUse = card;
            isShipSelectable = true;

            //if(movimento) -> segnala movimenti possibili
            //if(attacco) -> segnala attacchi disponibili
        }
    }

    protected override void OnCardDeselected(AbstractCard card)
    {
        base.OnCardDeselected(card);
        transform.DOKill(true);

        isShipSelectable = false;
    }

     //qui inserisco i metodi di riposta agli effetti, con le differenze dovute
}
