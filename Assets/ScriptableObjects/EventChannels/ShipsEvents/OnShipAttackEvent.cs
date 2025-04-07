using UnityEngine;

[CreateAssetMenu(fileName = "ShipAttackedEvent", menuName = "Events/Ship Attacked Event")]
public class OnShipAttackEvent : AbstractEventSO<ShipAttackStruct>
{
    
}

[System.Serializable]
public struct ShipAttackStruct
{
    //la struct contiene le coordinate della nave attaccata
    //TODO questa forse non deve essere pubblica
    public Vector2 gridPosition;
    public int damage;

    public ShipAttackStruct(Vector2 gridPosition, int damage)
    {
        this.damage = damage;
        this.gridPosition = gridPosition;
    }
}
