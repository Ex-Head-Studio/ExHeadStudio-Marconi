using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager : MonoBehaviour
{
    [Header("Parameters")]
    public List<String> shipNames=new List<String>();
    List<Ship> ships;

    [Header("Lists for debug, don't touch")]
    //le ho messe tutte pubbliche altimenti non posso fare debug
    public List<Ship>enemies=new List<Ship>();
    public List<Ship>allies=new List<Ship>();
    public List<Ship> allyAttackers=new List<Ship>();
    public List<Ship> movingAllies=new List<Ship>();
    public List<Ship> enemyAttackers=new List<Ship>();
    public List<Ship> movingEnemies=new List<Ship>();
    private static int allyCount = 1;
    private static int enemyCount = 1;
    
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private Material allyMaterial;
    [SerializeField] private Material enemyMaterial;

    [Header("Events")]
    //[SerializeField] private OnShipAttackEvent attackEvent;
    [SerializeField] private OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] private MessageSentEvent messageSentEvent;
    [SerializeField] private MessageReceivedEvent messageReceivedEvent;

    //TODO bisogna linkare il numero di navi con la UI (Stefano), verifica se serve anche in funzione delle luci
    //questa cosa va fatta usando un SO per i contatori


    //TODO trovare il modo di referenziare correttamente il grid manager, qui è fatto veloce
    private GridManager gridManager;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        ships = new List<Ship>();
        shipNames.OrderBy(x => Random.value);
        //TODO rivedere questa cosa
        gridManager = FindFirstObjectByType<GridManager>();

    }
    private void Start()
    {
        foreach (string shipName in shipNames)
        {
            InstantiateInMap(shipName);
        }

    }
    private void Update()
    {
        //non è più necessario
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChooseShips();
        }
    }

    void InstantiateInMap(string shipName)
    {
        

        Ship newShip=Instantiate(shipPrefab, transform.position, Quaternion.identity).GetComponent<Ship>();
        newShip.shipName=shipName;
        newShip.name=shipName;
        newShip.manager=this;
        ships.Add(newShip);    
        //Decide se la nave è alleata o nemica
        if(allyCount<=ships.Count/2){
            newShip.SetFaction(0);
            allies.Add(newShip);
            allyCount++;
            newShip.GetComponent<MeshRenderer>().material=allyMaterial;
        }
        else{
            newShip.SetFaction(1);
            enemies.Add(newShip);
            enemyCount++;
            newShip.GetComponent<MeshRenderer>().material=enemyMaterial;
        }
        
        //chiamata al metodo che piazza la nave nella griglia
        gridManager.InsertShips(newShip);
    }

    //questa funzione va rivista perchè non viene mai chiamata, la collego all'evento di inzio turno
    //BUG le liste non vengono aggiornate, è il primo problema da risolvere

    //TODO debug passo passo
    //TODO non compaiono attacchi
    public void ChooseShips()
    {

        //TODO le lamba non vengono chiamate, con l'invocazione tramite evento!!!!!
        //Seleziona le navi che possono attaccare e decidi tra loro chi attaccherà
        allyAttackers = allies.Where(x => x.LookForObjectives(enemies)==true).ToList();
        /*Debug.Log("Numero: " + allyAttackers.Count());
        foreach(Ship ship in allyAttackers)
        {
            Debug.Log("Allyatt: " + ship.shipName);
        }*/
        enemyAttackers = enemies.Where(x => x.LookForObjectives(allies)==true).ToList();
        List<Ship> movableEnemies = enemies.Where(x => x.LookForMovement()==true).ToList();
        List<Ship> movableAllies = allies.Where(x => x.LookForMovement()==true).ToList();

        //Una volta che le navi sono state selezionate, si decide cosa far fare a una fazione a seconda di quante navi ha a disposizione
        //per attaccare e per muoversi
        // 0 = solo movimento, 1 = movimento e attacco, 2 = solo attacco

        // Verificare che poi nella lista delle navi che eseguiranno qualcosa ci sia il numero giusto di navi
        //TODO correggere l'if a zero
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
        else
        {
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
        //le liste moving contengono le navi che proporranno un movimento o un attacco al giocatore
        movingAllies.Concat(allyAttackers);
        movingEnemies.Concat(enemyAttackers);

        Debug.Log("Nemici che fanno cose:" + movingEnemies.Count);
        Debug.Log("Alleati che fanno cose: "+ movingAllies.Count);

        SendMessages();
    }
    void SendMessages()
    {
        movingAllies.ForEach(x => x.SendMessage());
        movingEnemies.ForEach(x => x.SendMessage());
    }
    
    //cosa fa questo metodo? Da chi toglie cosa?
    public void RemoveShip(Ship ship, int isAlly)
    {
        if(isAlly==0){
            allies.Remove(ship);
        }
        else{
            enemies.Remove(ship);
        }
        ships.Remove(ship);

        //io in realtà vorrei distruggerlo, non me lo lascia fare (stefano)
        //TODO controllare che non venga più cercato lo scritt
        //TODO aggiungere la pulizia della griglia
        ship.gameObject.SetActive(false);
        ship.enabled = false;
    }
}