using UnityEngine;

public class OnShipDestroyedEventListener : AbstractEventListenerSO<ShipDestroyedStruct>
{
    public void OnShipDestroyed(ShipDestroyedStruct shipDestroyedStruct)
    {
        Debug.Log("Ship" + shipDestroyedStruct.shipName + "destroyed" + " at position: " + shipDestroyedStruct.gridPosition);
    }
}
