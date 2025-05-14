using UnityEngine;

[CreateAssetMenu(fileName = "ChangeClassEffect", menuName = "Scriptable Objects/Card Effects/Damage Effect")]
public class DamageEffect : AbstractEffectSO
{
    [SerializeField] public int damage;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
        {
            shipScript.TakeDamage(damage);
            EndEffect(0);
        }
    }
}
