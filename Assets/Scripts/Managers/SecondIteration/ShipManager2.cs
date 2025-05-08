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
    [SerializeField] private float timeBeforeEndEnemyTurn;
    private Dictionary<string, AShip> shipsD;

    //per vedere le liste in inspector, usare la modalità di debug

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
        //Togliere da start e creare l'evento nuovo di inizio gioco
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
    /// 
    

    //Possiamo invocare questo metodo da parte di una carta supporto
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
    /// <param name="shipName"></param>
    /// </summary>

    public void InstantiateEnemyShip(string shipName, int faction)
        {
            AShip newShip= SetupShip(shipName, faction);
            shipsD.Add(newShip.shipName, newShip);
            enemies.Add(newShip);
            enemyCount++;
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
        }



    //La funzione viene chiamata per dire alle navi di calcolare le loro azioni, fatto questo, le navi poi potranno eseguire la loro azione migliore
    //una volta che il turno verrà effettuato.
    public void EnemyMovesSelection()
    {
        Debug.Log("Ricerca mosse del nemico");
        foreach(AShip enemy in enemies)
        {
            enemy.LookForMovement();
            enemy.LookForAttacks();
        }
        
    }
    //Il metodo viene chiamato dall'evento di fine turno giocatore e fa eseguire alle navi la loro mossa preferita
    public void EnemyMovesExecution(){
        Debug.Log("Esecuzione turno nemico");
        for(int i=0, j=0 ;j<shipManagerSO.initialEnemyShips; i++, j++){
            if(j>=enemies.Count){
                i=0;
            }
            if(!enemies[i].moveDone)
                enemies[i].ExecuteMove();
            else {
                enemies[i].LookForMovement();
                enemies[i].LookForAttacks();
                enemies[i].ExecuteMove();
            }
        }
        StartCoroutine(EndEnemyTurn());
    }

    public IEnumerator EndEnemyTurn(){
        yield return new WaitForSeconds(timeBeforeEndEnemyTurn);
        shipManagerSO.onEndEnemyTurn.Invoke(new VoidEvent(0));
        Debug.Log("Fine turno nemico");
    }

    //Questa serve ancora?
    //Finito il turno, aggiorna la influence map con le nuove posizioni delle navi, così i calcoli nei turni successivi sono corretti
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
        if(shipsD.ContainsKey(shipDestroyedStruct.shipName)){
            if(shipDestroyedStruct.entity == (int)Entity.ally )
            {
                allies.Remove(shipsD[shipDestroyedStruct.shipName]);
                allyCount--;
            }
            else if(shipDestroyedStruct.entity == (int)Entity.enemy && shipsD.ContainsKey(shipDestroyedStruct.shipName))
            {
                enemies.Remove(shipsD[shipDestroyedStruct.shipName]);
                enemyCount--; 
            }
        
            
            influenceMap.UnregisterPropagator(shipDestroyedStruct.shipScript);
            influenceMap.Propagate();
            gridManager.RemoveShip(shipDestroyedStruct.shipScript);
        
            shipsD.Remove(shipDestroyedStruct.shipName);
            Destroy(shipDestroyedStruct.shipScript.gameObject);
        }
    }


    private AShip SetupShip(string shipName, int faction)
    {

        //Aggiunto per istanziare sempre la classe base se ho solo una nave alleata
        if(faction == (int)Entity.ally && shipManagerSO.allyShips == 1)
        {
            modelIndex = shipManagerSO.shipSOarray.Count - 1;
        }
        else
        {
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
        }

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
            //assegno ad ogni nuova nave i component per reagire alle carte
            newShip.AddComponent<CardAllyShip>();
            newShip.AddComponent<AllyShip>();
            
            AllyShip shipScript = newShip.GetComponent<AllyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);
            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.allyMaterial);
            return shipScript;
        }
        else
        {

            newShip.AddComponent<EnemyShip>();
            EnemyShip shipScript = newShip.GetComponent<EnemyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);
            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.enemyMaterial);
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