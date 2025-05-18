using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EnemyShip : AShip
{
    Move initialMove;
    int idMove = 0;
    //Quando viene chiamata la fine del turno, Execute Instructions fa fare l'azione migliore alla nave, a meno che non sia stata bloccata
    //dal giocatore, in quel caso non fa nulla
    public override void ExecuteMove()
    {
        if (!canMove && !canAttack)
        {
            SwitchOffMovesVisualization(shipMoves);

            shipMoves.Clear();
            return;
        }

        switch (initialMove.GetMessageType())
        {
            case MessageType.attack:
                shipSO.attackEvent?.Invoke(new ShipAttackStruct(initialMove.GetTargetPos(), shipSO.attackPower));
                break;
            case MessageType.movement:
                gridManager.MoveShip(position, initialMove.GetTargetPos(), faction);

                position = initialMove.GetTargetPos();
                break;
        }
        moveDone = true;

        gridManager.GetTileAtPosition(initialMove.GetTargetPos()).SetTileNotInteractable(faction);
    }

    public override bool LookForMovement()
    {
        moveDone = false;
        canMove = false;
        shipMoves.Clear();
        List<Move> possibleMoves = new List<Move>();
        //Crea una lista di possibili mosse e aggiungi tutte le mosse in tutte le posizioni che rimangono all'interno della mappa, 
        // in verticale e orizzontale

        //ricerca verso sx
        for (int x = position.x; x >= position.x - movementRange; x--)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (gridManager.GetTileAtPosition(pos)._type == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso dx
        for (int x = position.x; x <= position.x + movementRange; x++)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                Vector2Int pos = new Vector2Int(x, position.y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (gridManager.GetTileAtPosition(pos)._type == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso basso
        for (int y = position.y; y >= position.y - movementRange; y--)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (gridManager.GetTileAtPosition(pos)._type == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }

        //ricerca verso alto
        for (int y = position.y; y <= position.y + movementRange; y++)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                Vector2Int pos = new Vector2Int(position.x, y);

                if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                {
                    break;
                }

                float value = manager.InfluenceMap.CalculateMoveValue(pos.x, pos.y);
                if (gridManager.GetTileAtPosition(pos)._type == TileType.Empty)
                {
                    possibleMoves.Add(new Move(idMove++, shipName, pos, MessageType.movement, value));
                }
            }
        }


        //Fatto ciò, elimina le mosse che portano a posizioni già occupate.
        possibleMoves = possibleMoves.Where(x => gridManager.GetTileAtPosition(x.GetTargetPos())._type == TileType.Empty).ToList();
        //Ordina le mosse per valore decrescente, in modo da avere prima le mosse più vantaggiose.
        possibleMoves = possibleMoves.OrderByDescending(x => x.value).ToList();
        if (possibleMoves.Count > 0)
        {
            shipMoves = possibleMoves;
            canMove = true;
        }

        //Chiamo la funzione per il display delle mosse nemiche

        //SwitchOnMovesVisualization(shipMoves);
        return canMove;

    }

    public override bool LookForAttacks()
    {
        canAttack = false;
        foreach (Move move in shipMoves)
        {
            //Controlla se le mosse di movimento calcolate in LookForMovement portano a posizioni di attacco, se si, aggiungi il valore della mossa
            //più distanza c'è col nemico, più il valore è alto

            //ricerca verso sx
            for (int x = move.GetTargetPos().x; x >= move.GetTargetPos().x - attackRange; x--)
            {
                if (x >= 0 && x < gridManager._width && x != move.GetTargetPos().x)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (gridManager.GetTileAtPosition(new Vector2Int(x, move.GetTargetPos().y))._type == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(x, move.GetTargetPos().y);

                        if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso dx
            for (int x = move.GetTargetPos().x; x <= move.GetTargetPos().x + attackRange; x++)
            {
                if (x >= 0 && x < gridManager._width && x != move.GetTargetPos().x)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (gridManager.GetTileAtPosition(new Vector2Int(x, move.GetTargetPos().y))._type == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(x, move.GetTargetPos().y);

                        if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso basso
            for (int y = move.GetTargetPos().y; y >= move.GetTargetPos().y - attackRange; y--)
            {
                if (y >= 0 && y < gridManager._height && y != move.GetTargetPos().y)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (gridManager.GetTileAtPosition(new Vector2Int(move.GetTargetPos().x, y))._type == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(move.GetTargetPos().x, y);

                        if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }

            //ricerca verso alto
            for (int y = move.GetTargetPos().y; y <= move.GetTargetPos().y - attackRange; y++)
            {
                if (y >= 0 && y < gridManager._height && y != move.GetTargetPos().y)
                {

                    //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                    if (gridManager.GetTileAtPosition(new Vector2Int(move.GetTargetPos().x, y))._type == TileType.Ally)
                    {
                        Vector2Int pos = new Vector2Int(move.GetTargetPos().x, y);

                        if (gridManager.GetTileAtPosition(pos).GetType() == TileType.Obstacle)
                        {
                            break;
                        }

                        if (Vector2Int.Distance(move.GetTargetPos(), pos) == attackRange)
                        {
                            move.value += 2;
                        }
                        else if (Vector2Int.Distance(move.GetTargetPos(), pos) < attackRange && Vector2Int.Distance(move.GetTargetPos(), pos) > 0)
                        {
                            move.value += 1;
                        }
                    }
                }
            }
        }

        //Aggiungi le mosse di attacco dalla posizioni attuale della nave, non quelle future, aggiungi il valore della mossa
        // più distanza c'è col nemico, più il valore è alto, valori così alti servono perché se la nave può già attaccare,
        // non ha senso muoverla, quindi il valore della mossa di attacco deve essere più alto di quello di movimento
        for (int x = position.x - shipSO.attackRange; x < position.x + shipSO.attackRange; x++)
        {
            if (x >= 0 && x < gridManager._width && x != position.x)
            {
                if (gridManager.GetTileAtPosition(new Vector2Int(x, position.y))._type == TileType.Ally)
                {
                    Vector2Int pos = new Vector2Int(x, position.y);
                    Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    if (Vector2Int.Distance(position, pos) == shipSO.attackRange)
                    {

                        newMove.value += 6;


                    }
                    else if (Vector2Int.Distance(position, pos) < shipSO.attackRange && Vector2Int.Distance(position, pos) > 0)
                    {
                        newMove.value += 5;
                    }
                    shipMoves.Add(newMove);
                }
            }
        }
        for (int y = position.y - shipSO.attackRange; y < position.y + shipSO.attackRange; y++)
        {
            if (y >= 0 && y < gridManager._height && y != position.y)
            {
                //Controlla se la tile è occupata da un nemico, se si, aggiungi il valore della mossa
                if (gridManager.GetTileAtPosition(new Vector2Int(position.x, y))._type == TileType.Ally)
                {
                    Vector2Int pos = new Vector2Int(position.x, y);
                    Move newMove = new Move(idMove++, shipName, pos, MessageType.attack, 0);
                    if (Vector2Int.Distance(position, pos) == shipSO.attackRange)
                    {

                        newMove.value += 6;
                    }
                    else if (Vector2Int.Distance(position, pos) < shipSO.attackRange && Vector2Int.Distance(position, pos) > 0)
                    {

                        newMove.value += 5;
                    }
                    shipMoves.Add(newMove);
                }
            }
        }



        
        //Ordina le mosse per valore decrescente, in modo da avere prima le mosse più vantaggiose.
        shipMoves = shipMoves.OrderByDescending(x => x.value).ToList();
        initialMove = shipMoves[0];
        if (shipMoves.Count > 0 && shipMoves.Where(x => x.GetMessageType() == MessageType.attack).ToList().Count > 0)
        {
            canAttack = true;

            //Chiamo la funzione per il display delle mosse nemiche

        }

        //gridManager.GetTileAtPosition(initialMove.GetTargetPos()).SetTileInteractable(faction, initialMove);
        //Debug.Log("Ship: " + shipName + " performs: " + initialMove.GetMessageType() + " on: " + initialMove.GetTargetPos());
        return canAttack;
    }


    //Metodo da invocare nel caso si volesse bloccare l'azione di una nave nemica

    public void DisableShip()
    {
        canAttack = false;
        canMove = false;
    }

    //Funzioni da utilizzare per la risoluzione degli effetti delle carte
    public void NegateAction()
    {
        canAttack = false;
        canMove = false;
        //invoco ora la funzione per non attendere la fine del turno

    }

    private void SwitchOffMovesVisualization(List<Move> shipMoves)
    {
        foreach (Move move in shipMoves)
        {
            gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileNotInteractable(faction);
        }
    }
    private void SwitchOnMovesVisualization(List<Move> shipMoves)
    {
        foreach (Move move in shipMoves)
        {
            gridManager.GetTileAtPosition(move.GetTargetPos()).SetTileInteractable(faction);
        }
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
