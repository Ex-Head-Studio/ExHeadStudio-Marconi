using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class toggleSounds : MonoBehaviour
{
    private FMOD.Studio.EventInstance hoverMessage;

    private void Playhover()
    {
        hoverMessage = FMODUnity.RuntimeManager.CreateInstance("event:/UI/MessageHover");
        hoverMessage.start();
        hoverMessage.release();
    }

    private FMOD.Studio.EventInstance selectMessage;
    private void PlaySelect()
    {
        selectMessage = FMODUnity.RuntimeManager.CreateInstance("event:/UI/MessageSelection");
        selectMessage.start();
        selectMessage.release();
    }
}
