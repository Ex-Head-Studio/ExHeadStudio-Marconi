using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject healthBarCanvasPrefab;

    private AShip shipScript;
    private Vector2Int position;

    private int healthBarCount = 0;
    private List<GameObject> healthObjectsList = new List<GameObject>();
    private GameObject healthBarInstance;

    private void Start()
    {

        shipScript = GetComponent<AShip>();
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

    public void AddHealth(int amount)
    {
        healthBarCount += amount;
        for (int i = 0; i < amount; i++)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, healthBarCanvasPrefab.transform);
            healthObjectsList.Add(healthBarInstance);
        }
    }


    //funzione da chiamare quando la nave subisce danni, in concomitanza con l'evento
    public void UpdateHealthBar(ShipAttackStruct shipAttackStruct)
    {
        if (healthBarPrefab != null && healthBarCanvasPrefab != null 
            && shipAttackStruct.gridPosition == shipScript.GetPosition())
        {

            for(int i = healthBarCount-1; i >= 0; i--)
            {

                healthObjectsList[i].transform.DOPunchScale(Vector3.one * 0.5f, 0.5f, 10, 1).OnKill(() => { healthObjectsList[i].transform.DOKill(true); });
                healthObjectsList.RemoveAt(i);
                Destroy(healthObjectsList[i]);
            }


            healthBarCount -= shipAttackStruct.damage;
        }
    }


}
