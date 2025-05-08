using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class RadioScript : MonoBehaviour, IPointerClickHandler
{
    [field: Header("Radio Sound")]  
    [field: SerializeField] public EventReference radioSound { get; private set; }
    private float radioKnobValue = 0f;
    private FMOD.Studio.EventInstance radioInstance;

    private void Start()
    {
       PlayRadio();
    }

    public void PlayRadio()
    {
        radioInstance = FMODUnity.RuntimeManager.CreateInstance(radioSound);
        radioInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        radioInstance.start();
        radioInstance.release();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        radioKnobValue += 0.5f; 
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("RadioKnob", radioKnobValue);
        UnityEngine.Debug.Log("Manopola cazzo" + radioKnobValue);
    }
}