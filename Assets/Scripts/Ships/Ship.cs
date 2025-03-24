using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random=UnityEngine.Random;
public class Ship : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private ShipSO shipSO;

    [Header("Ship Parameters")]
    [SerializeField] float nearbyShipSearchRadius;
    [SerializeField] public Vector2 position;
    [SerializeField] private MessageSentEvent messageSentEvent;
    [SerializeField] private OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] private OnShipAttackEvent attackEvent;
    [SerializeField] private GridManager gridManager;
    private LayerMask shipLayer;
    public enum ShipState{
        Attacking,
        Moving,
        Waiting
    }
    
    public string shipName;
    public ShipManager manager;
    

    // questa va inserita nella logica delle navi
    private int health;
    public Vector2 nextPos;
    public ShipState currentState=ShipState.Waiting;
    public Vector2 targetPos;
    bool canMove;
    bool canAttack;
    public int faction;
    
    void Awake()
    {
        gridManager= FindFirstObjectByType<GridManager>();
        shipLayer=LayerMask.GetMask("Ship");

    }

   

   //TODO Gabriele controllare
    public void SendMessage()
    {
        Vector2 direction;
        switch(currentState){
            case ShipState.Attacking:
                direction=targetPos-position;
                if(direction.y==0){
                    if(direction.x>0){
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 3));
                    }
                    else{
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                       
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 0));
                    }
                    else{
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 1));
                    }
                }
                break;
            case ShipState.Moving:
                direction=nextPos-position;
                if(direction.y==0){
                    if(direction.x>0){
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 3));
                    }
                    else{
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 0));
                    }
                    else{
                        
                        messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 1));
                    }
                }
                break;
            default:
               
                //messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 1));
                break;
        }

    }
    
    //il metodo viene chiamato quando la nave registra una risposta a lei associata
    
    public void ExecuteInstructions(AnswerStruct answerStruct)
    {
        int entity = answerStruct.entity;
        bool answer = answerStruct.result;
        //True per gli alleati CONFERMA l'azione
        //True per i nemici NEGA l'azione

        if(this.faction == entity && this.faction==(int)Entity.ally)
        {
            if(shipName == answerStruct.receiver)
            {
                if(currentState==ShipState.Attacking)
                {
                    //evento dove si dichiara la posizione 2D della nave avversaria da colpire
                    attackEvent?.Invoke(new ShipAttackStruct(targetPos));

                }
                else if(currentState==ShipState.Moving)
                {
                    gridManager.MoveShip(position, nextPos, entity);
                    position=nextPos;
                    //nextPos=Vector2.negativeInfinity;
                }
            }
            else
            {
                currentState = ShipState.Waiting;
            }
        }
        else if(this.faction == entity && this.faction==(int)Entity.enemy)
        {
            if(shipName == answerStruct.receiver)
            {
                currentState = ShipState.Waiting;
            }
            else
            {
                    if(currentState==ShipState.Attacking)
                    {
                        //evento dove si dichiara la posizione 2D della nave avversaria da colpire
                        attackEvent?.Invoke(new ShipAttackStruct(targetPos));

                    }
                    else if(currentState==ShipState.Moving)
                    {
                        //qui siamo sicuri di non dover chiamare un metodo?
                        gridManager.MoveShip(position, nextPos, entity);
                        position=nextPos;
                        //nextPos=Vector2.negativeInfinity;
                    }
            }
        }
        
    }
    
    /*void SetNextPosition(Vector2 newPos)
    {
        nextPos=newPos;
    }*/

    public void SetState(ShipState newState)
    {
        currentState=newState;
    }



    //La nave cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione
    public bool LookForObjectives(List<Ship> possibleTargets)
    {
        canAttack=false;
        //Cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione tra le navi nemiche
        //SIAMO SICURI DELL'ORDINE DEI METODI?
        List<Ship> targets=possibleTargets.Where(k => k.position.x==position.x || k.position.y==position.y).OrderBy(x => Random.value).Take(1).ToList();        
        if(targets.Count>0){
            canAttack=true;
            targetPos=targets.OrderBy(x=>Random.value).Take(1).ToList()[0].position;
        }
        return canAttack;
    }   

    //TODO Gabriele controllare che inserisca giusto e non cancelli cosa serve
    //Allo stesso tempo, la nave controlla anche se ha spazio per muoversi, così da essere pronta a muoversi se non trova navi nemiche
    public bool LookForMovement(){
        canMove=false;

        //canMove=false;
        List<int> xOffsets=new List<int>(){-1, 1};
        List<int> yOffsets=new List<int>(){-1, 1};
        //Seleziona le navi vicine a quella attuale e prendi tutte le posizioni attuali e future di ciascuna nave trovata
        List<Vector2> nearbyShips = Physics.OverlapSphere(transform.position, nearbyShipSearchRadius, shipLayer).Select(x => x.GetComponent<Ship>().position).ToList();
        nearbyShips = nearbyShips.Concat(Physics.OverlapSphere(transform.position, nearbyShipSearchRadius, shipLayer).Select(x => x.GetComponent<Ship>().nextPos)
                    .Where(x => x!=Vector2.negativeInfinity).ToList()).ToList();
        //La nave mantiene solo gli offset che non la farebbero uscire dalla mappa e che non la farebbero andare su una casella già occupata

        //Rimuove gli offset che farebbero passare la nave su una posizione già prenotata o già occupata

        List<Vector2> possibleMoves=new List<Vector2>();
        foreach(int x in xOffsets){
                possibleMoves.Add(new Vector2(position.x+x, position.y));
        }
        foreach(int y in yOffsets){
                possibleMoves.Add(new Vector2(position.x, position.y+y));
        }
        if(nearbyShips.Count>0){
            List<Vector2> notValid= possibleMoves.Where(p => nearbyShips.Contains(p) ).ToList();
            possibleMoves=possibleMoves.Except(notValid).ToList();
            //possibleMoves = possibleMoves.Where(p => !nearbyShips.Contains(p) && gridManager.IsValidPosition(p)).ToList();
        }
        
        //possibleMoves contiene tutte le possibili mosse rimaste alla nave, se è vuota, significa che non ha mosse a disposizione
        //Debug.Log(shipName+ ": "+possibleMoves.Count);
        if(possibleMoves.Count>0){
            canMove=true;
            //canMove=true;

            //TODO qui siamo sicuri che faccia assegnazione? Non dobbiamo chiamare il metodo SetNextPos()?
            nextPos=possibleMoves.OrderBy(x => Random.value).Take(1).ToList()[0];
           // Debug.Log("Nave: " + shipName + " si sposta da " + position + " a " + nextPos);
            return true;
        }
        //int xOrY=Mathf.Round(Random.Range(0, 1));
        //Da
       // Debug.Log("Nave: " + shipName + " non trova posizioni valide");
        currentState=ShipState.Waiting;
        return false; 
    }
    public void SetFaction(int faction){
        this.faction=faction;
    }


    //possiamo valutare se passare anche il danno nella shipAttackStruct
    public void OnAttacked(ShipAttackStruct attackPosition)
    {
        if(position.x == attackPosition.gridPosition.x && position.y == attackPosition.gridPosition.y)
        {
            //per ora decrementiamo di uno la vita
            health--;
            if(health<=0)
            {
                shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(shipName, faction, position));
            }
            manager.RemoveShip(this, faction);
        }
    }
}