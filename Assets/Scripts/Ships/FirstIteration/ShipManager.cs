using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager : MonoBehaviour, IShipManager
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

    //le ho messe generiche, perchè ereditano da AShip

    //
    /*public List<NewShip>enemies=new List<NewShip>();
    public List<NewShip>allies=new List<NewShip>();*/

    public List<AShip>enemies=new List<AShip>();
    public List<AShip>allies=new List<AShip>();
    //public List<AShip> allyAttackers=new List<AShip>();
    //public List<AShip> activeAllies=new List<AShip>();
    //public List<AShip> enemyAttackers=new List<AShip>();
    //public List<AShip> activeEnemies=new List<AShip>();
    private static int allyCount = 0;
    private static int enemyCount = 0;
    

    //questo forse si può togliere se lo associamo ad ogni nave, cioè possiamo spostare il metodo che istanzia il prefab
    [SerializeField] private GameObject shipPrefab;
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
        
        ships = new Dictionary<string, AShip>();
        shipManagerSO.RandomizeShips();
        //TODO rivedere questa cosa
        gridManager = FindFirstObjectByType<GridManager>();
        influenceMap = new InfluenceMap(gridManager._width, gridManager._height, shipManagerSO.influenceDecay, shipManagerSO.decayMomentum);
        //shipChoice = AssetBundle.LoadFromFile("Assets/AssetBundle/shipchoices").LoadAllAssets<ShipSO>().ToList();
    }
    
    private void Start()
    {
        StartCoroutine(ShipGeneration());
    }
    public int NumberOfMessages
    {
        get { return numberOfMessages; }
    }
    public InfluenceMap InfluenceMap
    {
        get { return influenceMap; }
    }
    public void InstantiateAllyShip(string shipName, int faction){
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
            AShip newShip=Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90,0,0)).GetComponent<AShip>();
            newShip.shipName=shipName;
            newShip.name=shipName;
            //newShip.manager=this;
            ships.Add(newShip.shipName, newShip);
            newShip.SetFaction(0);
            newShip.shipSO = shipManagerSO.shipSOarray[modelIndex];
            newShip.adversaryShipLayer=LayerMask.GetMask("Enemy");
            newShip.shipInfluence=1;
            allies.Add(newShip);
            allyCount++;
            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(allyMaterial);
            gridManager.InsertShips(newShip);
            influenceMap.RegisterPropagator(newShip);
    }
    public void InstantiateEnemyShip(string shipName, int faction){
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
            AShip newShip=Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90,0,0)).GetComponent<AShip>();
            newShip.shipName=shipName;
            newShip.name=shipName;
            //newShip.manager=this;
            ships.Add(newShip.shipName, newShip);
            newShip.SetFaction(1);
            newShip.shipSO = shipManagerSO.shipSOarray[modelIndex];
            newShip.adversaryShipLayer=LayerMask.GetMask("Ally");
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
            InstantiateAllyShip(shipName, (int)Entity.ally);
            return;
        }
        if(enemyCount<shipManagerSO.enemyShips)
        {
            InstantiateEnemyShip(shipName, (int)Entity.enemy);
            return;
        }
    }

    public void ChooseShips()
    {
        foreach(AShip AShip in allies)
        {
            if(AShip is NewShip)
            {
                AShip.LookForMovement();
                //AShip.LookForAttacks(enemies);
            }

        }
        foreach(AShip ship in enemies)
        {
            if(ship is NewShip)
            {
                ship.LookForMovement();
                //ship.LookForAttacks(allies);
            }
            ship.LookForMovement();
            //ship.LookForAttacks(allies);
        }

        //Seleziona le navi che possono attaccare e decidi tra loro chi attaccherà
        allyMoves = allies.SelectMany(x=> x.shipMoves).OrderBy(x=>Random.value).ToList();
        enemyMoves = enemies.SelectMany(x => x.shipMoves).OrderBy(x=> Random.value).ToList();

        Debug.Log("Ally moves: " + allyMoves.Count());
        Debug.Log("Enemy moves: " + enemyMoves.Count());
        

        SendMessages();
    }
    public IEnumerator EndEnemyTurn(){
        yield return 1;
    }
    public void EnemyMovesSelection(){

    }
    public void EnemyMovesExecution(){

    }
    void SendMessages()
    {
        if(allyMoves.Count()<numberOfMessages)
        {
            foreach(Move move in allyMoves)
            {
                ships[move.GetShipName()].SendMessage(move);
            }
        }
        else{
            allyMoves=allyMoves.OrderBy(x=>Random.value).Take(numberOfMessages).ToList();
            foreach(Move move in allyMoves){
                ships[move.GetShipName()].SendMessage(move);
            }
        }

        if(enemyMoves.Count() < numberOfMessages){
            foreach(Move move in enemyMoves){
            ships[move.GetShipName()].SendMessage(move);
            }
        }
        else{
            enemyMoves = enemyMoves.OrderBy(x=>Random.value).Take(numberOfMessages).ToList();
            foreach(Move move in enemyMoves){
            ships[move.GetShipName()].SendMessage(move);
            }
        }

        //activeAllies.ForEach(x => x.SendMessage());
        //activeEnemies.ForEach(x => x.SendMessage());
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
        //shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(AShip.shipName, AShip.faction, AShip.position));
        Destroy(shipDestroyedStruct.shipScript.gameObject);
        influenceMap.Propagate();
    }
    private IEnumerator ShipGeneration(){
        yield return new WaitForSeconds(1);
        foreach (string AShip in shipManagerSO.startingShips)
        {
            InstantiateInMap(AShip);
        }
        influenceMap.Propagate();
    }
}