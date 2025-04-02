using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableObjectData", menuName = "Scriptable Objects/ConsumableObjectData")]
public class ConsumableObjectData : AbstractObjectDataSO
{
   [SerializeField] private DropEntity dropEntity;

    public int GetEntityDrop()
    {
        return (int)dropEntity;
    }   
}
