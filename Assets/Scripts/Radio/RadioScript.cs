using UnityEngine;

public class RadioScript : MonoBehaviour
{
    [SerializeField] private RadioData radioData;

    private int radioIndex = 0;
    public void ChangeClip()
    {
        radioIndex = radioIndex  % radioData.radioEventReferences.Length;
        FMOD.Studio.EventInstance radioInstance = FMODUnity.RuntimeManager.CreateInstance(radioData.radioEventReferences[radioIndex]);
        radioIndex++;
    }
}
