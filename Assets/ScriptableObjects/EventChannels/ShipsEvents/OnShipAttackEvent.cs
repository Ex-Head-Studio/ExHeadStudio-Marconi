using UnityEngine;

[CreateAssetMenu(fileName = "ShipAttackedEvent", menuName = "Events/Ship Attacked Event")]
public class OnShipAttackEvent : AbstractEventSO<ShipAttackStruct>
{
    
}

[System.Serializable]
public struct ShipAttackStruct
{
    //la struct contiene le coordinate della nave attaccata
    public Vector2 gridPosition;

    public ShipAttackStruct(Vector2 gridPosition)
    {
        this.gridPosition = gridPosition;
    }
}
