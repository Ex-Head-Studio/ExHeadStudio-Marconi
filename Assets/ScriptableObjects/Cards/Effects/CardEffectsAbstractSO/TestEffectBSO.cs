using UnityEngine;

[CreateAssetMenu(fileName = "TestEffectBSO", menuName = "Scriptable Objects/Card Effects/Test Effect B")]
public class TestEffectBSO : AbstractEffectSO
{
    public override void PerformEffect(AbstractCard card, GameObject obj)
    {
        obj.transform.localScale = obj.transform.localScale * 4f ;
    }
}
