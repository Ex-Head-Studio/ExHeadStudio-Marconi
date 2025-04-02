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
    [SerializeField] private GameObject _tilePrefab;

    [SerializeField] private ShipManager _shipManager;

    public Dictionary<Vector2, Tile> _tiles;
    private Dictionary<int, Ship> _ships;


    //mi serve a tenere traccia del numero di tentativi per il riposizionamento
    int attempts = 0;
    void Start()
    {
        GenerateGrid();
    }


    void Awake()
    {
        if(!Instance) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        //_tilePrefab = GetComponent<Tile>();
        //_cam= FindObjectsByType<Camera>()[0];
        //_shipManager = GetComponent<ShipManager>();
    }

   public void GenerateGrid() 
   {
        _tiles = new Dictionary<Vector2, Tile>();
        
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                //Nell'istanziare, prende lo script Tile attaccato all'oggetto creato
                Tile spawnedTile = Instantiate(_tilePrefab, new Vector3(x,y), Quaternion.identity, transform).GetComponent<Tile>();

                //non è corretto, i nomi non corrispondo alle posizioni
                spawnedTile.name = $"Tile {Mathf.Abs(y-4)} {x}";
                

                /*var isOffset = (x + y) % 2 == 1;
                spawnedTile.Init(isOffset);
                */
                _tiles[new Vector2(x, y)] = spawnedTile;
                
            }
        }
    }

    public bool IsValidPosition(Vector2 position)
     {
        //Debug.Log(position + "è valida: "+_tiles.ContainsKey(position));
        if(_tiles.ContainsKey(position) && _tiles[position].GetType() == TileType.Empty)
        {
            return true;
        }
        return false;
    }
    public void InsertShips(Ship ship)
    {
        //brutto, da rifare appena abbiamo tempo
        while(true)
        {
            Vector2Int position = new Vector2Int(Random.Range(0, _width), Random.Range(0, _height));
            if(IsValidPosition(position) && attempts < (_width * _height))
            {
                //fare un controllo su questa logica
                Tile tile = GetTileAtPosition(position);
                tile.SetType(tile.GetType(), ship.faction);
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
    public void MoveShip(Vector2 currentPosition, Vector2 newPosition, int entity)
    {
        if(currentPosition == Vector2.negativeInfinity || newPosition == Vector2.negativeInfinity)
        {
            return;
        }


        //aggiunto per evitare l'eccezione lanciata alla riga 127
        if(!_tiles.ContainsKey(currentPosition) || !_tiles.ContainsKey(newPosition))
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        Debug.Log("La nave in posizione: " + currentPosition.x + " " + currentPosition.y +"\n"+
        "muove in: "+ newPosition.x + " " + newPosition.y);

        GameObject tmpShip;
        Tile currentTile = _tiles[currentPosition];//  GetTileAtPosition(currentPosition);

        //qui lancia un'eccezione
        Tile newTile =  _tiles[newPosition];//  GetTileAtPosition(newPosition);

        if(currentTile == null || newTile == null) 
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        tmpShip = currentTile.GetShip();
        Debug.Log("GridManager, moveship, tmpShip: " + tmpShip.GetComponent<Ship>().shipName);

        Debug.Log("Tipo casella vecchia, tipo nuova: " + currentTile.GetType() + newTile.GetType());
        newTile.SetType(currentTile.GetType(), entity);
        Debug.Log("Tipo casella vecchia, tipo nuova: " + currentTile.GetType() + newTile.GetType());

        //qui ho rimosso un GetShip().gameObject (stefano)
        if(tmpShip != null)
        {
            newTile.SetShip(tmpShip.gameObject);
            currentTile.SetTypeEmpty();
            currentTile.SetShip(null);
        }
    }

    

    //serve ancora questo metodo? (Stefano)
    /*public void SwapTileTypes(Tile selectedTile) {
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
        
    }*/
    
}
