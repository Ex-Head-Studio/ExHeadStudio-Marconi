using UnityEngine;

public class CardSounds : MonoBehaviour
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

    private FMOD.Studio.EventInstance cardHeal;

    public void PlayHeal()
    {
        cardHeal = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Heal");
        cardHeal.start();
        cardHeal.release();
    }

    private FMOD.Studio.EventInstance soundChange;

    public void PlayClassChange()
    {
        soundChange = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/ClassChange");
        soundChange.start();
        soundChange.release();
    }

    private FMOD.Studio.EventInstance soundObstacle;

    public void PlayObstacle()
    {
        soundObstacle = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Obstacle");
        soundObstacle.start();
        soundObstacle.release();
    }

    private FMOD.Studio.EventInstance mineSound;

    public void PlayMineExplosion()
    {
        mineSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Mine");
        mineSound.start();
        mineSound.release();
    }

    private FMOD.Studio.EventInstance cardStats;

    public void PlayCardStats()
    {
        cardStats = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Stats");
        cardStats.start();
        cardStats.release();
    }

    private FMOD.Studio.EventInstance meteorSound;

    public void PlayMeteorSound()
    {
        meteorSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Meteor");
        meteorSound.start();
        meteorSound.release();
    }
}
