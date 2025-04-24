using System.Collections;
using UnityEngine;

public interface IShipManager
{
    int NumberOfMessages{ get;}
    InfluenceMap InfluenceMap{get;}
    void InstantiateAllyShip(string shipName, int faction);
    void InstantiateEnemyShip(string shipName, int faction);
    void InstantiateInMap(string shipName);
    void EnemyMovesExecution();

    void EnemyMovesSelection();

    IEnumerator EndEnemyTurn();

}
