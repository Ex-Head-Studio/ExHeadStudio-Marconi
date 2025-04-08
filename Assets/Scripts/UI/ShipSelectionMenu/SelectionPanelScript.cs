using UnityEngine;

public class SelectionPanelScript : MonoBehaviour
{
    [SerializeField] private ShipSO[] shipClasses;  
    [SerializeField] private GameObject classPanelPrefab; 
    [SerializeField] private Transform classPanelParent; 

    private void Start()
    {
        // Create a panel for each ship class
        foreach (ShipSO shipClass in shipClasses)
        {
            GameObject classPanel = Instantiate(classPanelPrefab, classPanelParent);
            ClassStatsScript classStatsScript = classPanel.GetComponent<ClassStatsScript>();
            classStatsScript.SetShipClass(shipClass);
            classPanel.name = shipClass.name; // Set the name of the panel to the ship class name
        }
        
    }


}
