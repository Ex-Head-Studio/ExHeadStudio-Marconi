using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager2 : MonoBehaviour, IShipManager
{
    
    [Header("Parameters")]
    [SerializeField] private ShipManagerSO shipManagerSO;
    [SerializeField] private float timeBeforeGeneration = 1f;
    
    private Dictionary<string, AShip> shipsD;

    //per vedere le liste in inspector, usare la modalità di debug

    private List<Move> allyMoves;
    private List<Move> enemyMoves;
    public InfluenceMap influenceMap;


    private List<AShip>enemies=new List<AShip>();
    private List<AShip>allies=new List<AShip>();

    private static int allyCount = 0;
    private static int enemyCount = 0;

    //TODO sistemare correttamente questo singleton 
    private GridManager gridManager;

    private int modelIndex;
    
    //Attacca i componenti di ship manager a ship manager
    void Awake()
    {
        shipsD = new Dictionary<string, AShip>();

        //TODO rivedere questa cosa, la reference al singleton va fatta in modo diverso
        gridManager = FindFirstObjectByType<GridManager>();

        shipManagerSO.RandomizeShips();

        influenceMap = new InfluenceMap(gridManager._width, gridManager._height, shipManagerSO.influenceDecay, shipManagerSO.decayMomentum);
    }

    
    
    private void Start()
    {
        StartCoroutine(ShipGeneration());
    }

    private IEnumerator ShipGeneration()
    {
        yield return new WaitForSeconds(timeBeforeGeneration);
        foreach (string ship in shipManagerSO.startingShips)
        {
            InstantiateInMap(ship);
        }
        influenceMap.Propagate();
    }

    public void InstantiateInMap(string shipName)
    {
        if(shipManagerSO.allyShips+shipManagerSO.enemyShips>shipManagerSO.startingShips.Count)
        {
            Debug.LogError("Not enough shipsD for the number of allies and enemies");
            return;
        }

        if(allyCount<shipManagerSO.allyShips)
        {
            InstantiateAllyShip(shipName, (int)Entity.ally);
            return;
        }
        if(enemyCount<shipManagerSO.enemyShips){
            InstantiateEnemyShip(shipName, (int)Entity.enemy);
            return;
        }
    }

    /// <summary>
    ///  Instantiates an ally ship and adds it to the dictionary of shipsD.
    /// </summary>
    /// <param name="shipName"></param>
    public void InstantiateAllyShip(string shipName, int faction)
    {
        AShip newShip = SetupShip(shipName, faction);
            shipsD.Add(newShip.shipName, newShip);
            allies.Add(newShip);
            allyCount++;
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
    }

    /// <summary>
    ///  Instantiates an anemy ship and adds it to the dictionary of shipsD.
    /// </summary>
    /// <param name="shipName"></param>
    /// 
    public void InstantiateEnemyShip(string shipName, int faction)
        {
            AShip newShip= SetupShip(shipName, faction);
            shipsD.Add(newShip.shipName, newShip);
            enemies.Add(newShip);
            enemyCount++;
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
        }



    //TODO commentare questa funzione, non è chiaro il suo scopo; Viene chiamata in risposta all'evento di inizio turno
    public void ChooseShips()
    {
        enemies.ForEach(e => {e.LookForAttacks(); e.LookForMovement(); });   
    }

    //TODO commentare questa funzione, non è chiaro il suo scopo
    public void EndTurn()
    {
        influenceMap.Propagate();
    }
    

    /// <summary>
    /// Callback functiont which removes the ship from the map, the dictionary of shipsD and the infuence map.
    /// It also destroys the ship game object.
    /// <param name="shipDestroyedStruct"> The Struct passed by the event channel</param> 
    /// </summary>
    public void RemoveShip(ShipDestroyedStruct shipDestroyedStruct)
    {
        if(shipDestroyedStruct.entity == (int)Entity.ally)
        {
            allies.Remove(shipsD[shipDestroyedStruct.shipName]);
        }
        else
        {
            enemies.Remove(shipsD[shipDestroyedStruct.shipName]);
        }

        influenceMap.UnregisterPropagator(shipDestroyedStruct.shipScript);
        influenceMap.Propagate();
    
        shipsD.Remove(shipDestroyedStruct.shipName);
        Destroy(shipDestroyedStruct.shipScript.gameObject);
    }


    private AShip SetupShip(string shipName, int faction)
    {
        modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
        GameObject newShip = Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90, 0, 0));

        Animator childAnim = newShip.gameObject.GetComponentInChildren<Animator>();
        childAnim.runtimeAnimatorController = shipManagerSO.shipSOarray[modelIndex].shipAnimatorController;

        Instantiate(shipManagerSO.shipSOarray[modelIndex].shipClassModel, childAnim.gameObject.transform, false);

        //tolgo il component NewShip
        if(newShip.TryGetComponent<NewShip>(out NewShip oldScript))
        {
            Destroy(oldScript);
        }

        if(faction == (int)Entity.ally)
        {

            newShip.AddComponent<AllyShip>();

            //assegno ad ogni nuova nave i component per reagire alle carte
            //newShip.AddComponent<CardAllyShip>();
            
            AllyShip shipScript = newShip.GetComponent<AllyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);
            //shipScript.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.allyMaterial);


            return shipScript;
        }
        else
        {

            newShip.AddComponent<EnemyShip>();
            EnemyShip shipScript = newShip.GetComponent<EnemyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);
            //shipScript.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.enemyMaterial);
            return shipScript;
        }
    }


    public int NumberOfMessages{
        get { return shipManagerSO.numberOfMessages; }
    }
    public InfluenceMap InfluenceMap{
        get { return influenceMap; }
    }
}