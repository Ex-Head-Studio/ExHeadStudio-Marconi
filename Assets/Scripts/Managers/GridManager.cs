using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour {

    public static GridManager Instance;

    [Header("Grid Parameters")]
    [SerializeField] private int _width, _height;
    [SerializeField] public int Width => _width;
    [SerializeField] public int Height => _height; 

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;
    [SerializeField] private ShipManager _shipManager;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<int, Ship> _ships;
    void Start(){
        GenerateGrid();
    }

    void Awake() {
        Instance = this;
        //_tilePrefab = GetComponent<Tile>();
        //_cam= FindObjectsByType<Camera>()[0];
        //_shipManager = GetComponent<ShipManager>();
    }

    public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x,y), Quaternion.identity, transform);
                spawnedTile.name = $"Tile {Mathf.Abs(y-4)} {x}";
                

                var isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);

                _tiles[new Vector2(x, y)] = spawnedTile;
                
            }
        }

        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height/2 - 0.5f, -5);

        
    }

    public Tile GetTileAtPosition(Vector2 position) {
        return _tiles.ContainsKey(position) ? _tiles[position] : null;
    }

    public void SwapTileTypes(Tile selectedTile) {
        if (selectedTile == null) return;

        Vector2 currentPos = new Vector2(selectedTile.transform.position.x, selectedTile.transform.position.y);
        Vector2[] adjacentPositions = {
            currentPos + Vector2.up,
            currentPos + Vector2.down,
            currentPos + Vector2.left,
            currentPos + Vector2.right
        };

        adjacentPositions = adjacentPositions.OrderBy(x => UnityEngine.Random.value).ToArray();

        foreach (var pos in adjacentPositions) {
            Tile adjacentTile = GetTileAtPosition(pos);
            if (adjacentTile != null && adjacentTile.IsEmpty()) {
                adjacentTile.SetType(selectedTile.GetType());
                selectedTile.SetTypeEmpty();
                break;
            }
        }
    }


    public void MoveShip(Ship ship){
        //FindInGrid(ship.position);

        /*idTile=_tiles[ship.position];
        if(_ships[idTile].GetInstanceID() == ship.GetInstanceID()){
            _ships[idTile]=null;
        }
        else if{ships[idTile].GetInstanceID() != ship.GetInstanceID()}{
            nextTile=_tiles[ship.nextPos];
            _ships[nextTile]=ship;
        }
        */
        
        
        //TODO: la nave si sposta nel nuovo tile e il tile precedente viene svuotato

    }
    
}
