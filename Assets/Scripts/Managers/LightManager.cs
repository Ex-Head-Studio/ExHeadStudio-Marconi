using UnityEngine;
using System.Collections.Generic;

public class LightManager : MonoBehaviour {

    //public GameObject lightPrefab;

    [SerializeField] private List<Light> allyLights = new List<Light>();
    [SerializeField] private List<Light> enemyLights = new List<Light>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        //GenerateLights();
    }

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
        /*if(shipDestroyed.entity == (int)Entity.ally)
        {
            int index = allyLights.Count;
            allyLights[index].enabled = false;
            allyLights.RemoveAt(index);
        }
        else
        {
            int index = enemyLights.Count;
            enemyLights[index].enabled = false;
            enemyLights.RemoveAt(index);
        }*/

    }
}
