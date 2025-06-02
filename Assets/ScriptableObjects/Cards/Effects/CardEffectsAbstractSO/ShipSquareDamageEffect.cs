using UnityEngine;

[CreateAssetMenu(fileName = "ShipSquareDamageEffect", menuName = "Scriptable Objects/Card Effects/ShipSquareDamageEffect")]
public class ShipSquareDamageEffect : AbstractEffectSO
{

    [SerializeField] private int damageRange = 1;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        effectStruct.obj.GetComponent<AllyShip>().SquareDamage(damageRange);
        EndEffect(0);
    }
}