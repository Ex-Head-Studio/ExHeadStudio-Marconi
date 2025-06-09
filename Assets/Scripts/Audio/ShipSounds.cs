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
        shipMove = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ShipMoving");
        shipMove.start();
        shipMove.release();
    }

    private FMOD.Studio.EventInstance shipMoveStart;

    public void PlayMoveStartSound()
    {
        shipMoveStart = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/MovementStart");
        shipMoveStart.start();
        shipMoveStart.release();
    }

    private FMOD.Studio.EventInstance shipDamage;

    public void PlayShipDamage()
    {
        shipDamage = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ShipDamage");
        shipDamage.start();
        shipDamage.release();
    }
}
