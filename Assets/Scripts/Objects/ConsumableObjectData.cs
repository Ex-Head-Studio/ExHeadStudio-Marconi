using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableObjectData", menuName = "Scriptable Objects/ConsumableObjectData")]
public class ConsumableObjectData : AbstractObjectDataSO
{
   [SerializeField] public DropEntity dropEntity;

    public int GetEntityDrop()
    {
        return (int)dropEntity;
    }   
}
