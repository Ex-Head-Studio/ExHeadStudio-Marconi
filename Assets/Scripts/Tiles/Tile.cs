using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileType 
{
    Empty,
    Ally,
    Enemy
}

public class Tile : MonoBehaviour 
{
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
    public GameObject _ship;
   

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

    void Awake() 
    {
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


    //di tutta questa parte dobbiamo capire cosa serve e cosa no (stefano)
    //Edit: commento le funzioni di hover sulle tiles per non far capire che sono interagibili (stefano)
    /*void OnMouseEnter() 
    {
        _highlight.SetActive(true);

        if (_isRightClicking) {
            SelectTile();
        }
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseUpAsButton() {
		if (Input.GetMouseButtonUp(0))
		{
			if (_type != TileType.Empty) _gridManager.SwapTileTypes(this);
			return;
		}
    }

    void OnMouseOver() {
        
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

    //TODO capire come legge il transform e se capita qualcosa
    public void SetShip(GameObject ship) 
    {
        _ship = ship;
        ship.transform.position = transform.position; //+ eventuale offset e controlli world-space
    }
    public GameObject GetShip()
    {
        if(_ship==null)
        {
            Debug.Log("La nave non è presente");
            return null;
        }
        else
        {
            return _ship;
        }
        
    }
    public void SetTypeEmpty() {
        _type = TileType.Empty;
        _mesh.material.color = _emptyColor;
    }
}
