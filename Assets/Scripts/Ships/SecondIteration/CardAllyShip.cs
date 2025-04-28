using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

///<summary>
/// The script manages all the interaction of a ship with a card, driving the responses from the ship script
/// <summary>
public class CardAllyShip : CardShipAbstract, ICardDropArea, IPointerClickHandler
{
    private AllyShip allyShip;
    private AbstractCard cardToUse;
    private Collider shipCollider;

    private bool isShipSelectable = false;

    //questo bool permette alla nave di riconoscere se è selezionata o meno, per evitare di selezionarla più volte
    //e serve anche a riconoscere l'ascoltatore degli eventi
    private bool isShipSelected = false;

    private void Start()
    {
        allyShip = GetComponent<AllyShip>();
        shipCollider = GetComponent<Collider>();
        Assert.IsNotNull(shipCollider, "Ship collider is not assigned in the inspector.");
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
                //animazione per la selezione della nave
                transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f).SetLoops(-1, LoopType.Yoyo);


                isShipSelected = true;
                if (cardToUse.GetCardEffects() == null)
                {
                    Debug.Log("La carta non ha effetti da applicare.");
                }
                else
                {
                    foreach (AbstractEffectSO effect in cardToUse.GetCardEffects())
                    {
                        effect.PerformEffect(new EffectStruct(cardToUse, this.gameObject));
                    }
                }

                cardToUse.InvokeCardUsed(cardToUse);
                isShipSelectable = false;
                shipCollider.enabled = false;
        }
    }

    public bool IsSelected()
    {
        return isShipSelected;
    }

    public void DeselectShip()
    {
        isShipSelected = false;
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
            //animazione che risponde se la carta selezionata funziona sugli alleati
            transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1).SetLoops(-1, LoopType.Yoyo);

            cardToUse = card;
            isShipSelectable = true;
            shipCollider.enabled = true;
        }
    }

    protected override void OnCardDeselected(AbstractCard card)
    {
        base.OnCardDeselected(card);
        transform.DOKill(true);

        isShipSelectable = false;
        shipCollider.enabled = false;
    }



    private void OnDrawGizmos()
    {
        if(isShipSelectable)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, new Vector3(shipCollider.bounds.size.x, shipCollider.bounds.size.y, shipCollider.bounds.size.z));
        }

    }

}
