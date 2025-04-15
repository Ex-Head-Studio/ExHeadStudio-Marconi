using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager2 : MonoBehaviour, IShipManager
{
    
    [Header("Parameters")]
    [SerializeField] private ShipManagerSO shipManagerSO;
    Dictionary<string, AShip> ships;
    [Tooltip("Numero di messaggi massimo per fazione per turno")] public int numberOfMessages;

    [Header("Lists for debug, don't touch")]
    //le ho messe tutte pubbliche altimenti non posso fare debug
    public List<Move> allyMoves;
    public List<Move> enemyMoves;
    public InfluenceMap influenceMap;

    //le ho messe generiche, perchè ereditano da ship

    //
    /*public List<NewShip>enemies=new List<NewShip>();
    public List<NewShip>allies=new List<NewShip>();*/

    public List<AShip>enemies=new List<AShip>();
    public List<AShip>allies=new List<AShip>();
    //public List<Ship> allyAttackers=new List<Ship>();
    //public List<Ship> activeAllies=new List<Ship>();
    //public List<Ship> enemyAttackers=new List<Ship>();
    //public List<Ship> activeEnemies=new List<Ship>();
    private static int allyCount = 0;
    private static int enemyCount = 0;
    

    //questo forse si può togliere se lo associamo ad ogni nave, cioè possiamo spostare il metodo che istanzia il prefab
    [SerializeField] private GameObject allyPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Material allyMaterial;
    [SerializeField] private Material enemyMaterial;
     

    [Header("Events")]
    [SerializeField] private OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] private MessageSentEvent messageSentEvent;
    [SerializeField] private MessageReceivedEvent messageReceivedEvent;



    //TODO trovare il modo di referenziare correttamente il grid manager, qui è fatto veloce
    private GridManager gridManager;

    private int modelIndex;
    
    void Awake()
    {

        //Attacca i componenti di ship manager a ship manager

        ships = new Dictionary<string, AShip>();
        shipManagerSO.RandomizeShips();
        //TODO rivedere questa cosa
        gridManager = FindFirstObjectByType<GridManager>();
        influenceMap = new InfluenceMap(gridManager._width, gridManager._height, shipManagerSO.influenceDecay, shipManagerSO.decayMomentum);
        numberOfMessages = shipManagerSO.numberOfMessages;
        //shipChoice = AssetBundle.LoadFromFile("Assets/AssetBundle/shipchoices").LoadAllAssets<ShipSO>().ToList();
    }

    
    
    private void Start()
    {
        StartCoroutine(ShipGeneration());
    }

    public int NumberOfMessages{
        get { return numberOfMessages; }
    }
    public InfluenceMap InfluenceMap{
        get { return influenceMap; }
    }

    //Istanzia una nave alleata e inseriscila nel dizionario delle navi
    public void InstantiateAllyShip(string shipName){
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
            AShip newShip=Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90,0,0)).GetComponent<AShip>();
            newShip.shipName=shipName;
            newShip.name=shipName;
            newShip.manager=this;
            ships.Add(newShip.shipName, newShip);
            newShip.SetFaction(0);
            newShip.shipSO = shipManagerSO.shipSOarray[modelIndex];
            newShip.shipInfluence=1;
            allies.Add(newShip);
            allyCount++;
            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(allyMaterial);
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
    }

        //Istanzia una nave nemica e inseriscila nel dizionario delle navi
        public void InstantiateEnemyShip(string shipName){
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
            AShip newShip=Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90,0,0)).GetComponent<AShip>();
            newShip.shipName=shipName;
            newShip.name=shipName;
            newShip.manager=this;
            ships.Add(newShip.shipName, newShip);
            newShip.SetFaction(1);
            newShip.shipSO = shipManagerSO.shipSOarray[modelIndex];
            newShip.shipInfluence=-1;
            enemies.Add(newShip);
            enemyCount++;
            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(enemyMaterial);
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
    }
    public void InstantiateInMap(string shipName)
    {
        if(shipManagerSO.allyShips+shipManagerSO.enemyShips>shipManagerSO.startingShips.Count)
        {
            Debug.LogError("Not enough ships for the number of allies and enemies");
            return;
        }

        if(allyCount<shipManagerSO.allyShips)
        {
            InstantiateAllyShip(shipName);
            return;
        }
        if(enemyCount<shipManagerSO.enemyShips){
            InstantiateEnemyShip(shipName);
            return;
        }
    }

    public void ChooseShips()
    {
        enemies.ForEach(e => {e.LookForAttacks(); e.LookForMovement(); });
        
    }

    public void EndTurn(){
        influenceMap.Propagate();
    }
    
    //cosa fa questo metodo? Da chi toglie cosa?
    public void RemoveShip(ShipDestroyedStruct shipDestroyedStruct)
    {
        if(shipDestroyedStruct.entity == (int)Entity.ally)
        {
            allies.Remove(ships[shipDestroyedStruct.shipName]);
        }
        else{
            enemies.Remove(ships[shipDestroyedStruct.shipName]);
        }
    
        ships.Remove(shipDestroyedStruct.shipName);
        influenceMap.UnregisterPropagator(shipDestroyedStruct.shipScript);
        //shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(ship.shipName, ship.faction, ship.position));
        Destroy(shipDestroyedStruct.shipScript.gameObject);
        influenceMap.Propagate();
    }
    private IEnumerator ShipGeneration(){
        yield return new WaitForSeconds(1);
        foreach (string ship in shipManagerSO.startingShips)
        {
            InstantiateInMap(ship);
        }
        influenceMap.Propagate();
    }
}