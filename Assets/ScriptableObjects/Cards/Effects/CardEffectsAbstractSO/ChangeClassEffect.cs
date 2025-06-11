using UnityEngine;
using System.Collections.Generic;   

[CreateAssetMenu(fileName = "ChangeClassEffect", menuName = "Scriptable Objects/Card Effects/ChangeClassEffect")]
public class ChangeClassEffect : AbstractEffectSO
{
    private enum ClassSelectionMode
    {
        random, 
        chooseByPlayer
    }
    [SerializeField] private List<ShipSO> shipClasses = new List<ShipSO>();
    [SerializeField] private ClassSelectionMode classSelectionMode;
    
    [SerializeField] private ParticleSystem changeClassParticle;
    private ParticleSystem changeClassParticleInstance;
    public override void PerformEffect(EffectStruct effectStruct)
    {


        switch (classSelectionMode)
        {
            case ClassSelectionMode.random:
                if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
                {
                    ShipSO newClass = shipClasses[Random.Range(0, shipClasses.Count)];
                    shipScript.ChangeClass(newClass);
                    changeClassParticleInstance = Instantiate(changeClassParticle, shipScript.transform.position, Quaternion.identity);
                    changeClassParticleInstance.Play();
                    PlayClassChange();
                }

                break;

            case ClassSelectionMode.chooseByPlayer:

                //capire come faccio a visualizzarlo???


                break;


            default:
                break;
        }
        

        EndEffect(0);
    }
private FMOD.Studio.EventInstance soundChange;

public void PlayClassChange()
    {
        soundChange = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/ClassChange");
        soundChange.start();
        soundChange.release();
    }
}



