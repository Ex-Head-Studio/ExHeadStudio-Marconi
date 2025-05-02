using UnityEngine;
using System.Collections.Generic;   

[CreateAssetMenu(fileName = "ChangeClassEffect", menuName = "Scriptable Objects/Card Effects/ChangeClassEffect")]
public class ChangeClassEffect : AbstractEffectSO
{
    [SerializeField] private List<ShipSO> shipClasses = new List<ShipSO>();
    public override void PerformEffect(EffectStruct effectStruct)
    {
        ShipSO newClass = shipClasses[Random.Range(0, shipClasses.Count)];
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
        {
            shipScript.ChangeClass(newClass);
            EndEffect(0);
        }
    }
}
