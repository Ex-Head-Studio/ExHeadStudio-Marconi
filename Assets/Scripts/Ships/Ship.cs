using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Ship : MonoBehaviour
{
    [Header("Ship Parameters")]
    [SerializeField] float nearbyShipSearchRadius;
    public enum ShipState{
        Waiting,
        Moving,
        Attacking
    }
    string shipName;
    Vector2 position;
    Vector2 nextPos;
    ShipState currentState=ShipState.Waiting;
    Vector2 targetPos;
    bool canMove;
    bool canAttack;
    private bool isAlly;
    public Ship(string name){
        this.name=name;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetupMessage(){
        //TODO: metodo che genera il messaggio da inviare in base alla mossa scelta dalla nave
    }
    void SendMessage(){
        //TODO: La nave manda un messaggio al giocatore per dirgli cosa intende fare
    }
    
    void ExecuteInstructions(){
        //TODO: la nave esegue il comando che le è stato impartito
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
        targetPos=possibleTargets.Where(x => x.position.x==position.x || x.position.y==position.y).OrderBy(x => Random.value).Take(1).ToList()[0].position;
        if(targetPos!=null){
            canAttack=true;
        }
        return canAttack;
    }   
    //Allo stesso tempo, la nave controlla anche se ha spazio per muoversi, così da essere pronta a muoversi se non trova navi nemiche
    public void LookForMovement(List<Ship> ships){
        canMove=false;
        List<int> xOffsets=new List<int>(){-1, 0, 1};
        List<int> yOffsets=new List<int>(){-1, 0, 1};
        //Seleziona le navi vicine a quella attuale e prendi tutte quelle navi che hanno già selezionato la loro prossima posizione, per verificare che la nave attuale non scelga posizioni già occupate
        List<Vector2> nearbyShips = Physics.OverlapSphere(transform.position, nearbyShipSearchRadius).Select(x => x.GetComponent<Ship>()).Where(x => x.nextPos!=null).Select(x=>x.nextPos).ToList();
        //La nave mantiene solo gli offset che non la farebbero uscire dalla mappa e che non la farebbero andare su una casella già occupata
        //TODO: controllare che la nave non vada su una casella già occupata e che rimanga nella mappa

        xOffsets.Where(p=> nearbyShips.Contains(new Vector2(position.x+p, position.y))==false);
      
        int xOrY=Random.Range(0,1);
       //Da

        
    }
}