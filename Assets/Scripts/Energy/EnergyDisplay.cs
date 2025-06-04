using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro; // Aggiungiamo il namespace per TextMeshPro

public class EnergyDisplay : MonoBehaviour
{
    private enum EnergyRechargeType
    {
        FullIncrement,
        PartialIncrement
    }

    [SerializeField] EnergyRechargeType energyRechargeType = EnergyRechargeType.PartialIncrement;
    [SerializeField] private EnergySystem energySystem;
    
    [Header("Energy Lights")]
    [SerializeField] private GameObject[] energyLights = new GameObject[6]; // Array fisso di 6 luci
    
    [Header("Energy Dial")]
    [SerializeField] private Transform dialTransform; // La manopola che ruota
    [SerializeField] private float rotationDuration = 1f; // Durata dell'animazione di rotazione
    [SerializeField] private float degreesPerEnergyUnit = -60f; // Gradi di rotazione per unità di energia

    [Header("Energy Text")]
    [SerializeField] private TextMeshProUGUI energyText; // Riferimento al testo che mostra l'energia
    [SerializeField] private string energyTextFormat = "{0}/{1}"; // Formato del testo (es. "3/6")
   
    private void Start()
    {
        if(energySystem == null)
        {
            Debug.LogError("EnergySystem is not assigned in the inspector.");
            return;
        }

        energySystem.InizializeValues();
        
        // Inizializza le luci in base all'energia iniziale
        int initialEnergy = energyRechargeType == EnergyRechargeType.PartialIncrement ? 
                            energySystem.defaultEnergy : energySystem.maxEnergy;
        
        UpdateLightsDisplay(initialEnergy);
        UpdateEnergyText(initialEnergy); // Aggiorniamo anche il testo
        
        // Imposta immediatamente la rotazione corretta della manopola senza animazioni
        if (dialTransform != null)
        {
            // Calcola l'angolo in base all'energia iniziale
            float baseAngle = -180f;
            float degPerUnit = Mathf.Abs(degreesPerEnergyUnit);
            float initialAngle = baseAngle + (initialEnergy * degPerUnit);
            
            // Applica la rotazione istantaneamente senza animazione, usando Y=0 come richiesto
            dialTransform.localRotation = Quaternion.Euler(0f, 0f, initialAngle);
        }
        
        // Imposta l'energia iniziale nel sistema
        if (energyRechargeType == EnergyRechargeType.PartialIncrement)
        {
            energySystem.SetDefaultEnergy();
        }
        else
        {
            energySystem.ResetEnergy();
        }
    }


    // Ruota la manopola da un valore di energia a un altro
    private void RotateDial(int fromEnergy, int toEnergy)
    {
        PlayEnergySound();
        if (dialTransform == null) return;

        // Per rotazione antioraria, l'angolo deve aumentare quando l'energia aumenta
        // Partiamo da -180 e aggiungiamo gradi positivi
        float baseAngle = -180f;

        // Assicuriamoci che degreesPerEnergyUnit sia positivo per la rotazione antioraria
        float degPerUnit = Mathf.Abs(degreesPerEnergyUnit);

        // Calcola l'angolo finale (più energia = più gradi aggiunti, rotazione antioraria)
        float toAngle = baseAngle + (toEnergy * degPerUnit);

        // IMPORTANTE: Preserva le rotazioni X e Y attuali, modifica solo Z
        Vector3 currentRotation = dialTransform.localEulerAngles;

        // Crea un oggetto DOTween che modifica SOLO l'angolo Z
        DOTween.To(
            () => dialTransform.localEulerAngles.z,  // Getter: valore attuale di Z
            (newZAngle) =>
            {
                // Setter: aggiorna solo Z, mantiene X e Y invariati
                dialTransform.localEulerAngles = new Vector3(
                    currentRotation.x,
                    currentRotation.y,
                    newZAngle
                );
            },
            toAngle,  // Valore target per Z
            rotationDuration  // Durata dell'animazione
        ).SetEase(Ease.OutBack);

        // Riproduci il suono di energia quando la manopola ruota
        
    }


    // Metodo dedicato per aggiornare il testo dell'energia
    private void UpdateEnergyText(int currentEnergy)
    {
        if (energyText != null)
        {
            energyText.text = string.Format(energyTextFormat, currentEnergy, energySystem.maxEnergy);
        }
    }
    
    public void AddTurnEnergy(VoidEvent numberOfRound)
    {
        switch (energyRechargeType)
        {
            case EnergyRechargeType.FullIncrement:
                if (numberOfRound.value == 0)
                {
                    // All'inizio del gioco, non facciamo niente perché è già stato gestito in Start()
                    break;
                }

                // Ripristina tutta l'energia all'inizio del turno
                UpdateLightsDisplay(energySystem.maxEnergy);
                RotateDial(energySystem.currentEnergy, energySystem.maxEnergy);
                energySystem.ResetEnergy();
                UpdateEnergyText(energySystem.maxEnergy); // Aggiungiamo l'aggiornamento del testo
                break;
                
            case EnergyRechargeType.PartialIncrement:
                if (numberOfRound.value == 0)
                {
                    // All'inizio del gioco, non facciamo niente perché è già stato gestito in Start()
                    break;
                }
                
                // Calcola l'energia dopo l'incremento
                int prevEnergy = energySystem.currentEnergy;
                int newEnergy = Mathf.Min(energySystem.currentEnergy + energySystem.energyPerTurn, energySystem.maxEnergy);
                
                // Aggiorna le luci per mostrare l'energia aggiunta
                UpdateLightsDisplay(newEnergy);
                UpdateEnergyText(newEnergy); // Aggiungiamo l'aggiornamento del testo
                
                // Ruota la manopola indietro (meno rotazione = più energia)
                RotateDial(prevEnergy, newEnergy);
                
                // Aggiungi energia per turno
                energySystem.AddEnergy(energySystem.energyPerTurn);
                break;
        }
    }
   
    public void RemoveEnergy(int amount)
    {
        if (amount <= 0) return;

        int prevEnergy = energySystem.currentEnergy;

        // Rimuovi l'energia dal sistema
        energySystem.RemoveEnergy(amount);

        // Spegni le luci direttamente, senza effetti
        UpdateLightsDisplay(energySystem.currentEnergy);
        UpdateEnergyText(energySystem.currentEnergy); // Aggiungiamo l'aggiornamento del testo

        // Ruota la manopola in avanti (più rotazione = meno energia)
        RotateDial(prevEnergy, energySystem.currentEnergy);
    }

    // Imposta lo stato di una luce (accesa/spenta)
    private void SetLightActive(GameObject light, bool active)
    {
        light.SetActive(active);
    }
    
    // Aggiorna tutte le luci in base all'energia corrente
    private void UpdateLightsDisplay(int energyAmount)
    {
        for (int i = 0; i < energyLights.Length; i++)
        {
            SetLightActive(energyLights[i], i < energyAmount);
        }
    }

    // Suono di energia
    private FMOD.Studio.EventInstance energySound;

    public void PlayEnergySound()
    {
        energySound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Energy");
        energySound.start();
        energySound.release();
    }
}