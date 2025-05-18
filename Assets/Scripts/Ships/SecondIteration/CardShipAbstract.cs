using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <remarks> <summary>
/// 
///Classe astratta da cui derivano tutte le navi che possono interagire con le carte
/// 
/// </summary></remarks>


public abstract class CardShipAbstract : MonoBehaviour, IPointerClickHandler
{

    protected AbstractCard cardToUse;
    protected Collider shipCollider;

    protected bool isShipSelectable = false;

    //questo bool permette alla nave di riconoscere se è selezionata o meno, per evitare di selezionarla più volte
    //e serve anche a riconoscere l'ascoltatore degli eventi
    protected bool isShipSelected = false;

    //Coda per gli effetti da applicare
    protected Queue<AbstractEffectSO> effectQueue = new Queue<AbstractEffectSO>();

    public virtual void Start()
    {
        shipCollider = GetComponent<Collider>();
    }
    
    #region Iscrizione agli eventi

    protected virtual void OnEnable()
    {
        UICardDragNDropHandler.cardDroppedEvent += OnCardDropped;
        UICard.cardSelectedEvent += OnCardSelected;
        UICard.cardDeselectedEvent += OnCardDeselected;
        AbstractEffectSO.effectEndedEvent += StartNextEffect;
    }

    protected virtual void OnDisable()
    {
        UICardDragNDropHandler.cardDroppedEvent -= OnCardDropped;
        UICard.cardSelectedEvent -= OnCardSelected;
        UICard.cardDeselectedEvent -= OnCardDeselected;
        AbstractEffectSO.effectEndedEvent -= StartNextEffect;
    }

    #endregion

    #region Funzioni di callback per gli eventi
    protected virtual void OnCardDropped(AbstractCard card)
    {
        cardToUse = card;
    }

    //Funzione che viene chiamata quando la carta viene selezionata
    protected virtual void OnCardSelected(AbstractCard card)
    {
        cardToUse = card;
    }

    //Funzione che viene chiamata quando la carta viene deselezionata
    protected virtual void OnCardDeselected(AbstractCard card)
    {
        transform.DOKill(gameObject);

        isShipSelectable = false;
        shipCollider.enabled = false;
        cardToUse = null;
    }

    #endregion

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

    public bool IsSelected()
    {
        return isShipSelected;
    }
    public void DeselectShip()
    {
        isShipSelected = false;
    }
    #endregion

    #region Risoluzione degli effetti
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
        if(isShipSelected)
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

    }
    #endregion

}


