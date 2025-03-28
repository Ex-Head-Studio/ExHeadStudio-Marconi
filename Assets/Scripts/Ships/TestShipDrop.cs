using UnityEngine;

public class TestShipDrop : MonoBehaviour, Idroppable
{
    [SerializeField] private ObjectDropEvent dropEvent;

    //TODO questa cosa deve essere modifcata, serve solo per fare test
    [SerializeField] private ConsumableObject consumableObject;

    private Ship ship;

    private void Start()
    {
        ship = GetComponent<Ship>();
    }

    //questo metodo si può inserire direttamente nella logica della nave
    private void  OnDestroy()
    {
        if(consumableObject.GetEntityDrop() == ship.faction)
        {
            Drop();
        }
    }

    public void Drop()
    {
        dropEvent.Invoke(consumableObject);
        Debug.Log("Dropped by the test ship");
    }
}
