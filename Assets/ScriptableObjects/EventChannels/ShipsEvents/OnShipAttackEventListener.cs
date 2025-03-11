using UnityEngine;

public class OnShipAttackEventListener : AbstractEventListenerSO<ShipAttackStruct>
{
    public void OnShipAttackEvent(ShipAttackStruct value)
    {
        Debug.Log($"Ship at {value.gridPosition} has been attacked!");
    }
}

