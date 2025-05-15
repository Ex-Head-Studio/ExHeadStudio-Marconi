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
    Enemy
}

public class Tile : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, ICardDropArea
{
    public enum Entity 
    {
        ally = 0,
        enemy = 1,
        empty = 2
    } 

    [SerializeField] private Color _baseColor, _offsetColor, _allyColor, _enemyColor, _emptyColor;
    [SerializeField] private Material _allyMaterial, _enemyMaterial;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private GameObject _highlight;

    private bool _isSelected = false;
    private static bool _isRightClicking = false;
    private static List<Tile> _selectedTiles = new List<Tile>();
    private GridManager _gridManager;

    //messo public per debug
    public TileType _type;
    
    private static int allyCount = 0;
    private static int enemyCount = 0;
    private static Material _currentMaterial;
    int width;
    int height;

    //messo public per debug
    public GameObject tileShip;
   

    [Header("Interaction")]
    [SerializeField] private GameObject allyMovementSignal;
    [SerializeField] private GameObject enemyMovementSignal;
    [SerializeField] private GameObject allyAttackSignal;
    [SerializeField] private GameObject enemyAttackSignal;

    private GameObject activeSignal = null;
    [SerializeField] bool isInteractable = false;
    [SerializeField] private Collider tileCollider;

    public static event Action<Tile> tileSelected;

    private bool dragNDropSelected = false;
    private float InfluenceValue { get; set; }

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

    #region Iscrizione agli eventi

    private void OnEnable() 
    {
        UICardDragNDropHandler.droppableCardSelectedEvent += ActivateTileCollider;
        UICardDragNDropHandler.droppableCardDeselectedEvent += DeactivateTileCollider;
    }

    private void OnDisable() 
    {
        UICardDragNDropHandler.droppableCardSelectedEvent -= ActivateTileCollider;
        UICardDragNDropHandler.droppableCardDeselectedEvent -= DeactivateTileCollider;
    }
    #endregion

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

        /* 
        int gridWidth = _gridManager._width;
        int gridHeight = _gridManager._height;
       
        Debug.Log($"La griglia è {gridWidth}x{gridHeight}");

        if (transform.position.x + transform.position.y < gridWidth - 1) { // Con questo codice non verifica che ci siano SEMPRE 3 ally e 3 enemy 
            if (Random.value > 0.5f && allyCount < 3) {
                _mesh.material = _allyMaterial;
                _type = TileType.Ally;
                allyCount++;
            }
        }
        else if (transform.position.x + transform.position.y > gridWidth - 1) {
            if (Random.value > 0.5f && enemyCount < 3) {
                _mesh.material = _enemyMaterial;
                _type = TileType.Enemy;
                enemyCount++;
            }
        }
        else if (transform.position.x + transform.position.y == gridWidth - 1){
            _mesh.material.color = Color.white;
            _type = TileType.Empty;
        }
        else {
            _mesh.material.color = _emptyColor;
            _type = TileType.Empty;
        }
       */
    }

    // void OnMouseOver () {
    //     if (Input.GetMouseButtonUpAsButton(0)) if (_type != TileType.Empty) GridManager.Instance.SwapTileTypes(this);
    // }

    #region Selezione della tile

    /// <remarks>
    /// <summary>
    /// This faction sets the tile interactable and chooses the tileType based on the entity passed as parameter.
    /// </summary>
    /// <param name="shipFaction"></param>
    /// </remarks>
    public void SetTileInteractable(int shipFaction, Move move = null) 
    {
        if(shipFaction == (int)Entity.ally)
        {
            isInteractable = true;
            tileCollider.enabled = true;
            if(move != null)
            {
                if(move.GetMessageType() == MessageType.attack)
                {
                    allyAttackSignal.SetActive(true);
                    activeSignal = allyAttackSignal;
                }
                else if(move.GetMessageType() == MessageType.movement)
                {
                    allyMovementSignal.SetActive(true);
                    activeSignal = allyMovementSignal;
                }
            }
        }
        else if(shipFaction == (int)Entity.enemy)
        {

            if(move != null)
            {
                if(move.GetMessageType() == MessageType.attack)
                {
                    enemyAttackSignal.SetActive(true);
                    activeSignal = enemyAttackSignal;
                }
                else if(move.GetMessageType() == MessageType.movement)
                {

                    //attenzione!!
                    _type = TileType.Enemy;
                    

                    enemyMovementSignal.SetActive(true);
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
    

    #endregion

    /*void OnMouseUpAsButton() {
		if (Input.GetMouseButtonUp(0))
		{
			if (_type != TileType.Empty) _gridManager.SwapTileTypes(this);
			return;
		}
    }*/

    /*void OnMouseOver() {
        
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("cazzi");
            _isRightClicking = true;
            _selectedTiles.Clear();
            SelectTile();
        }
    }

    void OnMouseUp() {
        if (Input.GetMouseButtonUp(1)) { // Tasto destro rilasciato
            _isRightClicking = false;
            ApplySelectionColor();
        }
    }*/

    private void SelectTile() {
        if (!_selectedTiles.Contains(this)) {
            _selectedTiles.Add(this);
        }
    }

    private void ApplySelectionColor() {
        foreach (var tile in _selectedTiles) {
            tile._mesh.material.color = Color.green;
        }
        _selectedTiles.Clear();
    }

    public bool IsEmpty() 
    {
        return _type == TileType.Empty;
    }

    //Occhio a scrivere nomi di metodi già presenti nella classe padre, rischiamo di sovrascrivere metodi importanti
    public new TileType GetType() 
    {
        return _type;
    }

    //ho dovuto modificare questo metodo perchè non viene registrato correttamente il tipo di tyle
    public void SetType(TileType type, int entity) 
    {
        if (entity == (int)Entity.ally) {
            _type = TileType.Ally;
           // _mesh.material = _allyMaterial;
        }
        else if (entity == (int)Entity.enemy) {
            _type = TileType.Enemy;
           // _mesh.material = _enemyMaterial;
        }
        else
        {
            _type = TileType.Empty;
            //_mesh.material = _emptyMaterial;
        }
    }
    public void SetShip(GameObject ship) 
    {
        if(ship != null)
        {
            tileShip = ship;
            tileShip.transform.position = gameObject.transform.position;
            SetType(this._type, ship.GetComponent<AShip>().faction);
        }
       
    }
    public GameObject GetShip()
    {
        if(tileShip==null)
        {
            //Debug.Log("La nave non è presente");
            return null;
        }
        else
        {
            return tileShip;
        }
        
    }
    public void SetTypeEmpty() {
        _type = TileType.Empty;
        _mesh.material.color = _emptyColor;
    }


    #region ICardDropArea

    void ICardDropArea.CardDrop(AbstractCard card)
    {
        Debug.Log("Card dropped on tile: " + gameObject.name);
    }

    private void ActivateTileCollider(AbstractCard card)
    {
        tileCollider.enabled = true;
    }

    private void DeactivateTileCollider(AbstractCard card)
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

    private void OnDrawGizmos()
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
