using System.Collections;
using TMPro;
using Unity.VisualScripting;
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

    [Header("Parameters to display")]

    //classe
    [SerializeField] private TMP_Text shipClassName;
    [SerializeField] private Image shipClassImageSprite;

    //vita
    [SerializeField] private TMP_Text shipHealth;

    //statistiche

    //effetti

    //immagine della classe
    [SerializeField] private Sprite shipClassImage;

    [Header("Display parameters")]
    [SerializeField] private int fontSize = 3;
    [SerializeField] private float waitTimeBeforeShow = 0.5f;

    private HorizontalLayoutGroup horizontalLayoutGroup;

    private bool isDisplaying = false;

    #region Iscrizione agli eventi
    private void OnEnable()
    {
        DisplayStats.OnEntityHoverStarted += ShowStatsPanel;
        DisplayStats.OnEntityHoverEnded += HideStatsPanel;
    }

    private void OnDisable()
    {
        DisplayStats.OnEntityHoverStarted -= ShowStatsPanel;
        DisplayStats.OnEntityHoverEnded -= HideStatsPanel;
    }

    #endregion

    #region  Metodi di display
    private void ShowStatsPanel(DisplayStatsClass displayStats)
    {
        if (!isDisplaying)
        {
            //mettere il discrimine qui
            isDisplaying = true;
        }

    }
    private void HideStatsPanel(DisplayStatsClass displayStats)
    {
        foreach (Transform child in statsParent)
        {
            //mettere il discrimine qui
            Destroy(child.gameObject);
        }
        isDisplaying = false;
    }

    private void SetShipClass(ShipSO shipClass)
   {
        shipClassName.text = shipClass.name;
        foreach (string statName in shipClass.statNames)
        {
            GameObject statObject = Instantiate(genericStatPrefab, statsParent, false);
            statObject.name = statName;
            statObject.GetComponentInChildren<TMP_Text>().text = statName;
            statObject.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
            if(statObject.GetComponent<HorizontalLayoutGroup>() == null)
            {
                statObject.AddComponent<HorizontalLayoutGroup>();
            }
            

            horizontalLayoutGroup = statObject.GetComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.spacing = 0.5f;
            horizontalLayoutGroup.padding.left = 10;
            horizontalLayoutGroup.padding.right = 10;


            for (int i = 0; i < shipClass.statsDictionary[statName] - 1; i++)
            {
                GameObject tacca = Instantiate(taccaPrefab, statObject.transform, false);
                tacca.name = "Tacca" + i;
            }
        }
   }
    #endregion
}
