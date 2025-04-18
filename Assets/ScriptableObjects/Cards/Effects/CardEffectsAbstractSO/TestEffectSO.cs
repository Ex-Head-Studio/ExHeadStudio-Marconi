using UnityEngine;

[CreateAssetMenu(fileName = "TestEffectSO", menuName = "Scriptable Objects/Card Effects/Test Effect")]
public class TestEffectSO : AbstractEffectSO
{
    public override void PerformEffect(EffectStruct effectStruct)
    {
        //funzioni di test, lanciano delle eccezioni p
        Destroy(effectStruct.obj, 3f);
    }
}

