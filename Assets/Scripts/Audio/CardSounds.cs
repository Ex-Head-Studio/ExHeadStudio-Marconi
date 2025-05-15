using UnityEngine;

public class CardSounds: MonoBehaviour
{
    private FMOD.Studio.EventInstance cardSelection;

    public void PlayCardSelection()
    {
        cardSelection = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardSelection");
        cardSelection.start();
        cardSelection.release();
    }

    private FMOD.Studio.EventInstance cardDrop;

    public void PlayCardDrop()
    {
        cardDrop = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardDrop");
        cardDrop.start();
        cardDrop.release();
    }

    private FMOD.Studio.EventInstance cardError;

    public void PlayCardError()
    {
        cardError = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/CardError");
        cardError.start();
        cardError.release();
    }


}
