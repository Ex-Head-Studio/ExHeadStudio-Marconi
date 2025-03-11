using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random=UnityEngine.Random;
using System.Data.Common;
using System;
using UnityEditor.Experimental.GraphView;
public class Ship : MonoBehaviour
{
    [Header("Ship Parameters")]
    [SerializeField] float nearbyShipSearchRadius;
    [SerializeField] private Vector2 position;
    [SerializeField] private MessageSentEvent messageSentEvent;
    public enum ShipState{
        Attacking,
        Moving,
        Waiting
    }
    public string shipName;
    public ShipManager manager;
    
    public Vector2 nextPos;
    public ShipState currentState=ShipState.Waiting;
    public Vector2 targetPos;
    bool canMove;
    bool canAttack;
    public int faction;
    
    void Awake()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void SetupMessage(){
        //TODO: metodo che genera il messaggio da inviare in base alla mossa scelta dalla nave
    }
    public void SendMessage(){
        //TODO: La nave manda un messaggio al giocatore per dirgli cosa intende fare
        Vector2 direction;
        switch(currentState){
            case ShipState.Attacking:
                direction=targetPos-position;
                if(direction.y==0){
                    if(direction.x>0){
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 3));
                    }
                    else{
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 0));
                    }
                    else{
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 1));
                    }
                }
                break;
            case ShipState.Moving:
                direction=nextPos-position;
                if(direction.y==0){
                    if(direction.x>0){
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 3));
                    }
                    else{
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 0));
                    }
                    else{
                        messageSentEvent?.Invoke(new MessageStruct(name, (int) currentState, faction, 1));
                    }
                }
                break;
            default:
                //messageSentEvent.Raise(new MessageStruct(name, 2, faction, 0));
                break;
        }

    }
    
    public void ExecuteInstructions(bool answer, int faction){
        if(this.faction==0){
            if(answer){
                if(currentState==ShipState.Attacking){
                    //TODO: evento dove si dichiara la posizione 2D della nave avversaria da colpire
                }
                else if(currentState==ShipState.Moving){
                    position=nextPos;
                    //TODO: indicare alla griglia di spostare la nave dalla posizione corrente alla posizione successiva
                    nextPos=Vector2.negativeInfinity;
                }
            }
        }

    }
    void SetNextPosition(Vector2 newPos){
        nextPos=newPos;
    }
    public void SetState(ShipState newState){
        currentState=newState;
    }
    //La nave cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione
    public bool LookForObjectives(List<Ship> possibleTargets){
        canAttack=false;
        //Cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione tra le navi nemiche
        targetPos=possibleTargets.Where(k => k.position.x==position.x || k.position.y==position.y).OrderBy(x => Random.value).Take(1).ToList()[0].position;
        if(targetPos!=null){
            canAttack=true;
        }
        return canAttack;
    }   
    //Allo stesso tempo, la nave controlla anche se ha spazio per muoversi, così da essere pronta a muoversi se non trova navi nemiche
    public bool LookForMovement(List<Ship> ships){
        canMove=false;

        List<int> xOffsets=new List<int>(){-1, 0, 1};
        List<int> yOffsets=new List<int>(){-1, 0, 1};
        //Seleziona le navi vicine a quella attuale e prendi tutte quelle navi che hanno già selezionato la loro prossima posizione, per verificare che la nave attuale non scelga posizioni già occupate
        List<Vector2> nearbyShips = Physics.OverlapSphere(transform.position, nearbyShipSearchRadius).Select(x => x.GetComponent<Ship>()).Where(x => x.nextPos!=Vector2.negativeInfinity).Select(x=>x.nextPos).ToList();
        //La nave mantiene solo gli offset che non la farebbero uscire dalla mappa e che non la farebbero andare su una casella già occupata
        //TODO: controllare che la nave non vada su una casella già occupata e che rimanga nella mappa

        xOffsets.Where(p=> nearbyShips.Contains(new Vector2(position.x+p, position.y))==false);
        yOffsets.Where(p=> nearbyShips.Contains(new Vector2(position.x, position.y+p))==false);
        //Se la nave ha spazio per muoversi, può muoversi
        if(xOffsets.Count>0){
            canMove=true;
            return true;
        }
        //int xOrY=Mathf.Round(Random.Range(0, 1));
       //Da
        return false; 
    }
    public void SetFaction(int faction){
        this.faction=faction;
    }

    void OnDestroy()
    {
        manager.RemoveShip(this, faction);
        //Invia evento per UI
    }
}