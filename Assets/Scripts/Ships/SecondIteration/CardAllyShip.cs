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

    //Coda per gli effetti da applicare
    private Queue<AbstractEffectSO> effectQueue = new Queue<AbstractEffectSO>();

    private void Start()
    {
        allyShip = GetComponent<AllyShip>();
        shipCollider = GetComponent<Collider>();
        Assert.IsNotNull(shipCollider, "Ship collider is not assigned in the inspector.");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        AbstractEffectSO.effectEndedEvent += StartNextEffect;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        AbstractEffectSO.effectEndedEvent -= StartNextEffect;
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
                //aggiungo gli effetti alla coda
                effectQueue.Enqueue(effect);
            }

            ResolveEffectQueue();
        }
    }
    protected override void OnCardDropped(AbstractCard card)
    {
        base.OnCardDropped(card);
    }

    #region Selezione della nave
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(isShipSelectable)
        {
            cardToUse.SetNotInteractable();

                //animazione per la selezione della nave
                transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnKill(() => {transform.DOKill(true);});

                isShipSelected = true;
                
                if (cardToUse.GetCardEffects() != null)
                {
                    foreach (AbstractEffectSO effect in cardToUse.GetCardEffects())
                    {
                        //aggiungo gli effetti alla coda
                        effectQueue.Enqueue(effect);
                    }       
                    
                    //Eseguo gli effetti della coda
                    ResolveEffectQueue();                              
                }
        }
    }

    public void ResolveEffectQueue()
    {
        //Dalla coda estraggo il primo effettto e lo eseguo
        if (effectQueue.Count > 0)
        {
            AbstractEffectSO effect = effectQueue.Dequeue();
            effect.StartEffect(0);
            effect.PerformEffect(new EffectStruct(cardToUse, gameObject));
        }
    }

    private void StartNextEffect(int id)
    {
        //Controllo se ci sono effetti nella coda
        if (effectQueue.Count > 0)
        {
            Debug.Log("Ci sono ancora effetti nella coda.");
            ResolveEffectQueue();
        }
        else
        {
            Debug.Log("Non ci sono più effetti nella coda.");
            //Se non ci sono più effetti nella coda, invoco l'evento di fine effetto
            effectQueue.Clear();

            //invoco l'evento di carta usata dopo tutti gli effetti
            cardToUse.InvokeCardUsed(cardToUse);
            isShipSelectable = false;
            isShipSelected = false;
            shipCollider.enabled = false;
        }
    }

    #endregion



    //Funzione che viene chiamata quando la carta viene selezionata
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

    //Funzione che viene chiamata quando la carta viene deselezionata
    protected override void OnCardDeselected(AbstractCard card)
    {
        base.OnCardDeselected(card);
        transform.DOKill(true);

        isShipSelectable = false;
        shipCollider.enabled = false;
        cardToUse = null;
    }

    public bool IsSelected()
    {
        return isShipSelected;
    }
    public void DeselectShip()
    {
        isShipSelected = false;
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
