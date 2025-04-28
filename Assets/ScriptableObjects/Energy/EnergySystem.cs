using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergySystem", menuName = "Scriptable Objects/EnergySystem")]
public class EnergySystem : ScriptableObject
{
    [SerializeField] public int defaultEnergy;
    [SerializeField] public int maxEnergy;
    [SerializeField] public int energyPerTurn = 2;

    public int currentEnergy;

    public void InizializeValues()
    {
        currentEnergy = 0;
    } 

    private void Start()
    {
        currentEnergy = defaultEnergy;
    }

    public void ResetEnergy()
    {
        currentEnergy = defaultEnergy;
    }

    public void SetDefaultEnergy()
    {
        currentEnergy = defaultEnergy;
    }

    public void AddEnergy(int amount)
    {
        
        currentEnergy += amount;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
    }
    public void RemoveEnergy(int amount)
    {
        currentEnergy -= amount;
        
        if (currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }
}
