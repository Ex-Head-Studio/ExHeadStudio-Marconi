using TMPro;
using UnityEngine;

public class ClassStatsScript : MonoBehaviour
{
    [SerializeField] private GameObject genericStatPrefab;
    [SerializeField] private Transform statsParent; 
    [SerializeField] private GameObject taccaPrefab;     
   public void SetShipClass(ShipSO shipClass)
   {
        foreach (string statName in shipClass.statNames)
        {
            GameObject statObject = Instantiate(genericStatPrefab, statsParent);
            statObject.name = statName;
            statObject.GetComponentInChildren<TMP_Text>().text = statName;
            for(int i = 0; i < shipClass.statsDictionary[statName]; i++)
            {
                GameObject tacca = Instantiate(taccaPrefab, statsParent);
                tacca.name = "Tacca" + i;
                tacca.transform.localPosition = new Vector3(0, -i * 20, 0); // Adjust the position as needed
            }
        }
   }
}
