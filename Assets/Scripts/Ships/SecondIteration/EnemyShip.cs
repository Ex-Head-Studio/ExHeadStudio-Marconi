using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EnemyShip : AShip
{
    int numberOfTurnsPredicted;
    Move initialMove;
    public override void ExecuteInstructions(AnswerStruct directives){
        //TODO: Eseguire le istruzioni ricevute da ShipManager
    }

    public override bool LookForMovement()
    {
        List<Move> possibleMoves= new List<Move>();
        for(int x=position.x-shipSO.movementRange; x<position.x+shipSO.movementRange; x++){
            if(x>=0 && x<gridManager._width){
                Vector2Int pos = new Vector2Int(x, position.y);
                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                Move move = new Move(shipName, pos, MessageType.movement, value);
            }
                
        }
        for(int y=position.y-shipSO.movementRange; y<position.y+shipSO.movementRange;y++){
            if(y>=0 && y<gridManager._height){
                Vector2Int pos = new Vector2Int(position.x, y);
                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                possibleMoves.Add(new Move(shipName, pos, MessageType.movement,value));
            }
        }

        return false;
        
    }

    public override bool LookForAttacks()
    {
        throw new System.NotImplementedException();
    }
    public override bool LookForAttacks(List<AShip> nearbyShips)
    {
        throw new System.NotImplementedException();
    }
    public override void SendMessage(Move move)
    {
        throw new System.NotImplementedException();
    }
   
    
}
