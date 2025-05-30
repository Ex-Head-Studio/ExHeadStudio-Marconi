using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class CardTile : MonoBehaviour, IPointerClickHandler
{
    protected AbstractCard cardToUse;
    [SerializeField] protected Collider tileCollider;

    protected bool isTileSelectable = false;

    //questo bool permette alla tile di riconoscere se è selezionata o meno, per evitare di selezionarla più volte
    //e serve anche a riconoscere l'ascoltatore degli eventi
    protected bool isTileSelected = false;

    //Coda per gli effetti da applicare
    protected Queue<AbstractEffectSO> effectQueue = new Queue<AbstractEffectSO>();

    private Tile tileScript;
    private UnityEngine.Vector2 tilePosition;

    private void Start()
    {
        tileScript = GetComponent<Tile>();
        tilePosition = GridManager.Instance.GetPositionFromTile(tileScript);
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


        if (card.GetCardEntity() == (int)CardEntityType.Tile)
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

    //Funzione che viene chiamata quando la carta viene selezionata
    protected virtual void OnCardSelected(AbstractCard card)
    {
        cardToUse = card;
        

        if (card.GetCardEntity() == (int)CardEntityType.Tile)
        {
            if (card.hasToPlaceSomethingOnTile)
            {
                UnityEngine.Vector2 allyPosition = GridManager.Instance.GetAllyShipPosition();
                if (allyPosition == UnityEngine.Vector2.negativeInfinity)
                {
                    Debug.LogError("La nave alleata non è stata trovata.");
                    return;
                }

                for (int x = (int)allyPosition.x - 1; x < (int)allyPosition.x + 1; x++)
                {
                    for (int y = (int)allyPosition.y - 1; y < (int)allyPosition.y + 1; y++)
                    {
                        if (tilePosition.x == x && tilePosition.y == y)
                        {
                            //animazione per il tile
                            transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1).SetLoops(-1, LoopType.Yoyo);
                            isTileSelectable = true;
                            tileCollider.enabled = true;
                        }
                    }
                }
            }
            else
            {
                transform.DOPunchPosition(Vector3.up * 0.1f, 0.5f, 10, 1).SetLoops(-1, LoopType.Yoyo);

                isTileSelectable = true;
                tileCollider.enabled = true;
            }

        }
    }

    //Funzione che viene chiamata quando la carta viene deselezionata
    protected virtual void OnCardDeselected(AbstractCard card)
    {
        transform.DOKill(this.gameObject);

        isTileSelectable = false;
        tileCollider.enabled = false;
        cardToUse = null;
    }

    #endregion

    #region Selezione dell'ostacolo
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isTileSelectable)
        {
            cardToUse.SetNotInteractable();

                //animazione per la selezione della nave
                transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnKill(() => {transform.DOKill(true);});

                isTileSelected = true;
                
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
        return isTileSelected;
    }
    public void DeselectTile()
    {
        isTileSelected = false;
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
        if(isTileSelected)
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
                isTileSelectable = false;
                isTileSelected = false;
                tileCollider.enabled = false;
            }
        }

    }
    #endregion
  
}