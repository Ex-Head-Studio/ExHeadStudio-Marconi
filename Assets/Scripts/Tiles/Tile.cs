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

public class Tile : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
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
    [SerializeField] private GameObject interactionSignal;
    [SerializeField] bool isInteractable = false;
    [SerializeField] private Collider tileCollider;

    public static event Action<Tile> tileSelected;
    private float InfluenceValue {get; set;}

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

    void Awake() 
    {
        interactionSignal.SetActive(false);
        tileCollider.enabled = false;

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

    [ContextMenu("Set Tyle Interactable" )]
    //Questa funzione è per il debug, per vedere se la tile è interagibile o meno
    private void SetTileInteractableEditor() 
    {
        SetTileInteractable(true);
    }

    public void SetTileInteractable(bool interactable = true) 
    {
        isInteractable = interactable;
        interactionSignal.SetActive(interactable);
        tileCollider.enabled = true;
    }

    [ContextMenu("Set Tyle Not Interactable" )]
    public void SetTileNotInteractable() 
    {
        isInteractable = false;
        interactionSignal.SetActive(false);
        tileCollider.enabled = false;
        _highlight.SetActive(false);
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
        Debug.Log("Mouse clicked tile: " + gameObject.name);
        if(isInteractable)
        {
            tileSelected?.Invoke(this);
            SetTileNotInteractable();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(tileCollider.bounds.size.x, tileCollider.bounds.size.y, tileCollider.bounds.size.z));   
    }
}
