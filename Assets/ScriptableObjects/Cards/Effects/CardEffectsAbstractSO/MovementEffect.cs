using UnityEngine;

[CreateAssetMenu(fileName = "Movement Effect", menuName = "Scriptable Objects/Card Effects/Movement Effect")]
public class MovementEffect : AbstractEffectSO
{

    private AllyShip shipScript;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if(effectStruct.obj.TryGetComponent<AllyShip>(out shipScript))
        {
            shipScript.ReceiveEffect(this);
            shipScript.LookForMovement();
        }
    }
}
