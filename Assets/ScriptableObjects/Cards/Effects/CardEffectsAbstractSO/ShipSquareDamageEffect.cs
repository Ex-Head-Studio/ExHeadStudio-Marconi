using UnityEngine;

[CreateAssetMenu(fileName = "ShipSquareDamageEffect", menuName = "Scriptable Objects/Card Effects/ShipSquareDamageEffect")]
public class ShipSquareDamageEffect : AbstractEffectSO
{
    public override void PerformEffect(EffectStruct effectStruct)
    {
        effectStruct.obj.GetComponent<AllyShip>().SquareDamage();
        EndEffect(0);
    }
}