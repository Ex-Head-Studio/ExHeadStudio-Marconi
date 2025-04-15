using UnityEngine;

public interface IShipManager
{
    int NumberOfMessages{ get;}
    InfluenceMap InfluenceMap{get;}
    void InstantiateAllyShip(string shipName);
    void InstantiateEnemyShip(string shipName);
    void InstantiateInMap(string shipName);
    void ChooseShips();
}
