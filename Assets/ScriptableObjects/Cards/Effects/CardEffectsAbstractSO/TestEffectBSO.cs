using UnityEngine;

[CreateAssetMenu(fileName = "TestEffectBSO", menuName = "Scriptable Objects/Card Effects/Test Effect B")]
public class TestEffectBSO : AbstractEffectSO
{
    public override void PerformEffect(EffectStruct effectStruct)
    {
        effectStruct.obj.transform.localScale = effectStruct.obj.transform.localScale * 4f ;
    }
}
