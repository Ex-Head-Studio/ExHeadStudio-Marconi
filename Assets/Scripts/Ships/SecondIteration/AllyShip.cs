using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AllyShip : AShip
{
    int moveId = 0;
    public override void ExecuteInstructions(AnswerStruct directives)
    {
        
    }

    //Le funzioni che seguono servono per dare al giocatore la possibilità di scegliere solo azioni consentite
    //e non tutte le azioni possibili, come nel caso delle navi nemiche
    public override bool LookForMovement()
    {
        canMove=false;
        for(int x = position.x - shipSO.movementRange; x < position.x + shipSO.movementRange; x++)
        {
            if(x >= 0 && x < gridManager._width)
            {
                if(x != position.x)
                {
                    Vector2Int pos = new Vector2Int(x, position.y);
                    Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                    shipMoves.Add(move);
                }
            }
        }
        for(int y = position.y - shipSO.movementRange; y < position.y + shipSO.movementRange; y++)
        {
            if(y >= 0 && y < gridManager._height)
            {
                if(y != position.y)
                {
                    Vector2Int pos = new Vector2Int(position.x, y);
                    Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                    shipMoves.Add(move);
                }
            }
        }
        //Se le carte permettono giusto di muoversi e poi il player decide dove, questa lista gli farà vedere solo dove potrà spostarsi.
        shipMoves = shipMoves.Where(x => gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Empty).ToList();
        if(shipMoves.Count > 0)
        {
            canMove = true;
        }
        return canMove;
    }
    public override bool LookForAttacks()
    {
        canAttack = false;
        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        
       
        for(int i = position.x - shipSO.attackRange; i < position.x + shipSO.attackRange; i++)
        {
            if(i >= 0 && i < gridManager._width)
            {
                if(i != position.x)
                {
                    Vector2Int pos = new Vector2Int(i, position.y);
                    if(gridManager.GetTileAtPosition(pos)._type == TileType.Enemy){
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    }
                }
            }
        }
        for(int j = position.y - shipSO.attackRange; j < position.y + shipSO.attackRange; j++)
        {
            if(j >= 0 && j < gridManager._height)
            {
                if(j != position.y)
                {
                    Vector2Int pos = new Vector2Int(position.x, j);
                    if(gridManager.GetTileAtPosition(pos)._type == TileType.Enemy){
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    }
                }
            }
        }
        return canAttack;
        
    }
    public override bool LookForAttacks(List<AShip> nearbyShips)
    {
        throw new System.NotImplementedException();
    }
    public override void SendMessage(Move move)
    {
        throw new System.NotImplementedException();
    }

    public void PerformAttack()
    {

    }

    public void PerformMovement()
    {

    }
}
