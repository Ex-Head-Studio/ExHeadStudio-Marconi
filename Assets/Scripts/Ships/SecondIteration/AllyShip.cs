using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


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
        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canMove=false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - movementRange; x--)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    gridManager.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && gridManager.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + movementRange; x++)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    gridManager.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && gridManager.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - movementRange; y--)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    gridManager.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && gridManager.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }
        
        //ricerca verso alto
        for (int y = position.y; y <= position.y + movementRange; y++)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    gridManager.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && gridManager.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //Se le carte permettono giusto di muoversi e poi il player decide dove, questa lista gli farà vedere solo dove potrà spostarsi.
        shipMoves = shipMoves.Where(x => gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Empty).ToList();
        if(shipMoves.Count > 0)
        {
            foreach(Move move in shipMoves)
            {
                gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canMove = true;
        }
        else
        {
            ByPassEffect();
        }
        return canMove;
    }
    public override bool LookForAttacks()
    {

        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canAttack = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - attackRange; x--)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (gridManager.GetTileAtPosition(pos)._type == TileType.Enemy
                     && gridManager.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + attackRange; x++)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (gridManager.GetTileAtPosition(pos)._type == TileType.Enemy
                     && gridManager.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - attackRange; y--)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (gridManager.GetTileAtPosition(pos)._type == TileType.Enemy
                     && gridManager.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + attackRange; y++)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (gridManager.GetTileAtPosition(pos)._type == TileType.Enemy
                     && gridManager.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }
       
       
        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => (gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle)).ToList();
        if(shipMoves.Count > 0)
        {
            foreach(Move move in shipMoves)
            {
                gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
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

    public void ReceiveTile(Tile tile)
    {

        if(gameObject.TryGetComponent<CardAllyShip>( out cardAllyScript))
        {
            if(!cardAllyScript.IsSelected()) return;
        }
        if(tile._type == TileType.Empty)
        {
            Debug.Log("Movement in tile: " + tile.name);
            gridManager.MoveShip(this.position, gridManager.GetPositionFromTile(tile), this.faction);
            this.position = Vector2Int.RoundToInt(gridManager.GetPositionFromTile(tile));
        }
        else if (tile._type == TileType.Enemy || tile._type == TileType.Obstacle)
        {
            Debug.Log("Attack in tile: " + tile.name);
            PerformAttack(gridManager.GetPositionFromTile(tile));
        }  

        //disattivo le tile interagibili
        foreach (Move move in shipMoves)
        {
            gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable(faction);
        }

        //comunico che l'evento è terminato
        Debug.Log("Effect ended: " + effectSO.name);
        effectSO.EndEffect(0);
    }

    private void ByPassEffect()
    {
        transform.DOShakeRotation(0.5f, 10, 10, 90).OnComplete(() =>
        {
            transform.DOKill(true);
        });

        effectSO.EndEffect(0);
    }

    public void AddHealth(int healthToAdd)
    {
        health += healthToAdd;
        displayHealthScript.AddHealth(healthToAdd);
    }

}
