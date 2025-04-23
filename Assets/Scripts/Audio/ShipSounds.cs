using UnityEngine;

public class ShipSounds: MonoBehaviour
{
    private FMOD.Studio.EventInstance shipDestroy;

    public void PlayShipDestroy()
    {
        shipDestroy = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ShipExploding");
        shipDestroy.start();
        shipDestroy.release();
    }

    private FMOD.Studio.EventInstance shipMove;
    public void PlayShipMove()
    {
        shipMove = FMODUnity.RuntimeManager.CreateInstance("event:/UI/ShipMove");
        shipMove.start();
        shipMove.release();
    }
    
}
