using System.Collections.Generic;
using UnityEngine;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject healthBarCanvasPrefab;

    private Ship shipScript;
    private Vector2Int position;

    private int healthBarCount = 0;
    private List<GameObject> healthObjectsList = new List<GameObject>();
    private GameObject healthBarInstance;

    private void Start()
    {

        shipScript = GetComponent<Ship>();
        if (shipScript != null && healthBarPrefab != null)
        {
            healthBarCount = shipScript.GetHealth();
            for(int i = 0; i < healthBarCount; i++)
            {
                healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, healthBarCanvasPrefab.transform);
                healthObjectsList.Add(healthBarInstance);
            }
            
        }
    }


    //funzione da chiamare quando la nave subisce danni, in concomitanza con l'evento
    public void UpdateHealthBar(ShipAttackStruct shipAttackStruct)
    {
        if (healthBarPrefab != null && healthBarCanvasPrefab != null 
            && shipAttackStruct.gridPosition == shipScript.GetPosition())
        {
            healthBarCount--;
            //distruggo l'health bar
            Destroy(healthObjectsList[healthBarCount]);
            healthObjectsList.RemoveAt(healthBarCount);
        }
    }


}
