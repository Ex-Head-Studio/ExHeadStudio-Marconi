using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using System.Collections;


public class AllyShip : AShip
{
    int moveId = 0;
    Vector3 targetPosition;
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

        canMove = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - movementRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + movementRange; x++)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - movementRange; y--)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + movementRange; y++)
        {
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Enemy)
                {
                    break;
                }

                Move move = new Move(moveId++, shipName, pos, MessageType.movement, 0);
                if (move != null && GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Empty)
                    shipMoves.Add(move);
            }
        }

        //Se le carte permettono giusto di muoversi e poi il player decide dove, questa lista gli farà vedere solo dove potrà spostarsi.
        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Empty).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
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
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                    break;
                }

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }


        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => (GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle)).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
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
    void Update()
    {
        if (startMovement)
        {
            //Attiva l'animazione di movimento della nave
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, (Vector3.Distance(transform.position, targetPosition)) / timeToMove * Time.fixedDeltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                startMovement = false;
            }
        }
    }
    public override void LookForMoves()
    {
        throw new System.NotImplementedException();
    }
    public void ReceiveTile(Tile tile)
    {

        if (gameObject.TryGetComponent<CardAllyShip>(out cardAllyScript))
        {
            if (!cardAllyScript.IsSelected()) return;
        }
        if (tile._type == TileType.Empty)
        {
            Debug.Log("Movement in tile: " + tile.name);
            targetPosition = tile.transform.position;

            StartCoroutine(MoveShip(tile));

        }
        else if (tile._type == TileType.Enemy || tile._type == TileType.Obstacle)
        {
            Debug.Log("Attack in tile: " + tile.name);
            StartCoroutine(Attack(tile));
        }

        //disattivo le tile interagibili
        foreach (Move move in shipMoves)
        {
            GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable(faction);
        }

        //comunico che l'evento è terminato
        Debug.Log("Effect ended: " + effectSO.name);
        effectSO.EndEffect(0);
    }
    IEnumerator Attack(Tile tile)
    {
        //shipAnimator.Play("Attack");
        //yield return new WaitForSeconds(shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(0);
        PerformAttack(GridManager.Instance.GetPositionFromTile(tile));
    }
    IEnumerator MoveShip(Tile tile)
    {
        //shipAnimator.Play("Startup");
        startMovement = true;
        //yield return new WaitForSeconds(shipAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length + shipAnimator.GetNextAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(timeToMove);
        GridManager.Instance.MoveShip(this.position, GridManager.Instance.GetPositionFromTile(tile), this.faction);
        this.position = Vector2Int.RoundToInt(GridManager.Instance.GetPositionFromTile(tile));
    }

    private void ByPassEffect()
    {
        transform.DOShakeRotation(0.5f, 10, 10, 90).OnComplete(() =>
        {
            transform.DOKill(true);
        });

        effectSO.EndEffect(0);
    }



    #region Effetti richiamabili dalle carte


    public void AddHealth(int healthToAdd)
    {
        health += healthToAdd;
        displayHealthScript.AddHealth(healthToAdd);
    }

    /// <remark><summary>
    /// The function deals 1 damage to all entities in a square (1 tile) area around the ship.
    /// </summary></remark>
    public void SquareDamage()
    {
        CicloX();
        CicloY();
        CicloDiagonaliQuadrato();
    }

    private void CicloDiagonaliQuadrato()
    {

        //diagonale
        for (int x = (int)position.x - 1, y = (int)position.y - 1;
            x < (int)position.x + 1 && y < (int)position.y + 1;
            x++, y++)
        {
            if (x != (int)position.x && y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), 1));
                InstantiateEffect(new Vector2(x, y));
            }
        }

        //antidiagonale
        for (int x = (int)position.x - 1, y = (int)position.y + 1;
            x < (int)position.x + 1 && y > (int)position.y - 1;
            x++, y--)
        {
            if (x != (int)position.x && y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), 1));
                InstantiateEffect(new Vector2(x, y));
            }
        }
    }

    private void CicloX()
    {
        for (int x = (int)position.x - 1; x < (int)position.x + 1; x++)
        {
            if (x != (int)position.x)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, position.y), 1));
                InstantiateEffect(new Vector2(x, (int)position.y));
            }
        }
    }

    private void CicloY()
    {
        for (int y = (int)position.y - 1; y < (int)position.y + 1; y++)
        {
            if (y != (int)position.y)
            {
                shipSO.attackEvent.Invoke(new ShipAttackStruct(new Vector2(position.x, y), 1));
                InstantiateEffect(new Vector2((int)position.x, y));
            }
        }
    }

    private void InstantiateEffect(Vector2 position)
    {
        //Istanziare qui l'effetto visivo dell'attacco
    }


    public bool LookForAttacksWithoutObstacles()
    {

        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canAttack = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - attackRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }


        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
        }
        return canAttack;
    }



    //da capire -> attacco sulla singola riga
    /*public bool LookForAttackInLine()
    {
        //quando la invoco, pulisco la lista delle azioni possibili e la riempio con le nuove
        shipMoves.Clear();

        canAttack = false;

        //ricerca verso sx
        for (int x = position.x; x >= position.x - attackRange; x--)
        {
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy ||
                    GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (x >= 0 && x < GridManager.Instance._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
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
            if (y >= 0 && y < GridManager.Instance._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if ((GridManager.Instance.GetTileAtPosition(pos)._type == TileType.Enemy
                    || GridManager.Instance.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                     && GridManager.Instance.GetTileAtPosition(pos).GetShip() != null)
                {
                    Move move = new Move(moveId++, shipName, pos, MessageType.attack, 0);
                    shipMoves.Add(move);
                    canAttack = true;
                }
            }
        }


        //Se il giocatore ha la libertà di scegliere in quale posizione attaccare, allora questo metodo
        //gli farà vedere solo le posizioni in cui può attaccare.
        shipMoves = shipMoves.Where(x => GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Enemy ||
                                    GridManager.Instance.GetTileAtPosition(x.GetTargetPos())._type == TileType.Obstacle).ToList();
        if (shipMoves.Count > 0)
        {
            foreach (Move move in shipMoves)
            {
                GridManager.Instance.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction, move);
            }
            canAttack = true;
        }
        else
        {
            ByPassEffect();
        }
        return canAttack;
    }*/

    


    #endregion
}
