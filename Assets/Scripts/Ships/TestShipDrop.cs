using UnityEngine;

public class TestShipDrop : MonoBehaviour, Idroppable
{
    //TODO questa cosa deve essere modifcata, serve solo per fare test
    [SerializeField] private ObjectDropEvent dropEvent;
    [SerializeField] private AbstractObjectDataSO[] consumableObject;

    private Ship ship;
    private int index = 0;
    private AbstractObjectDataSO tmpObjectDataSO;
    private ConsumableObjectData tmpConsumableObjectDataSO;
 

    private void Start()
    {
        ship = GetComponent<Ship>();
    }

    //questo metodo preferisco lasciarlo separato dalla logica della nave
    //TODO qui manca tutta a logica probabilistica del drop
    private void  OnDestroy()
    {
        //index = Random.Range(0, consumableObject.Length);
        index = 0;
        tmpObjectDataSO = consumableObject[index];
        if(tmpObjectDataSO.GetType() == typeof(ConsumableObjectData))
        {
            tmpConsumableObjectDataSO = tmpObjectDataSO as ConsumableObjectData;
            if(tmpConsumableObjectDataSO.GetEntityDrop() == ship.faction)
            {
                //fino a qui voglio lavorare con i dati
                //qui devo inviare una classe?
                Debug.Log("Ho dropato un oggetto consumabile");
                ConsumableObject consumable =  new ConsumableObject(tmpConsumableObjectDataSO);
                DropConsumable(consumable);
                return;
            }

        }
    }

    public void DropConsumable(ConsumableObject consObj)
    {
        //TODO qui ci va la logica del drop, ora lo faccio solo per testare
        //l'evento deve essere ricevuto dalla UI, che lo visualizza sui pannelli necessari
        dropEvent.Invoke(consObj);
        Debug.Log("Dropped by the test ship");
    }

    public void DeathAnimation()
    {
        Debug.Log("Death animation called");
    }
}
