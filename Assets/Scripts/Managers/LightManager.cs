using UnityEngine;
using System.Collections.Generic;

public class LightManager : MonoBehaviour
{
    [Tooltip("Nelle due liste devono essere inserite le luci associate alla navi")]
    [SerializeField] private List<Light> allyLights = new List<Light>();
    [SerializeField] private List<Light> enemyLights = new List<Light>();

    private int index;

    //public GameObject lightPrefab;
    void Start()
    {
        foreach(Light light in allyLights)
        {
            light.enabled = true;
        }
        foreach(Light light in enemyLights)
        {
            light.enabled = true;
        }
    }


    //TODO valutare se questo metodo serve ancora. Se si, come settarlo correttamente con il numero di navi
    //TODO e come istanziare le luci sulla plancia
    void GenerateLights() 
    {
        /*
        int lightCount = 3; // Con la funzione GetLightCount() si può ottenere il numero di luci presenti nella scena
        Vector3 startPosition = Vector3.zero;
        Vector3 offset = new Vector3(10, 0, 0); // L'offset può dipendere dal numero delle navi
        for (int i = 0; i < lightCount; i++) {
            GameObject light = Instantiate(lightPrefab, startPosition + i * offset, Quaternion.identity, transform);
            _lights.Add(light.GetComponentInChildren<Light>());
        }*/
    }
    public void OnShipDestroyedSwitchLight(ShipDestroyedStruct shipDestroyed) 
    {
        Debug.Log("Ship destroyed, light manager registered");
        if(shipDestroyed.entity == (int)Entity.ally)
        {
            index = allyLights.Count;
            if(index > 0)
            {
                allyLights[index-1].enabled = false;
                allyLights.RemoveAt(index-1);
            }

        }
        else
        {
            int index = enemyLights.Count;
            if(index > 0)
            {
            enemyLights[index-1].enabled = false;
            enemyLights.RemoveAt(index-1);
            }
        }
    }
}
