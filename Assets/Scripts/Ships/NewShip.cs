using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random= UnityEngine.Random;
public class NewShip : Ship
{
    /*
    protected StrategyState strategyState;
    
    public void UpdateState(){
        //da provare quando c'è tempo (gabriele)

        int enemies = Physics.OverlapSphereNonAlloc(transform.position, shipSO.movementRange, null, shipLayer);
        if(enemies>0){
            currentState=StrategyState.InRange;
        }
        else currentState=StrategyState.Patrolling;

        switch(currentState){
            case StrategyState.InRange:
                FindBestPositioning();
            break;
            case StrategyState.Patrolling:
                LookForMovement();
            break;
            default:

            break;
        }
    }
    public void FindBestPositioning(){

    }*/
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
        possibleMoves=possibleMoves.Where(x=> x.x>=0 && x.x<mapWidth && x.y>=0 && x.y<mapHeight).ToList(); 
        foreach(Vector2Int move in possibleMoves){
            bestMoves.Add(new Move(shipName, move, MessageType.movement, manager.influenceMap.GetValue(move.x, move.y)));
        }
        List<Vector2Int> occupiedPositions = Physics.OverlapSphere(this.transform.position, shipSO.movementRange, shipLayer ).Select(x=> x.GetComponent<Ship>().position).ToList();
        occupiedPositions=occupiedPositions.Concat(Physics.OverlapSphere(this.transform.position, shipSO.movementRange, gameObject.layer).Select(x=> x.GetComponent<Ship>().position)).ToList();
        bestMoves=bestMoves.Where(x=>!occupiedPositions.Contains(x.GetTargetPos())).ToList();

        if(faction==0)
            bestMoves.Sort((a, b)=> b.value.CompareTo(a.value));
        else bestMoves.Sort((a, b)=> a.value.CompareTo(b.value));

        if(bestMoves.Count()>0){
            canMove=true;
            nextPos=bestMoves.Select(a=>a.GetTargetPos()).ToList();
        }
        return canMove;
    }

    public bool LookForObjectives()
    {
        canAttack=false;
        //Cerca se ci sono navi nemiche in linea retta rispetto alla sua posizione tra le navi nemiche
        //SIAMO SICURI DELL'ORDINE DEI METODI?
        List<Vector2Int> targets=Physics.OverlapSphere(transform.position, shipSO.attackRange, shipLayer).ToList().Select(x=> x.GetComponent<NewShip>().position).Where(k => k.x==position.x || k.y==position.y).ToList();        
        if(targets.Count>=2){
            canAttack=true;
            targetPos=targets.OrderBy(x=>Random.value).Take(2).ToList();
        }
        else if(targets.Count>0){
            canAttack=true;
            targetPos=targets;
        }
    
        return canAttack;
    }
}
