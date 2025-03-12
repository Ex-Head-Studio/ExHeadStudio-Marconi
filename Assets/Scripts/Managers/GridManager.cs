using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour 
{

    public static GridManager Instance;

    [Header("Grid Parameters")]
    public int _width;
    public int _height;

    [Header("Camera relative position")]

    [SerializeField] private float _camX =0;
    [SerializeField] private float _camY = 0;
    [SerializeField] private float _camZ = -10;

    [SerializeField] private GameObject _tilePrefab;
    

    [SerializeField] private Transform _cam;
    [SerializeField] private ShipManager _shipManager;

    public Dictionary<Vector2, Tile> _tiles;
    private Dictionary<int, Ship> _ships;
    void Start(){
        GenerateGrid();
    }


    void Awake() {
        if(!Instance) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        //_tilePrefab = GetComponent<Tile>();
        //_cam= FindObjectsByType<Camera>()[0];
        //_shipManager = GetComponent<ShipManager>();
    }

   public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                //Nell'istanziare, prende lo script Tile attaccato all'oggetto creato
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x,y), Quaternion.identity, transform).GetComponent<Tile>();

                spawnedTile.name = $"Tile {Mathf.Abs(y-4)} {x}";
                

                /*var isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);
                */
                _tiles[new Vector2(x, y)] = spawnedTile;
                
            }
        }

        _cam.transform.position = new Vector3((float)_width/2 - 0.5f + _camX, (float)_height/2 - 0.5f + _camY, _camZ);

        
        
    }

    public void InsertShips(Ship ship)
    {
        int attempts = 0;
        //brutto, da rifare appena abbiamo tempo
        while(true)
        {
            Vector2 position = new Vector2(Random.Range(0, _width), Random.Range(0, _height));
            if(IsValidPosition(position) && attempts < (_width * _height))
            {
                //fare un controllo su questa logica
                Tile tile = GetTileAtPosition(position);
                tile.SetShip(ship.gameObject);
                ship.position = position;
                break;
            }

            if(attempts >= (_width * _height))
            {
                Debug.LogError("Non c'è spazio per la nave");
                break;
            } 
            attempts++;
        }        
    }

    public Tile GetTileAtPosition(Vector2 position) 
    {
        return _tiles.ContainsKey(position) ? _tiles[position] : null;
    }
    public void MoveShip(Vector2 currentPosition, Vector2 newPosition){
        Tile currentTile = GetTileAtPosition(currentPosition);
        Tile newTile = GetTileAtPosition(newPosition);
        if(currentTile == null || newTile == null) return;

        newTile.SetType(currentTile.GetType());
        newTile.SetShip(currentTile.GetShip().gameObject);

        currentTile.SetTypeEmpty();
        currentTile.SetShip(null);
        
    }

    public bool IsValidPosition(Vector2 position)
     {
        //Debug.Log(position + "è valida: "+_tiles.ContainsKey(position));
        if(_tiles.ContainsKey(position))
        {
            return true;
        }
        return false;
    }

    //serve ancora questo metodo? (Stefano)
    public void SwapTileTypes(Tile selectedTile) {
       /* if (selectedTile == null) return;

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
        */
    }
    
}
