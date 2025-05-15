using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RadioScript : MonoBehaviour
{
    [Header("Radio Sound")]  
    [SerializeField] private EventReference radioSound;

    [Header("Knob Settings")]
    [SerializeField] private float knobSpeed = 1f;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 3f;
    [SerializeField] private AnimationCurve introCurve = AnimationCurve.Linear(0, 0, 1, 1); // curva solo per inizio intervalli

    private float radioKnobValue = 0f;
    private EventInstance radioInstance;

    [SerializeField] private Transform knobVisual;
[SerializeField] private float rotationMultiplier = 90f; // Gradi di rotazione per unità (puoi regolare)

    private void Start()
    {
        PlayRadio();
    }

    public void PlayRadio()
    {
        radioInstance = RuntimeManager.CreateInstance(radioSound);
        radioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        radioInstance.start();
        radioInstance.release(); // rilascio gestito da FMOD internamente
    }

    private void Update()
    {
        bool changed = false;
        float delta = 0f;

        // Controlli
        if (Input.GetKey(KeyCode.Y))
        {
            delta += knobSpeed * Time.deltaTime;
            changed = true;
        }

        if (Input.GetKey(KeyCode.T))
        {
            delta -= knobSpeed * Time.deltaTime;
            changed = true;
        }

        if (changed)
        {
            radioKnobValue += delta;

            // Loop continuo da 0 a 3
            float loopedValue = Mathf.Repeat(radioKnobValue, 3f);

            int station = Mathf.FloorToInt(loopedValue);
            float local = loopedValue - station;

            float finalValue = loopedValue;

            // Applica curva solo nei tratti [x, x + 0.1]
            if (local >= 0f && local <= 0.1f && station >= 0 && station <= 2)
            {
                float normalized = Mathf.InverseLerp(0f, 0.1f, local);
                float curved = delta >= 0f 
                    ? introCurve.Evaluate(normalized)
                    : introCurve.Evaluate(1f - normalized);

                finalValue = station + Mathf.Lerp(0f, 0.1f, curved);
            }

            RuntimeManager.StudioSystem.setParameterByName("RadioKnob", finalValue);
            Debug.Log("Valore Manopola (FMOD): " + finalValue.ToString("F3"));

            // Ruota il knob visivo
            // Ruota il knob visivo in modo continuo
            if (knobVisual != null)
            {
                float rotationX = (loopedValue / 3f) * 360f; // Mappatura 0–3 → 0–360°
                knobVisual.localRotation = Quaternion.Euler(-rotationX, 0f, 0f);
            }
        }
    }
}