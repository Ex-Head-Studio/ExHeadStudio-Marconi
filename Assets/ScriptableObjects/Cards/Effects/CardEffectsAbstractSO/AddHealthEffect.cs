using UnityEngine;

[CreateAssetMenu(fileName = "Add Health Effect", menuName = "Scriptable Objects/Card Effects/Add Health Effect")]
public class AddHealthEffect : AbstractEffectSO
{
    [SerializeField] private int healthToAdd = 1;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<AllyShip>(out AllyShip shipScript))
        {
            shipScript.AddHealth(healthToAdd);
            EndEffect(0);
        }
    }
}
