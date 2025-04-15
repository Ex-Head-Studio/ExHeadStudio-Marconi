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
            newShip.manager=this;
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
            newShip.manager=this;
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
                AShip.LookForAttacks(enemies);
            }

        }
        foreach(AShip ship in enemies)
        {
            if(ship is NewShip)
            {
                ship.LookForMovement();
                ship.LookForAttacks(allies);
            }
            ship.LookForMovement();
            ship.LookForAttacks(allies);
        }

        //Seleziona le navi che possono attaccare e decidi tra loro chi attaccherà
        allyMoves = allies.SelectMany(x=> x.shipMoves).OrderBy(x=>Random.value).ToList();
        enemyMoves = enemies.SelectMany(x => x.shipMoves).OrderBy(x=> Random.value).ToList();

        Debug.Log("Ally moves: " + allyMoves.Count());
        Debug.Log("Enemy moves: " + enemyMoves.Count());
        /*
        foreach(AShip AShip in allyAttackers)
        {
            Debug.Log("Allyatt: " + AShip.shipName);
        }*/
  

        

        /*
        enemyAttackers = enemies.Where(x => x.LookForObjectives(allies)).ToList();
        List<AShip> movableEnemies = enemies.Where(x => x.LookForMovement()).ToList();
        Debug.Log("Numero nemici: "+ enemyAttackers.Count());

        //Una volta che le navi sono state selezionate, si decide cosa far fare a una fazione a seconda di quante navi ha a disposizione
        //per attaccare e per muoversi
        // 0 = solo movimento, 1 = movimento e attacco, 2 = solo attacco

        // Verificare che poi nella lista delle navi che eseguiranno qualcosa ci sia il numero giusto di navi
        
        int allyDecision, enemyDecision;
        Debug.Log(allyAttackers.Count);
        Debug.Log(movableAllies.Count);
        if(allyAttackers.Count>1 && movableAllies.Count>1){
            
            allyDecision=Random.Range(0, 3);

        }
        else if(allyAttackers.Count>1 && movableAllies.Count==1){
            allyDecision=Random.Range(1, 3);
        }
        else if(allyAttackers.Count==1 && movableAllies.Count>1){
            allyDecision=Random.Range(0, 2);
        }
        else
        {
            allyDecision=0;
        }
        
        if(enemyAttackers.Count>1 && movableEnemies.Count>1){
            
            enemyDecision=Random.Range(0, 3);
        }
        else if(enemyAttackers.Count>1 && movableEnemies.Count==1){
            enemyDecision=Random.Range(1, 3);
        }
        else if(enemyAttackers.Count==1 && movableEnemies.Count>1){
            enemyDecision=Random.Range(0, 2);
        }
        else{
            enemyDecision=0;
        }
 

        switch(allyDecision){
            case 0:
                movableAllies=movableAllies.OrderBy(x=> Random.value).ToList();
                
                //activeAllies.ForEach(x => x.SetState(AShip.ShipState.Moving));
                allyAttackers.Clear();
                break;
                
            case 1:
                activeAllies=allyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                activeAllies[0].SetState(AShip.ShipState.Attacking);
                movableAllies.Remove(activeAllies[0]);
                activeAllies.Add(movableAllies.OrderBy(x=> Random.value).Take(1).ToList()[0]);
                activeAllies[1].SetState(AShip.ShipState.Moving);
                break;
            case 2:
                if(allyAttackers.Count>1){
                    activeAllies=allyAttackers.OrderBy(x=> Random.value).Take(2).ToList();
                }
                else{
                    activeAllies=allyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                }
                activeAllies.ForEach(x => x.SetState(AShip.ShipState.Attacking));
                movableAllies.Clear();
                
                break;
            default:
                break;
        }

        switch(enemyDecision){
            case 0:
                //Non ci sono nemici che possono attaccare, scelgo solo mosse di movimento 
                if(movableEnemies.Count>1){
                    activeEnemies=movableEnemies.OrderBy(x=> Random.value).Take(2).ToList();
                }
                else{
                    activeEnemies=movableEnemies.OrderBy(x=> Random.value).Take(1).ToList();
                }
                activeEnemies.ForEach(x => x.SetState(AShip.ShipState.Moving));
                enemyAttackers.Clear();
                break;
            case 1:
                //Ci sono abbastanza nemici per scegliere un attacco e un movimento
                activeEnemies = enemyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                activeEnemies[0].SetState(AShip.ShipState.Attacking);
                movableEnemies.Remove(activeEnemies[0]);
                activeEnemies.Add(movableEnemies.OrderBy(x=> Random.value).Take(1).ToList()[0]);
                activeEnemies[1].SetState(AShip.ShipState.Moving);
                break;
            case 2:
                //Non ci sono nemici che possono muoversi, scelgo solo attacchi
                if(enemyAttackers.Count>1){
                    activeEnemies=enemyAttackers.OrderBy(x=> Random.value).Take(2).ToList();
                }
                else{
                    activeEnemies=enemyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                }
                activeEnemies.ForEach(x => x.SetState(AShip.ShipState.Attacking));
                //movableEnemies.Clear();
                movableEnemies.Clear();
                break;
            default:
                break;
        }*/
        //le liste moving contengono le navi che proporranno un movimento o un attacco al giocatore

    //        Debug.Log("Nemici che fanno cose:" + activeEnemies.Count);
    //      Debug.Log("Alleati che fanno cose: "+ activeAllies.Count);

        SendMessages();
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