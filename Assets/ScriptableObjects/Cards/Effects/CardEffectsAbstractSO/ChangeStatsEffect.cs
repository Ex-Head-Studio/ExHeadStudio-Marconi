using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatsEffect", menuName = "Scriptable Objects/Card Effects/ChangeStatsEffect")]
public class ChangeStatsEffect : AbstractEffectSO
{
    [HideInInspector]
    public ShipSO shipSO;
    [HideInInspector]
    public int arrayIdx;

    //VOGLIO CAMBIARE QUESTA COSA
    [SerializeField] private string statName;
    [SerializeField] public int amount;

    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
        {
            shipScript.ChangeStat(statName, amount);
            EndEffect(0);
        }
    }
}
