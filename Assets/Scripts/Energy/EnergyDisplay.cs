using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnergyDisplay : MonoBehaviour
{

    private enum EnergyRechargeType
    {
        FullIncrement,
        PartialIncrement
    }

    [SerializeField] EnergyRechargeType energyRechargeType = EnergyRechargeType.PartialIncrement;
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

        energySystem.InizializeValues();
    }

    public void AddTurnEnergy(VoidEvent numberOfRound)
    {

        switch (energyRechargeType)
        {
            case EnergyRechargeType.FullIncrement:

                if (numberOfRound.value == 0)
                {
                    UpdateEnergyDisplay();
                    energySystem.ResetEnergy();
                    break;
                }

                //l'ordine di chiamata è importante, prima si aggiorna la UI e poi si resetta l'energia
                UpdateEnergyDisplay();
                energySystem.ResetEnergy();
                break;
                
            case EnergyRechargeType.PartialIncrement:
                if (numberOfRound.value == 0)
                {
                    UpdateEnergyDisplay();
                    energySystem.SetDefaultEnergy();
                    break;
                }
                UpdateEnergyDisplay(energySystem.energyPerTurn);
                energySystem.AddEnergy(energySystem.energyPerTurn);

                break;
        }

    }
    
    public void RemoveEnergy(int amount)
    {
        //questa funzione deve restare fuori dal ciclo
        energySystem.RemoveEnergy(amount);

            //distruggo gli elementi nella lista
            for(int i = 0; i < amount && energyBars.Count> 0; i++)
            {
                int index = energyBars.Count-1;

                energyBars[index].transform.DOShakePosition(0.5f, 0.1f, 10, 90, false, true).OnKill(() => {energyBars[index].transform.DOKill(true);});
                energyBars.RemoveAt(index);
                Destroy(energyBarContainer.GetChild(index).gameObject);
            }     
    }


    //metodo che istanzia la massima energia possibile
    //la prima differenza che viene eseguita impone che prima si aggiorni la UI e poi si resetti l'energia
    private void UpdateEnergyDisplay()
    {
        int energyDiff = energySystem.maxEnergy - energySystem.currentEnergy;
        for(int i = 0; i < energyDiff && energyBars.Count < energySystem.maxEnergy; i++)
        {
            Instantiate(energyBarPrefab, energyBarContainer);
            energyBars.Add(energyBarPrefab);
        }
    }

    //metodo che istanzia un certo numero di barre di energia
    private void UpdateEnergyDisplay(int amount)
    {
        for(int i = 0; i < amount && energyBars.Count <= energySystem.maxEnergy; i++)
        {
            Instantiate(energyBarPrefab, energyBarContainer);
            energyBars.Add(energyBarPrefab);
        }
    }




}
