using UnityEngine;

public enum TileType {
    Empty,
    Ally,
    Enemy
}

public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor, _allyColor, _enemyColor, _emptyColor;
    [SerializeField] private Material _allyMaterial, _enemyMaterial;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private GameObject _highlight;
     private TileType _type;
   
    private static int allyCount = 0;
    private static int enemyCount = 0;
    private static Material _currentMaterial;

   

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

    public void Init(bool isOffset) {
        if (transform.position.x + transform.position.y < 4) { // Con questo codice non verifica che ci siano SEMPRE 3 ally e 3 enemy 
            if (Random.value > 0.5f && allyCount < 3) {
                _mesh.material = _allyMaterial;
                _type = TileType.Ally;
                allyCount++;
            }
        }
        else if (transform.position.x + transform.position.y > 4) {
            if (Random.value > 0.5f && enemyCount < 3) {
                _mesh.material = _enemyMaterial;
                _type = TileType.Enemy;
                enemyCount++;
            }
        }
        else {
            _mesh.material.color = _emptyColor;
            _type = TileType.Empty;
        }
    }

    void OnMouseEnter() {
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseDown() {
        if (_type != TileType.Empty) GridManager.Instance.SwapTileTypes(this);
    }

    public bool IsEmpty() {
        return _type == TileType.Empty;
    }

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
    }
}
