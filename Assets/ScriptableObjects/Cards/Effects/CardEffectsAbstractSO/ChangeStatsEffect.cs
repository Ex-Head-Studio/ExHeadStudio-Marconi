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
    [SerializeField] private ParticleSystem changeStatParticle;
    private ParticleSystem changeStatParticleInstance;

    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<AShip>(out AShip shipScript))
        {
            changeStatParticleInstance = Instantiate(changeStatParticle, shipScript.transform.position + new Vector3(0f,5f,0f), Quaternion.identity);
            changeStatParticleInstance.GetComponent<CartoonFX.CFXR_ParticleText>().UpdateText($"+{amount} {statName}");
            changeStatParticleInstance.Play();
            shipScript.ChangeStat(statName, amount);
            EndEffect(0);
        }
    }
}
