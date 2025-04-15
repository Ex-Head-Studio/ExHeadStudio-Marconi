using UnityEngine;


[CreateAssetMenu(fileName = "ShipDestroyedEvent", menuName = "Events/Ship Destroyed Event")]
public class OnShipDestroyedEvent : AbstractEventSO<ShipDestroyedStruct>
{
}

[System.Serializable]
public struct ShipDestroyedStruct
{
    public string shipName;
    public int entity;
    public Vector2 gridPosition;
    public AShip shipScript;
    
    /// <summary>
    /// Class which encapsulates all the data of destroyed ship
    /// <param name="shipName"></param>
    /// <param name="entity"></param>
    /// <param name="gridPosition"></param>
    /// <param name="shipScript"></param> <summary>
    public ShipDestroyedStruct(string shipName, int entity, Vector2 gridPosition, AShip shipScript)
    {
        this.shipName = shipName;
        this.entity = entity;
        this.gridPosition = gridPosition;
        this.shipScript = shipScript;
    }
}
