using UnityEngine;

[CreateAssetMenu(fileName = "Attack Effect", menuName = "Scriptable Objects/Card Effects/AttackInLineEffect")]
public class AttackInLineEffect : AbstractEffectSO
{
    private AllyShip shipScript;
    
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if(effectStruct.obj.TryGetComponent<AllyShip>(out shipScript))
        {
            shipScript.ReceiveEffect(this);
            shipScript.LookForAttackInLine();
        }
        else
        {
            Debug.LogError("L'oggetto non è una AllyShip: " + effectStruct.obj.name);
            EndEffect(1); // Termina con errore
        }
    }
}