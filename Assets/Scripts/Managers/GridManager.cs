using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour, ICardDropArea
{

    private enum ObstacleMode
    {
        randomFromList,
    }

    private enum GenerationMode
    {
        random,
        layout,
    }

    [Header("Grid Parameters")]
    public int _width;
    public int _height;
    [SerializeField] private GameObject _tilePrefab;

    [Header("Obstacle Options")]
    [SerializeField] private bool canGenerateObstacles = false;
    [SerializeField] private ObstacleMode obstacleMode;
    [SerializeField] private int numberOfObstacles;

    [Header("Obstacle List")]
    [SerializeField] private List<GameObject> obstaclePrefabsList = new List<GameObject>();

    [Header("Generation Mode")]
    [SerializeField] private GenerationMode generationMode = GenerationMode.random;
    [Tooltip("If you want to use a specific layout, set the generation mode to layout and assign the grid layout scriptable object.")]
    [SerializeField] private GridLayoutScript gridLayout;


    public Dictionary<Vector2, Tile> _tiles;

    private Collider gridCollider;

    //mi serve a tenere traccia del numero di tentativi per il riposizionamento
    int attempts = 0;

    #region Singleton
    private static GridManager instance;
    public static GridManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetUpInstance();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private static void SetUpInstance()
    {
        instance = FindAnyObjectByType<GridManager>();
        if (instance == null)
        {
            GameObject gridManagerObject = new GameObject();
            gridManagerObject.name = "GridManager";
            instance = gridManagerObject.AddComponent<GridManager>();
            DontDestroyOnLoad(gridManagerObject);
        }
    }

    #endregion


    void Start()
    {
        GenerateGrid();
        gridCollider = GetComponent<Collider>();
        gridCollider.providesContacts = true;
        gridCollider.bounds.Equals( new Vector3(_width, _height, 10));
        gridCollider.bounds.center.Equals( new Vector3(_width/2, 0,_height/2));
        gridCollider.bounds.size.Equals( new Vector3(_width, 0.5f, _height));
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        switch ((int)generationMode)
        {
            case (int)GenerationMode.random:
                GenerateRandomGrid();
                break;

            case (int)GenerationMode.layout:
                GenerateLayoutGrid(gridLayout);
                break;
            default:
                Debug.LogError("Invalid Generation Mode");
                break;
        }


        if (canGenerateObstacles)
                {
                    switch ((int)obstacleMode)
                    {
                        case (int)ObstacleMode.randomFromList:

                            for (int i = 0; i < numberOfObstacles; i++)
                            {
                                int listIndex = Random.Range(0, obstaclePrefabsList.Count - 1);
                                Vector2 randomTilePos = new Vector2(Random.Range(0, _width - 1), Random.Range(0, _height - 1));

                                while (!IsValidPosition(randomTilePos))
                                {
                                    randomTilePos = new Vector2(Random.Range(0, _width - 1), Random.Range(0, _height - 1));
                                }

                                Tile randomTile = _tiles[randomTilePos];
                                randomTile.InstantiateObstacle(obstaclePrefabsList[listIndex], randomTile);
                            }
                            break;

                        default:

                            break;

                    }
                }
    }

    private void GenerateRandomGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                //int tileIndex = Random.Range(0, );
                //Nell'istanziare, prende lo script Tile attaccato all'oggetto creato
                Tile spawnedTile = Instantiate(_tilePrefab, transform.position + new Vector3(x * transform.localScale.x, 0, y * transform.localScale.x), Quaternion.Euler(90, 0, 0), transform).GetComponent<Tile>();
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

    private void GenerateLayoutGrid(GridLayoutScript gridLayout)
    {
        if (gridLayout == null)
        {
            Debug.LogError("Grid Layout is not assigned!");
            return;
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile spawnedTile = Instantiate(gridLayout.grid[x].values[y], transform.position + new Vector3(x * transform.localScale.x, 0, y * transform.localScale.x), Quaternion.Euler(90, 0, 0), transform).GetComponent<Tile>();
                //spawnedTile.transform.SetParent(pivotGrid.transform);
                spawnedTile.transform.localScale = Vector3.one;

                //non è corretto, i nomi non corrispondo alle posizioni
                spawnedTile.name = $"Tile {Mathf.Abs(y - 4)} {x}";

                _tiles[new Vector2(x, y)] = spawnedTile;

            }
        }

    }
    public bool IsValidPosition(Vector2 position)
    {
        //Debug.Log(position + "è valida: "+_tiles.ContainsKey(position));
        if (_tiles.ContainsKey(position) && _tiles[position].GetType() == TileType.Empty)
        {
            return true;
        }
        return false;
    }
    public void InsertShips(AShip ship)
    {
        while(true)
        {
            Vector2Int position = new Vector2Int(Random.Range(0, _width), Random.Range(0, _height));
            if(IsValidPosition(position) && attempts < (_width * _height))
            {
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

    public Vector2 GetAllyShipPosition()
    {
        foreach (var tile in _tiles.Values)
        {
            if (tile.GetShip() != null && tile.GetShip().GetComponent<AShip>().faction == (int)Entity.ally)
            {
                return GetPositionFromTile(tile);
            }
        }
        return Vector2.negativeInfinity; // Se non trovata, ritorna un valore negativo
    }

    #endregion
    public void MoveShip(Vector2 currentPosition, Vector2 newPosition, int entity)
    {

        GameObject tmpShip;
        Tile currentTile = _tiles[currentPosition];
        Tile newTile = _tiles[newPosition];

        if (currentPosition == Vector2.negativeInfinity || newPosition == Vector2.negativeInfinity)
        {
            return;
        }

        if (!_tiles.ContainsKey(currentPosition) || !_tiles.ContainsKey(newPosition))
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        if (currentTile == null || newTile == null)
        {
            Debug.Log("La nave non può muoversi in questa posizione");
            return;
        }

        tmpShip = currentTile.GetShip();
        newTile.SetType(currentTile.GetType(), entity);

        //qui ho rimosso un GetShip().gameObject (stefano)
        if (tmpShip != null)
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

    #region Gestione Ostacoli



    #endregion

    //Metodo per il drop della carta
    public void CardDrop(AbstractCard card)
    {
        //throw new NotImplementedException();
    }

}
