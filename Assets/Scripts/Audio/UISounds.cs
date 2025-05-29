using UnityEngine;

public class UISounds: MonoBehaviour
{
    private FMOD.Studio.EventInstance bigButton;

    public void PlayPhaseButton()
    {
        bigButton = FMODUnity.RuntimeManager.CreateInstance("event:/UI/BigButton");
        bigButton.start();
        bigButton.release();
    }

    private FMOD.Studio.EventInstance energySound;

    public void PlayEnergySound()
    {
        energySound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Energy");
        energySound.start();
        energySound.release();
    }
}
