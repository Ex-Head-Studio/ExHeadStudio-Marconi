using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CreateAssetMenu(fileName = "Attack Effect", menuName = "Scriptable Objects/Card Effects/Attack Effect")]
public class AttackEffect : AbstractEffectSO
{

    private AllyShip shipScript;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if(effectStruct.obj.TryGetComponent<AllyShip>(out shipScript))
        {
            shipScript.ReceiveEffect(this);
            shipScript.LookForAttacks();
        }
    }
}