using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class DisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;

    [Header("Generic Stat Prefab")]
    [SerializeField] private Transform statsParent; 
    [SerializeField] private GameObject genericStatPrefab;
    [SerializeField] private GameObject taccaPrefab;  


    [Header("Display parameters")]
    [SerializeField] private float waitTime = 0.5f;
    [SerializeField] private int fontSize = 3;
    private ShipSO shipSO;

    private void Update()
    {
        statsPanel.transform.LookAt(Camera.main.transform.position); // Make the stats panel face the camera
        statsPanel.transform.Rotate(0, 180, 0); // Rotate it to face the camera correctly
    }

    public void SetShipClass(ShipSO shipClass)
   {
        foreach (string statName in shipClass.statNames)
        {
            GameObject statObject = Instantiate(genericStatPrefab, statsParent, false);
            statObject.name = statName;
            statObject.GetComponentInChildren<TMP_Text>().text = statName;

            statObject.GetComponentInChildren<TMP_Text>().fontSize = fontSize;

            for(int i = 0; i < shipClass.statsDictionary[statName]; i++)
            {
                GameObject tacca = Instantiate(taccaPrefab, statsParent, false);
                tacca.name = "Tacca" + i;
                tacca.transform.localPosition = new Vector3(0, -i * 20, 0); // Adjust the position as needed
            }
        }
   }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(shipSO == null)
        {
            shipSO = GetComponent<AShip>().GetShipSO();
            SetShipClass(shipSO);
        }

        if (shipSO != null)
        {
            statsPanel.name = shipSO.name; // Set the name of the panel to the ship class name
            statsPanel.SetActive(false); // Initially hide all panels
        }
        StartCoroutine(WaitBeforeShowingStats(waitTime));
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        statsPanel.SetActive(false);
    }

    private IEnumerator WaitBeforeShowingStats(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        statsPanel.SetActive(true);
    }


}
