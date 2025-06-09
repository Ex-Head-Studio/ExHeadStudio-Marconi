using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;
using DG.Tweening;
public enum TileType 
{
    Empty,
    Ally,
    Enemy,
    Obstacle,
}

public class Tile : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, ICardDropArea
{
    public enum Entity
    {
        ally = 0,
        enemy = 1,
        empty = 2,
        obstacle = 3,
    } 

    [SerializeField] protected Color _emptyColor;
    [SerializeField] protected MeshRenderer _mesh;
    [SerializeField] protected GameObject _highlight;
    protected static List<Tile> _selectedTiles = new List<Tile>();

    //messo public per debug
    public TileType _type;

    protected GameObject tileShip;
    protected GameObject tileObstacle = null;
   

    [Header("Interaction")]
    [SerializeField] protected GameObject allyMovementSignal;
    [SerializeField] protected GameObject enemyMovementSignal;
    [SerializeField] protected GameObject allyAttackSignal;
    [SerializeField] protected GameObject enemyAttackSignal;

    protected GameObject activeSignal = null;
    [SerializeField] protected bool isInteractable = false;
    [SerializeField] protected Collider tileCollider;

    public static event Action<Tile> tileSelected;

    /* [Header("Highlight Effects")]
    [Tooltip("Effetto che si attiva quando la nave che sta sopra la tile può essere attività")]
    [SerializeField] protected ParticleSystem highlightEffect; */

    void Awake()
    {
        //Forse si può migliorare, ma per ora va bene
        allyMovementSignal.SetActive(false);
        enemyMovementSignal.SetActive(false);
        allyAttackSignal.SetActive(false);
        enemyAttackSignal.SetActive(false);

        tileCollider.enabled = false;
        tileCollider.isTrigger = true;
        tileCollider.providesContacts = true;

        SetTileHighlight(false);
    }

    #region Iscrizione agli eventi

    protected void OnEnable()
    {
        UICardDragNDropHandler.droppableCardSelectedEvent += ActivateTileCollider;
        UICardDragNDropHandler.droppableCardDeselectedEvent += DeactivateTileCollider;
    }

    protected void OnDisable()
    {
        UICardDragNDropHandler.droppableCardSelectedEvent -= ActivateTileCollider;
        UICardDragNDropHandler.droppableCardDeselectedEvent -= DeactivateTileCollider;
    }
    #endregion

    #region Selezione della tile

    /// <remarks>
    /// <summary>
    /// This faction sets the tile interactable and chooses the tileType based on the entity passed as parameter.
    /// </summary>
    /// <param name="shipFaction"></param>
    /// </remarks>
    public void
    SetTileInteractable(int shipFaction, Move move = null)
    {
        if (shipFaction == (int)Entity.ally)
        {
            isInteractable = true;
            tileCollider.enabled = true;
            if (move != null)
            {
                if (move.GetMessageType() == MessageType.attack)
                {
                    allyAttackSignal.SetActive(true);
                    activeSignal = allyAttackSignal;
                }
                else if (move.GetMessageType() == MessageType.movement)
                {
                    allyMovementSignal.SetActive(true);
                    activeSignal = allyMovementSignal;
                }
            }
        }
        else if (shipFaction == (int)Entity.enemy)
        {

            if (move != null)
            {
                if (move.GetMessageType() == MessageType.attack)
                {
                    //enemyAttackSignal.SetActive(true);
                    activeSignal = enemyAttackSignal;
                }
                else if (move.GetMessageType() == MessageType.movement)
                {

                    //attenzione!!
                    _type = TileType.Enemy;


                    //enemyMovementSignal.SetActive(true);
                    activeSignal = enemyMovementSignal;
                }
            }
        }
        else
        {
            _type = TileType.Empty;
        }
    }

    public void SetTileNotInteractable(int shipFaction) 
    {
        if(activeSignal != null)
        {
            activeSignal.SetActive(false);
        }
        _highlight.SetActive(false);
        if(shipFaction == (int)Entity.ally)
        {   
            isInteractable = false;
            tileCollider.enabled = false;
        }
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        if(isInteractable)
        {
            //Debug.Log("Mouse entered tile: " + gameObject.name);
            _highlight.SetActive(true);

            //animazione per l'entrata del puntatore
            transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnKill(() => {transform.DOKill(true);
            });
        }

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) 
    {
        if(isInteractable)
        {
            //Debug.Log("Mouse exited tile: " + gameObject.name);
            _highlight.SetActive(false);
            transform.DOKill(true);
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) 
    {
        if(isInteractable)
        {
            tileSelected?.Invoke(this);
            SetTileNotInteractable((int)Entity.ally);
        }

        //La tile comunica con un evento che è stata selezionata, lo riceverà una nave
    }

    public void SetTileHighlight(bool value)
    {
        //highlightEffect.gameObject.SetActive(value);
    }


    #endregion

    #region Gestione entità
    public new TileType GetType()
    {
        return _type;
    }

    //ho dovuto modificare questo metodo perchè non viene registrato correttamente il tipo di tyle
    public void SetType(TileType type, int entity) 
    {
        if (entity == (int)Entity.ally)
        {
            _type = TileType.Ally;
            // _mesh.material = _allyMaterial;
        }
        else if (entity == (int)Entity.enemy)
        {
            _type = TileType.Enemy;
            // _mesh.material = _enemyMaterial;
        }
        else if (entity == (int)Entity.obstacle)
        {
            _type = TileType.Obstacle;
        }
        else
        {
            _type = TileType.Empty;
            //_mesh.material = _emptyMaterial;
        }
    }
    public virtual void SetShip(GameObject ship) 
    {
        if (ship != null)
        {
            tileShip = ship;
            tileShip.transform.position = gameObject.transform.position;
            SetType(this._type, ship.GetComponent<AShip>().faction);
            ship.GetComponent<AShip>().dodgeChance = -1;
            Debug.Log("Set dodgeChance to -1 in tile: " + name);
        }
       
    }

    public void InstantiateObstacle(GameObject obstacle, Tile tile)
    {
        GameObject tmpObs = Instantiate(obstacle);
        SetType(this._type, (int)Entity.obstacle);
        SetObstacle(tmpObs);
        tmpObs.GetComponent<AbstractObstacle>().SetPosition(GridManager.Instance.GetPositionFromTile(tile));
        tmpObs.GetComponent<AbstractObstacle>().SetTile(tile);
        PlayObstacle();
    }

    // suono di ostacolo
    private FMOD.Studio.EventInstance soundObstacle;

    public void PlayObstacle()
    {
        soundObstacle = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Obstacle");
        soundObstacle.start();
        soundObstacle.release();
    }

    public void SetObstacle(GameObject obstacle)
    {
        if (obstacle != null)
        {
            tileObstacle = obstacle;
            obstacle.transform.position = gameObject.transform.position;
            SetType(this._type, (int)Entity.obstacle);
        }

    }
    public GameObject GetShip()
    {
        if (tileShip == null)
        {
            return null;
        }
        else
        {
            return tileShip;
        }
    }


    public void RemoveObstacle()
    {
        Destroy(GetObstacle());
        SetTypeEmpty();
        SetObstacle(null);
    }

    public GameObject GetObstacle()
    {
        if (tileObstacle == null)
        {
            return null;
        }
        else
        {
            return tileObstacle;
        }
    }
    public void SetTypeEmpty()
    {
        _type = TileType.Empty;
    }


    #endregion

    #region ICardDropArea

    void ICardDropArea.CardDrop(AbstractCard card)
    {
        Debug.Log("Card dropped on tile: " + gameObject.name);
    }

    protected void ActivateTileCollider(AbstractCard card)
    {
        tileCollider.enabled = true;
    }

    protected void DeactivateTileCollider(AbstractCard card)
    {
        tileCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        UICard cardScript;
        if (other.gameObject.TryGetComponent<UICard>(out cardScript) && cardScript.IsCardSelected())
        {
            Debug.Log("Entrato nel collider di: " + gameObject.name);
            _highlight.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        UICard cardScript;
        if (other.gameObject.TryGetComponent<UICard>(out cardScript) || !cardScript.IsCardSelected())
        {
            _highlight.SetActive(false);
        }
    }

    #endregion

    protected void OnDrawGizmos()
    {
        if (tileCollider.enabled)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(tileCollider.bounds.size.x, tileCollider.bounds.size.y, tileCollider.bounds.size.z));
    }
}
