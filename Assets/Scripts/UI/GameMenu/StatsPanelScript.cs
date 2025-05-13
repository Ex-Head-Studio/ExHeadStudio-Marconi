using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelScript : MonoBehaviour
{
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;

    [Header("Generic Stat Prefab")]
    [SerializeField] private Transform statsParent; 
    [SerializeField] private GameObject genericStatPrefab;
    [SerializeField] private GameObject taccaPrefab;  


    [Header("Display parameters")]
    [SerializeField] private int fontSize = 3;
    [SerializeField] private float waitTimeBeforeShow = 0.5f;

    private HorizontalLayoutGroup horizontalLayoutGroup;

    private void OnEnable()
    {
        DisplayStats.OnShipOverStarted += ShowStatsPanel;
        DisplayStats.OnShipOverEnded += HideStatsPanel;
    }

    private void OnDisable()
    {
        DisplayStats.OnShipOverStarted -= ShowStatsPanel;
        DisplayStats.OnShipOverEnded -= HideStatsPanel;
    }
    private void ShowStatsPanel(ShipSO shipSO)
    {
        StartCoroutine(WaitBeforeShow(waitTimeBeforeShow, shipSO));

    }
    private void HideStatsPanel(ShipSO shipSO)
    {
        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetShipClass(ShipSO shipClass)
   {
        foreach (string statName in shipClass.statNames)
        {
            GameObject statObject = Instantiate(genericStatPrefab, statsParent, false);
            statObject.name = statName;
            statObject.GetComponentInChildren<TMP_Text>().text = statName;
            statObject.GetComponentInChildren<TMP_Text>().fontSize = fontSize;

            statObject.AddComponent<HorizontalLayoutGroup>();

            horizontalLayoutGroup = statObject.GetComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.spacing = 0.5f;
            horizontalLayoutGroup.padding.left = 10;
            horizontalLayoutGroup.padding.right = 10;


            for(int i = 0; i < shipClass.statsDictionary[statName]-1; i++)
            {
                GameObject tacca = Instantiate(taccaPrefab, statObject.transform, false);
                tacca.name = "Tacca" + i;
            }
        }
   }

   private IEnumerator WaitBeforeShow(float time, ShipSO shipSO)
   {
        yield return new WaitForSeconds(time);
        SetShipClass(shipSO);
   }

}
