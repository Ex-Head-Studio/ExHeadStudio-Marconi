using UnityEngine;

[CreateAssetMenu(fileName = "DenyEffect", menuName = "Scriptable Objects/Card Effects/DenyEffect")]
public class DenyEffectSO : AbstractEffectSO
{
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<EnemyShip>(out EnemyShip shipScript))
        {
            shipScript.NegateAction();
            EndEffect(0);
        }
    }   
}
