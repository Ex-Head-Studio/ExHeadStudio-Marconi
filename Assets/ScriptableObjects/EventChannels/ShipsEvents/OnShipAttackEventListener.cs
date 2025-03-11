using UnityEngine;

public class OnShipAttackEventListener : AbstractEventSO<ShipAttackStruct>
{
    public void OnShipAttackEvent(ShipAttackStruct value)
    {
        Debug.Log($"Ship at {value.gridPosition} has been attacked!");
    }
}

