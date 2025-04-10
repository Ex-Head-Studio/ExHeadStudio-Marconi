using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro; // Ensure you have the TextMeshPro package installed and imported

public class SelectionPanelScript : MonoBehaviour
{
    [SerializeField] private ShipSO[] shipClasses;  
    [SerializeField] private GameObject classPanelPrefab; 
    [SerializeField] private Transform classPanelParent; 
    [SerializeField] private Transform modelPosition;

    private List<GameObject> classPanels = new List<GameObject>(); // List to keep track of instantiated panels
    private int currentClassIndex = 0; // Index to keep track of the current ship class

    private void Start()
    {
        // Create a panel for each ship class
        foreach (ShipSO shipClass in shipClasses)
        {
            GameObject classPanel = Instantiate(classPanelPrefab, classPanelParent);
            classPanel.GetComponentInChildren<TMP_Text>().text = shipClass.name; // Set the text of the panel to the ship class name
            classPanels.Add(classPanel); // Add the instantiated panel to the list
            ClassStatsScript classStatsScript = classPanel.GetComponent<ClassStatsScript>();
            classStatsScript.SetShipClass(shipClass);
            classPanel.name = shipClass.name; // Set the name of the panel to the ship class name
            classPanel.SetActive(false); // Initially hide all panels
            GameObject shipMesh = Instantiate(shipClass.shipModelMesh, modelPosition); // Instantiate the ship model at the specified position
            shipMesh.gameObject.transform.SetParent(classPanel.transform); // Set the parent of the ship model to the model position
        }
        
        classPanels[currentClassIndex].SetActive(true); // Show the first panel
    }

    public void NextClass()
    {
        classPanels[currentClassIndex].SetActive(false); // Hide the current panel
        currentClassIndex = (currentClassIndex + 1) % classPanels.Count; // Move to the next class, wrap around if at the end
        classPanels[currentClassIndex].SetActive(true); // Show the new panel
    }

    public void PreviousClass()
    {
        classPanels[currentClassIndex].SetActive(false); // Hide the current panel
        currentClassIndex = (currentClassIndex - 1 + classPanels.Count) % classPanels.Count; // Move to the previous class, wrap around if at the beginning
        classPanels[currentClassIndex].SetActive(true); // Show the new panel
    }

}
