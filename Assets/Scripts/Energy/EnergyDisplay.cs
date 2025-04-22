using System.Collections.Generic;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private GameObject energyBarPrefab;
    [SerializeField] private Transform energyBarContainer; // Parent object for energy bars

    private List<GameObject> energyBars = new List<GameObject>();



    private void Start()
    {
        if(energySystem == null)
        {
            Debug.LogError("EnergySystem is not assigned in the inspector.");
            return;
        }

        //l'offset serve perchè a inizio gioco conto anche il primo turno
        UpdateEnergyDisplay(energySystem.defaultEnergy-1);
    }

    public void AddTurnEnergy()
    {
        UpdateEnergyDisplay(energySystem.energyPerTurn);
    }

    private void UpdateEnergyDisplay(int amount)
    {
        energySystem.AddEnergy(amount);
        for(int i = 0; i < amount; i++)
        {
            Instantiate(energyBarPrefab, energyBarContainer);
            energyBars.Add(energyBarPrefab);
        }
    }

    public void RemoveEnergy(int amount)
    {
        //questa funzione deve restare fuori dal ciclo
        energySystem.RemoveEnergy(amount);

            //implementare un controllo più corretto e rifattorizzare se serve
            if(energyBarContainer.childCount > 0 && amount < energyBarContainer.childCount)
            {
                for(int i = amount-1; i >= 0; i--)
                {
                    energyBars.RemoveAt(i);
                    //mi piacerebbe fare una piccola animazione di distruzione
                    Destroy(energyBarContainer.GetChild(i).gameObject, 0.5f);
                }
            }
            else if(amount > energyBarContainer.childCount)
            {
                for(int i = energyBarContainer.childCount-1; i >= 0; i--)
                {
                    energyBars.RemoveAt(i);
                    Destroy(energyBarContainer.GetChild(i).gameObject);
                }
             }
        
    }
}
