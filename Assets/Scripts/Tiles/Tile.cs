using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor, _allyColor, _enemyColor, _emptyColor;
    [SerializeField] private MeshRenderer _mesh;
    [SerializeField] private GameObject _highlight;

    // [SerializeField] private bool _isPlaceable;

    // public BaseShip OccupiedShip;
    // public bool Placeable => _isPlaceable && OccupiedShip == null;

    public void Init(bool isOffset) {
        _mesh.material.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter() {
        _highlight.SetActive(true);
    }

    void OnMouseExit() {
        _highlight.SetActive(false);
    }

    void OnMouseDown() {
        if (_mesh.material.color == _allyColor) _mesh.material.color = _enemyColor;
        else if (_mesh.material.color == _enemyColor) _mesh.material.color = _emptyColor;
        else _mesh.material.color = _allyColor;
        Debug.Log(name);
    }

    // public void SetShip(BaseShip ship) {
    //     if(ship.OccupiedTile != null) ship.OccupiedTile.OccupiedShip = null;
    //     ship.transform.position = transform.position;
    //     OccupiedShip = ship;
    //     ship.OccupiedTile = this;
    // }
}
