using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RadioScript : MonoBehaviour
{
    [Header("Radio Sound")]
    [SerializeField] private EventReference radioSound;
    [SerializeField] private EventReference knobSound;

    [Header("Knob Settings")]
    [SerializeField] private float knobSpeed = 1f;
    [SerializeField] private float rotationMultiplier = 90f;

    private float radioKnobValue = 0f;
    private EventInstance radioInstance;
    private EventInstance knobInstance;
    private bool isKnobPlaying = false;
    private float knobLastPlayTime = 0f;
    private float knobCooldown = 0.1f;

    [SerializeField] private Transform knobVisual;
        
    private void Start()
    {
        PlayRadio();
        knobInstance = RuntimeManager.CreateInstance(knobSound);
        knobInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayRadio()
    {
        radioInstance = RuntimeManager.CreateInstance(radioSound);
        radioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        radioInstance.start();
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

            // Gestione suono manopola
            if (!isKnobPlaying && Time.time - knobLastPlayTime > knobCooldown)
            {
                knobInstance.start();
                isKnobPlaying = true;
                knobLastPlayTime = Time.time;
            }

            // Loop continuo da 0 a 3
            float loopedValue = Mathf.Repeat(radioKnobValue, 3f);
            RuntimeManager.StudioSystem.setParameterByName("RadioKnob", loopedValue);

            // Ruota il knob visivo
            if (knobVisual != null)
            {
                float rotationX = (loopedValue / 3f) * 360f;
                knobVisual.localRotation = Quaternion.Euler(-rotationX, 0f, 0f);
            }
        }
        else if (isKnobPlaying)
        {
            // Ferma il suono quando la manopola smette di girare
            knobInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isKnobPlaying = false;
        }
    }

    private void OnDestroy()
    {
        radioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        radioInstance.release();
        knobInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        knobInstance.release();
    }
}