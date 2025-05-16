using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]

public class GridManager : MonoBehaviour, ICardDropArea
{

    public static GridManager Instance;

    [Header("Grid Parameters")]
    public int _width;
    public int _height;
    [SerializeField] private GameObject _tilePrefab;

    public Dictionary<Vector2, Tile> _tiles;
    private Dictionary<int, Ship> _ships;

    private Collider gridCollider;

    //mi serve a tenere traccia del numero di tentativi per il riposizionamento
    int attempts = 0;
    void Start()
    {
        GenerateGrid();
        gridCollider = GetComponent<Collider>();
        gridCollider.providesContacts = true;
        gridCollider.bounds.Equals( new Vector3(_width, _height, 10));
        gridCollider.bounds.center.Equals( new Vector3(_width/2, 0,_height/2));
        gridCollider.bounds.size.Equals( new Vector3(_width, 0.5f, _height));
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
                Tile spawnedTile = Instantiate(_tilePrefab, transform.position + new Vector3(x*transform.localScale.x, 0, y*transform.localScale.x), Quaternion.Euler(90,0,0), transform).GetComponent<Tile>();
                //spawnedTile.transform.SetParent(pivotGrid.transform);
                spawnedTile.transform.localScale = Vector3.one;

                //non è corretto, i nomi non corrispondo alle posizioni
                spawnedTile.name = $"Tile {x} {y}";
                

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
    public void InsertShips(AShip ship)
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
                ship.transform.localScale = ship.transform.localScale * transform.localScale.x;
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


    #region Ricerca nel dizionario
    public Tile GetTileAtPosition(Vector2 position) 
    {
        return _tiles.ContainsKey(position) ? _tiles[position] : null;
    }
    //scritto da Copilots
    public Vector2 GetPositionFromTile(Tile tile) 
    {
        foreach (var kvp in _tiles) 
        {
            if (kvp.Value == tile) 
            {
                return kvp.Key;
            }
        }
        return Vector2.negativeInfinity;
    }

    #endregion
    public void MoveShip(Vector2 currentPosition, Vector2 newPosition, int entity)
    {

        GameObject tmpShip;
        Tile currentTile = _tiles[currentPosition];
        Tile newTile =  _tiles[newPosition];

        if(currentPosition == Vector2.negativeInfinity || newPosition == Vector2.negativeInfinity)
        {
            return;
        }

        if(!_tiles.ContainsKey(currentPosition) || !_tiles.ContainsKey(newPosition))
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        if(currentTile == null || newTile == null) 
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        tmpShip = currentTile.GetShip();
        newTile.SetType(currentTile.GetType(), entity);

        //qui ho rimosso un GetShip().gameObject (stefano)
        if(tmpShip != null)
        {
            newTile.SetShip(tmpShip.gameObject);
            currentTile.SetTypeEmpty();
            currentTile.SetShip(null);
        }
    }
    
    public void RemoveShip(AShip ship)
    {
        Vector2 position = ship.position;
        Tile tile = GetTileAtPosition(position);
        tile.SetTypeEmpty();
        tile.SetShip(null);
    }

    //Metodo per il drop della carta
    public void CardDrop(AbstractCard card)
    {
        //throw new NotImplementedException();
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
