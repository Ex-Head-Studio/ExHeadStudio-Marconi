using UnityEngine;

public class OnShipAttackEventListener : AbstractEventListenerSO<ShipAttackStruct>
{
    Ship ship;
    public void OnShipAttackEvent(ShipAttackStruct value)
    {
        if(value.gridPosition==ship.position)
            Debug.Log($"Ship at {value.gridPosition} has been attacked!");
    }
}

