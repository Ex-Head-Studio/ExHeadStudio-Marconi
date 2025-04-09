using UnityEngine;

public class RadioScript : MonoBehaviour
{
    [SerializeField] private RadioData radioData;

    private int radioIndex = 0;
    private FMOD.Studio.EventInstance currentRadioInstance;

    public void ChangeClip()
    {
        // Stop the current clip if it is playing
        if (currentRadioInstance.isValid())
        {
            currentRadioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentRadioInstance.release();
        }

        // Play the next radio clip
        radioIndex = radioIndex % radioData.radioEventReferences.Length;
        currentRadioInstance = FMODUnity.RuntimeManager.CreateInstance(radioData.radioEventReferences[radioIndex]);
        radioIndex++;
        currentRadioInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        currentRadioInstance.start();
    }
}