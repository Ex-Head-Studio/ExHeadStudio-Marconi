using UnityEngine;
using System.Collections.Generic;

public class LightManager : MonoBehaviour {

    public GameObject lightPrefab;

    private List<Light> _lights = new List<Light>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        GenerateLights();
    }

    void GenerateLights() {

        int lightCount = 3; // Con la funzione GetLightCount() si può ottenere il numero di luci presenti nella scena
        Vector3 startPosition = Vector3.zero;
        Vector3 offset = new Vector3(10, 0, 0); // L'offset può dipendere dal numero delle navi
        for (int i = 0; i < lightCount; i++) {
            GameObject light = Instantiate(lightPrefab, startPosition + i * offset, Quaternion.identity, transform);
            _lights.Add(light.GetComponentInChildren<Light>());
        }
    }

    void OnShipDestroyed(ShipDestroyedStruct shipDestroyed) {
        Debug.Log("Ship destroyed");
        int index = _lights.Count;
        _lights[index].enabled = false;
        _lights.RemoveAt(index);
    }

    void GetLightCount(){
        // Get the number of lights in the scene
    }
}
