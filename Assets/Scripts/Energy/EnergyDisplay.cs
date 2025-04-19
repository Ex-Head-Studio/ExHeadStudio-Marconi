using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private GameObject energyBarPrefab;
    [SerializeField] private Transform energyBarContainer; // Parent object for energy bars

    private void Start()
    {
        if(energySystem == null)
        {
            Debug.LogError("EnergySystem is not assigned in the inspector.");
            return;
        }
        for(int i = 0; i < energySystem.defaultEnergy; i++)
        {
            GameObject energyBar = Instantiate(energyBarPrefab, energyBarContainer);
        }
    }

    public void AddTurnEnergy()
    {
        energySystem.AddEnergy(energySystem.energyPerTurn);
        UpdateEnergyDisplay(energySystem.energyPerTurn);
    }

    private void UpdateEnergyDisplay(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Instantiate(energyBarPrefab, energyBarContainer);
        }
    }
}
