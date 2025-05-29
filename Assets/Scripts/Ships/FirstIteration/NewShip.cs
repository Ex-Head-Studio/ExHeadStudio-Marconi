using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random= UnityEngine.Random;
public class NewShip : AShip
{
    public override void LookForMoves()
    {
        throw new NotImplementedException();
    }
    protected int moveId = 0;
   
    public override bool LookForMovement()
    {
        canMove=false;
        List<Move> bestMoves = new List<Move>();
        List<Vector2Int> possibleMoves= new List<Vector2Int>();
        for(int i=0 ; i<2; i++){
            for(int j=-shipSO.movementRange; j<shipSO.movementRange;j++){
                if(i==0)
                    possibleMoves.Add(new Vector2Int(position.x+j, position.y));
                possibleMoves.Add( new Vector2Int(position.x, position.y+j));
            }
        }
        possibleMoves=possibleMoves.Where(x=> x.x>=0 && x.x<mapWidth && x.y>=0 && x.y<mapHeight && x!=position).Distinct().ToList(); 
        foreach(Vector2Int move in possibleMoves){
            Move newMove=new Move(moveId++, shipName, move, MessageType.movement, manager.InfluenceMap.CalculateMoveValue(move.x, move.y));
            bestMoves.Add(newMove);
            Debug.Log("Nave "+shipName+" muove in "+newMove.GetTargetPos()+" con value: "+newMove.value);
        }
        //Recupera tutte le posizioni occupate da navi, tra quelle che può fare la nave corrente
        List<Vector2Int> occupiedPositions = possibleMoves.Where(x=>GridManager.Instance._tiles[x].GetShip()!=null).ToList();

        Debug.Log("Navi affianco: "+ occupiedPositions.Count());
        //Toglie tutti i movimenti che porterebbero a caselle già occupate
        bestMoves=bestMoves.Where(x=>!occupiedPositions.Contains(x.GetTargetPos())).ToList();

        if(faction==0)
            bestMoves.Sort((a, b)=> b.value.CompareTo(a.value));
        else bestMoves.Sort((a, b)=> a.value.CompareTo(b.value));

        if(bestMoves.Count()>manager.NumberOfMessages){
            bestMoves=bestMoves.Take(2).ToList();
            canMove=true;
        }
        else if(bestMoves.Count()>0){
            canMove=true;
        }
        
        shipMoves=bestMoves;
        return canMove;
    }

    public override bool LookForAttacks(List<AShip> adv)
    {
        canAttack=false;
        //Cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione tra le navi nemiche
        List<Vector2Int> targets=adv.Select(x=> x.position).Where(k => (k.x==position.x || k.y==position.y) && (Vector2Int.Distance(k, position)<= shipSO.attackRange)).ToList();        
        
        if(targets.Count()>manager.NumberOfMessages){
            canAttack=true;
            targets=targets.Take(2).ToList();
        }
        else if(targets.Count()>0){
            canAttack=true;
        }
        targets.ForEach(x=> shipMoves.Add(new Move(moveId++, shipName, x, MessageType.attack, faction==0 ? -1f:1f)));
        //shipMoves=shipMoves.Distinct().ToList();
        return canAttack;
    }

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
                        GridManager.Instance.MoveShip(position, selectedMove.GetTargetPos(), entity);
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
                        GridManager.Instance.MoveShip(position, selectedMove.GetTargetPos(), entity);
                        position=selectedMove.GetTargetPos();
                        canMove=false;
                        //nextPos=Vector2.negativeInfinity;
                    }
                }
            }
        }
    }

    public override bool LookForAttacks()
    {
        throw new NotImplementedException();
    }

    public override void SendMessage(Move move)
    {
        throw new NotImplementedException();
    }
}
