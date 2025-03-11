using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float attackRange;
    [SerializeField] int movementRange;
    [SerializeField] int shipsToSelect;

    public List<String> shipNames=new List<String>();
    List<Ship> ships;
    public List<Ship>enemies=new List<Ship>();
    public List<Ship>allies=new List<Ship>();
    private List<Ship> allyAttackers=new List<Ship>();
    private List<Ship> movingAllies=new List<Ship>();
    private List<Ship> enemyAttackers=new List<Ship>();
    private List<Ship> movingEnemies=new List<Ship>();
    private static int allyCount = 1;
    
    GameObject shipPrefab;

    [Header("Events")]
    [SerializeField] private OnShipAttackEvent attackEvent;
    [SerializeField] private OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] private MessageSentEvent messageSentEvent;
    [SerializeField] private MessageReceivedEvent messageReceivedEvent;

    //TODO vedere dove inserire l'evento di distruzione nave

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        
        ships = new List<Ship>();
        shipNames.OrderBy(x => Random.value);
    }
    void Start()
    {
       foreach (string shipName in shipNames){
            InstantiateInMap(shipName);
        }
    }
    void Update()
    {
    }
    void InstantiateInMap(string shipName){
        //TODO: istanzia la nave nella mappa, di Beto

        Ship newShip=Instantiate(shipPrefab, transform.position, Quaternion.identity).GetComponent<Ship>();
        newShip.name=shipName;
        newShip.manager=this;
        ships.Add(newShip);    
        //Decide se la nave è alleata o nemica
        if(allyCount<ships.Count/2){
            newShip.SetFaction(0);
            allies.Add(newShip);
            allyCount++;
        }
        else{
            newShip.SetFaction(1);
            enemies.Add(newShip);
        }
        
        Debug.Log("Navi alleate: "+allies.Count);
        Debug.Log("Navi nemiche: "+enemies.Count);
    }
    public void ChooseShips(){

        //Seleziona le navi che possono attaccare e decidi tra loro chi attaccherà
        allyAttackers = allies.Where(x => x.LookForObjectives(enemies)==true).ToList();
        enemyAttackers = enemies.Where(x => x.LookForObjectives(allies)==true).ToList();
        List<Ship> movableEnemies = enemies.Where(x => x.LookForMovement(ships)==true).ToList();
        List<Ship> movableAllies = allies.Where(x => x.LookForMovement(ships)==true).ToList();
        
        int allyDecision, enemyDecision;
        if(allyAttackers.Count>1 && movableAllies.Count>1){
            allyDecision=Random.Range(0, 2);
        }
        else if(allyAttackers.Count>1 && movableAllies.Count==1){
            allyDecision=Random.Range(1, 2);
        }
        else if(allyAttackers.Count==1 && movableAllies.Count>1){
            allyDecision=Random.Range(0, 1);
        }
        else{
            allyDecision=0;
        }
        
        if(enemyAttackers.Count>1 && movableEnemies.Count>1){
            enemyDecision=Random.Range(0, 2);
        }
        else if(enemyAttackers.Count>1 && movableEnemies.Count==1){
            enemyDecision=Random.Range(1, 2);
        }
        else if(enemyAttackers.Count==1 && movableEnemies.Count>1){
            enemyDecision=Random.Range(0, 1);
        }
        else{
            enemyDecision=0;
        }

        switch(allyDecision){
            case 0:
                movingAllies=movableAllies.OrderBy(x=> Random.value).Take(2).ToList();
                movingAllies.ForEach(x => x.SetState(Ship.ShipState.Moving));
                allyAttackers.Clear();
                break;
            case 1:
                allyAttackers=allyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                allyAttackers[0].SetState(Ship.ShipState.Attacking);
                movableAllies.Remove(allyAttackers[0]);
                movingAllies=movableAllies.OrderBy(x=> Random.value).Take(1).ToList();
                movingAllies[0].SetState(Ship.ShipState.Moving);
                break;
            case 2:
                allyAttackers=allyAttackers.OrderBy(x=> Random.value).Take(2).ToList();
                allyAttackers.ForEach(x => x.SetState(Ship.ShipState.Attacking));
                movableAllies.Clear();
                movingAllies.Clear();
                break;
            default:
                break;
        }

        switch(enemyDecision){
            case 0:
                //Non ci sono nemici che possono attaccare, scelgo solo mosse di movimento 
                movingEnemies=movableEnemies.OrderBy(x=> Random.value).Take(2).ToList();
                movingEnemies.ForEach(x => x.SetState(Ship.ShipState.Moving));
                enemyAttackers.Clear();
                break;
            case 1:
                //Ci sono abbastanza nemici per scegliere un attacco e un movimento
                enemyAttackers=enemyAttackers.OrderBy(x=> Random.value).Take(1).ToList();
                enemyAttackers[0].SetState(Ship.ShipState.Attacking);
                movableEnemies.Remove(enemyAttackers[0]);
                movingEnemies=movableEnemies.OrderBy(x=> Random.value).Take(1).ToList();
                movingEnemies[0].SetState(Ship.ShipState.Moving);
                break;
            case 2:
                //Non ci sono nemici che possono muoversi, scelgo solo attacchi
                enemyAttackers=enemyAttackers.OrderBy(x=> Random.value).Take(2).ToList();
                enemyAttackers.ForEach(x => x.SetState(Ship.ShipState.Attacking));
                movableEnemies.Clear();
                movingEnemies.Clear();
                break;
            default:
                break;
        }

        movingAllies.Concat(allyAttackers);
        movingEnemies.Concat(enemyAttackers);
    }
    void SendMessages(){
        movingAllies.ForEach(x => x.SendMessage());
        movingEnemies.ForEach(x => x.SendMessage());
        //TODO riempire la struct per l'invio del messaggio
        //messageSentEvent.Invoke(new MessageStruct());
    }
    void CallAttack()
    {
        //TODO riempire la struct per l'invio dell'attacco con le cooridaate del bersaglio
        //attackEvent.Invoke(new ShipAttackStruct());
    }
    // Update is called once per frame
    
    
    void OrderMovement(Ship ship){
        //MoveShip(ship);
    }

    public void RemoveShip(Ship ship, int isAlly){
        if(isAlly==0){
            allies.Remove(ship);
        }
        else{
            enemies.Remove(ship);
        }
        ships.Remove(ship);
    }
}