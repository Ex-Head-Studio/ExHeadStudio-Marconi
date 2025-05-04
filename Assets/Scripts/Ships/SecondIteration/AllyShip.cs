using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class AllyShip : AShip
{
    int moveId = 0;

    private CardAllyShip cardAllyScript;
    private DisplayHealth displayHealthScript;  


    //la nave, per gli eventi di movimento e attacco, ha bisogno di sapere quale effetto sta usando
    private AbstractEffectSO effectSO;


    private void OnEnable()
    {
        Tile.tileSelected += ReceiveTile;
    }

    private void OnDisable()
    {
        Tile.tileSelected -= ReceiveTile;
    }

    private void  Start()
    {
        displayHealthScript = GetComponent<DisplayHealth>();
    }
    //Le funzioni che seguono servono per dare al giocatore la possibilità di scegliere solo azioni consentite
    //e non tutte le azioni possibili, come nel caso delle navi nemiche
    [ContextMenu("LookForMovement")]
    public override bool LookForMovement()
    {
        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canMove=false;
        for(int x = position.x - shipSO.movementRange; x <= position.x + shipSO.movementRange; x++)
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
        for(int y = position.y - shipSO.movementRange; y <= position.y + shipSO.movementRange; y++)
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
        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        
       
        for(int i = position.x - shipSO.attackRange; i <= position.x + shipSO.attackRange; i++)
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
        for(int j = position.y - shipSO.attackRange; j <= position.y + shipSO.attackRange; j++)
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

    
    public void ReceiveEffect(AbstractEffectSO effectSO)
    {
        Debug.Log("Received effect: " + effectSO.name);
        this.effectSO = effectSO;
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
        else if (tile._type == TileType.Enemy)
        {
            Debug.Log("Attack in tile: " + tile.name);
            PerformAttack(gridManager.GetPositionFromTile(tile));
        }  

        //disattivo le tile interagibili
        foreach(Move move in shipMoves)
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
