using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random=UnityEngine.Random;
using System;
public class Ship : AShip
{
    
    void Awake()
    {
        gridManager= FindFirstObjectByType<GridManager>();
        
        mapHeight=FindAnyObjectByType<GridManager>()._height;
        mapWidth=FindFirstObjectByType<GridManager>()._width;
    }

    void Start()
    {

        //TODO l'assegnazione dei parametri deve essere discussa, se li modifichiamo ogni istanza dinave deve avere le proprie varabili
        health=shipSO.health;

        attackRange=shipSO.attackRange;
        movementRange=shipSO.movementRange;
    }

    public override void SendMessage(Move move)
    {
        Vector2Int direction;
        switch(move.GetMessageType())
        {
            case MessageType.attack:
                direction = move.GetTargetPos()-position;
                if(direction.y==0){
                    if(direction.x>0){
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 3));
                    }
                    else{
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                       
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 0));
                    }
                    else{
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 1));
                    }
                }
                break;
            case MessageType.movement:
                direction=move.GetTargetPos()-position;
                if(direction.y==0){
                    if(direction.x>0){
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 3));
                    }
                    else{
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 2));
                    }
                }
                else{
                    if(direction.y>0){
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 0));
                    }
                    else{
                        
                        shipSO.messageSentEvent?.Invoke(new MessageStruct(move.GetShipName(), move.GetIdMove(), (int) move.GetMessageType(), faction, 1));
                    }
                }
                break;
            default:
               
                //messageSentEvent?.Invoke(new MessageStruct(shipName, (int) currentState, faction, 1));
                break;
        }

    }
    


    //il metodo viene chiamato quando la nave registra una risposta a lei associata, deve rispondere all'evento
    public override void ExecuteInstructions(AnswerStruct answerStruct)
    {
        int entity = answerStruct.entity;
        bool answer = answerStruct.result;
        //True per gli alleati CONFERMA l'azione, le altre navi non devono fare nulla
        //True per i nemici NEGA l'azione, le altre navi devono eseguire le loro azioni nulla

        if(this.faction == entity && this.faction==(int)Entity.ally)
        {
            if(shipName == answerStruct.receiver && answer)
            {
                Move selectedMove;
                 
                if(shipMoves.Where(x=>x.GetIdMove()==answerStruct.idMove).Count() > 0 )
                {
                    selectedMove = shipMoves.Where(x=>x.GetIdMove()==answerStruct.idMove).ToList()[0];
                    if(selectedMove.GetMessageType() == MessageType.attack){
                        shipSO.attackEvent?.Invoke(new ShipAttackStruct(selectedMove.GetTargetPos(), shipSO.attackPower));
                    }
                    else{
                        gridManager.MoveShip(position, selectedMove.GetTargetPos(), entity);
                        position=selectedMove.GetTargetPos();
                    }
                }
            }
        
          
        }
        else if(this.faction == entity && this.faction==(int)Entity.enemy)
        {
            if(shipName == answerStruct.receiver && answer)
            {
                currentState = ShipState.Waiting;
            }
            else if(shipName == answerStruct.receiver && !answer)
            {
                Move selectedMove;
                if(shipMoves.Where(x=> x.GetIdMove()== answerStruct.idMove).Count()>0){
                    selectedMove = shipMoves.Where(x=>x.GetIdMove()==answerStruct.idMove).ToList()[0];
                    
                    if(selectedMove.GetMessageType()==MessageType.attack && canAttack)
                    {
                        canAttack=false;
                        //evento dove si dichiara la posizione 2D della nave avversaria da colpire
                        shipSO.attackEvent?.Invoke(new ShipAttackStruct(selectedMove.GetTargetPos(), shipSO.attackPower));

                    }
                    else if(selectedMove.GetMessageType()==MessageType.movement && canMove)
                    {
                        //qui siamo sicuri di non dover chiamare un metodo?
                        gridManager.MoveShip(position, selectedMove.GetTargetPos(), entity);
                        position=selectedMove.GetTargetPos();
                        canMove=false;
                        //nextPos=Vector2.negativeInfinity;
                    }
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
    override public bool LookForAttacks(List<AShip> possibleTargets)
    {
        canAttack=false;
        //Cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione tra le navi nemiche
        List<AShip> targets=possibleTargets.Where(k => k.position.x==position.x || k.position.y==position.y).OrderBy(x => Random.value).Take(1).ToList();        
        if(targets.Count>0){
            canAttack=true;
            targetPos=targets.OrderBy(x=>Random.value).Select(a=>a.position).Take(2).ToList();
        }
        return canAttack;
    }   

    override public bool LookForAttacks()
    {
       throw new System.NotImplementedException();
    }

    //TODO Gabriele controllare che inserisca giusto e non cancelli cosa serve
    //Allo stesso tempo, la nave controlla anche se ha spazio per muoversi, così da essere pronta a muoversi se non trova navi nemiche
    public override bool LookForMovement(){
        canMove=false;

        //canMove=false;
        List<int> xOffsets=new List<int>(){-1, 1};
        List<int> yOffsets=new List<int>(){-1, 1};
        //Seleziona le navi vicine a quella attuale e prendi tutte le posizioni attuali e future di ciascuna nave trovata
        List<Vector2Int> nearbyShips = Physics.OverlapSphere(transform.position, nearbyShipSearchRadius, adversaryShipLayer).Select(x => x.GetComponent<Ship>().position).ToList();
        nearbyShips = nearbyShips.Concat(Physics.OverlapSphere(transform.position, nearbyShipSearchRadius, adversaryShipLayer).SelectMany(x => x.GetComponent<Ship>().nextPos)
                    .Where(x => x!=Vector2.negativeInfinity).ToList()).ToList();
        //La nave mantiene solo gli offset che non la farebbero uscire dalla mappa e che non la farebbero andare su una casella già occupata

        //Rimuove gli offset che farebbero passare la nave su una posizione già prenotata o già occupata

        List<Vector2Int> possibleMoves=new List<Vector2Int>();
        foreach(int x in xOffsets){
                possibleMoves.Add(new Vector2Int(position.x+x, position.y));
        }
        foreach(int y in yOffsets){
                possibleMoves.Add(new Vector2Int(position.x, position.y+y));
        }
        if(nearbyShips.Count>0){
            List<Vector2Int> notValid= possibleMoves.Where(p => nearbyShips.Contains(p) ).ToList();
            possibleMoves=possibleMoves.Except(notValid).ToList();
            //possibleMoves = possibleMoves.Where(p => !nearbyShips.Contains(p) && gridManager.IsValidPosition(p)).ToList();
        }
        
        //possibleMoves contiene tutte le possibili mosse rimaste alla nave, se è vuota, significa che non ha mosse a disposizione
        //Debug.Log(shipName+ ": "+possibleMoves.Count);
        if(possibleMoves.Count>0){
            canMove=true;
            //canMove=true;

            //TODO qui siamo sicuri che faccia assegnazione? Non dobbiamo chiamare il metodo SetNextPos()?
            nextPos=possibleMoves.OrderBy(x => Random.value).Take(2).ToList();
           // Debug.Log("Nave: " + shipName + " si sposta da " + position + " a " + nextPos);
            return true;
        }
        //int xOrY=Mathf.Round(Random.Range(0, 1));
        //Da
       // Debug.Log("Nave: " + shipName + " non trova posizioni valide");
        currentState=ShipState.Waiting;
        return false; 
    }
}