using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private EventInstance ambianceEventInstance;
    [field: Header("Boat Control Ambience")]
    [field: SerializeField] public EventReference controlAmbience { get; private set; }


    //private EventInstance musicInstance;
    //[field: Header("Music")]
    //[field: SerializeField] public EventReference music { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;
    }
    public static void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private void Start()
    {
       InitializeAmbience(controlAmbience);
    }
    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambianceEventInstance = RuntimeManager.CreateInstance(ambienceEventReference);
        if (!ambianceEventInstance.isValid())
        {
            UnityEngine.Debug.LogError("Ambience event not found");
            return;
        }
        ambianceEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        ambianceEventInstance.start();
    }

    //private void InitializeMusic(EventReference eventReference)
    //{
    //    musicInstance = RuntimeManager.CreateInstance(eventReference);
    //    musicInstance.start();
    //}

    //public void SetMusicState(MusicState state)
    //{
    //    musicInstance.setParameterByName("Stato Musica", (float)state);
    //}

}
