using UnityEngine;

[CreateAssetMenu(fileName = "Attack Effect", menuName = "Scriptable Objects/Card Effects/AttackNoObstaclesEffect")]
public class AttackNoObstaclesEffect : AbstractEffectSO
{
    private AllyShip shipScript;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if(effectStruct.obj.TryGetComponent<AllyShip>(out shipScript))
        {
            shipScript.ReceiveEffect(this);
            shipScript.LookForAttacksWithoutObstacles();
        }
    }
}
