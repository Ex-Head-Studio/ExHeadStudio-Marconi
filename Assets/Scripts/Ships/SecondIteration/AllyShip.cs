using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AllyShip : AShip
{
    int moveId = 0;

    private CardAllyShip cardAllyScript;


    private void OnEnable()
    {
        Tile.tileSelected += ReceiveTile;
    }

    private void OnDisable()
    {
        Tile.tileSelected -= ReceiveTile;
    }


    //Le funzioni che seguono servono per dare al giocatore la possibilità di scegliere solo azioni consentite
    //e non tutte le azioni possibili, come nel caso delle navi nemiche
    [ContextMenu("LookForMovement")]
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
                    if(move != null)
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
            foreach(Move move in shipMoves)
            {
                gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(true);
            }
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

        shipMoves = shipMoves.Where(x => gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy).ToList();
        if(shipMoves.Count > 0)
        {
            foreach(Move move in shipMoves)
            {
                gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(true);
            }
            canAttack = true;
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

    public void PerformAttack(Vector2 targetPos)
    {
        shipSO.attackEvent.Invoke(new ShipAttackStruct(targetPos, shipSO.attackPower));
    }

    public void PerformMovement()
    {

    }

    public void ReceiveTile(Tile tile)
    {
        if(gameObject.TryGetComponent<CardAllyShip>( out cardAllyScript))
        {
            if(!cardAllyScript.IsSelected()) return;
        }
        if(tile._type == TileType.Empty)
        {
            gridManager.MoveShip(this.position, gridManager.GetPositionFromTile(tile), this.faction);
            this.position = Vector2Int.RoundToInt(gridManager.GetPositionFromTile(tile));
        }
        else if (tile._type == TileType.Enemy)
        {
            PerformAttack(gridManager.GetPositionFromTile(tile));
        }  

        //disattivo le tile interagibili
        foreach(Move move in shipMoves)
        {
            gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable();
        }

        cardAllyScript.DeselectShip();
    }

    public void AddHealth(int healthToAdd)
    {
        health += healthToAdd;
        //update UI
    }

}
