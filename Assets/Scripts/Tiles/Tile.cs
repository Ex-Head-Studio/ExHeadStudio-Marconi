using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileType {
    Empty,
    Ally,
    Enemy
}

public class Tile : MonoBehaviour {
    /*[SerializeField] private Color _baseColor, _offsetColor, _allyColor, _enemyColor, _emptyColor;
    [SerializeField] private Material _allyMaterial, _enemyMaterial;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private GameObject _highlight;

    private bool _isSelected = false;
    private static bool _isRightClicking = false;
    private static List<Tile> _selectedTiles = new List<Tile>();

    private TileType _type;
    
    private static int allyCount = 0;
    private static int enemyCount = 0;
    private static Material _currentMaterial;
    int width = GridManager.Instance.Width;
    int height = GridManager.Instance.Height;*/

   

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

   /* public void Init(bool isOffset) {

        int gridWidth = GridManager.Instance.Width;
        int gridHeight = GridManager.Instance.Height;
        
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
    }*/

    // void OnMouseOver () {
    //     if (Input.GetMouseButtonUpAsButton(0)) if (_type != TileType.Empty) GridManager.Instance.SwapTileTypes(this);
    // }
    
   /* void OnMouseEnter() {
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
			if (_type != TileType.Empty) GridManager.Instance.SwapTileTypes(this);
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
    }

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

    public bool IsEmpty() {
        return _type == TileType.Empty;
    }

    //Occhio a scrivere nomi di metodi già presenti nella classe padre, rischiamo di sovrascrivere metodi importanti
    public TileType GetType() {
        return _type;
    }

    public void SetType(TileType type) {
        if (type == TileType.Ally) {
            _type = TileType.Ally;
            _mesh.material = _allyMaterial;
        }
        else if (type == TileType.Enemy) {
            _type = TileType.Enemy;
            _mesh.material = _enemyMaterial;
        }
    }

    public void SetTypeEmpty() {
        _type = TileType.Empty;
        _mesh.material.color = _emptyColor;
    }*/
}
