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
    [SerializeField] private TMP_Text shipClassName = null;
    [SerializeField] private Image shipClassImageSprite = null;

    //vita
    [SerializeField] private TMP_Text shipHealth = null;

    //statistiche

    //effetti


    [Header("Display parameters")]
    [SerializeField] private int fontSize = 3;
    [SerializeField] private float waitTimeBeforeShow = 0.5f;

    //Classi da usare per convertire gli script passati dagli eventi
    AShip shipScript = null;
    AbstractObstacle obstacleScript = null;
    Tile tileScript = null;

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
            //qui l'ordine degli if è importante!!
            
            if (displayStats.script is AShip)
            {
                Debug.Log("Ho preso una nave");
                shipScript = (AShip)displayStats.script;
                //SetShipClass(displayStats.script.gameObject.GetComponent<AShip>().GetShipSO());
                shipClassName.text = shipScript.GetShipSO().name;
            }
            else if (displayStats.script is AbstractObstacle)
            {
                Debug.Log("Qui ho preso un ostacolo ");
                obstacleScript = (AbstractObstacle)displayStats.script;
            }
            else if (displayStats.script is Tile)
            {
                Debug.Log("Qui ho preso una tile");
                tileScript = (Tile)displayStats.script;

            }


            isDisplaying = true;
        }

    }
    private void HideStatsPanel(DisplayStatsClass displayStats)
    {
        foreach (Transform child in statsParent)
        {
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
