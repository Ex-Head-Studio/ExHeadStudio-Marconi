using UnityEngine;

[CreateAssetMenu(fileName = "ChangeStatsEffect", menuName = "Scriptable Objects/Card Effects/Remove Obstacle Effect")]
public class RemoveObstacleEffect : AbstractEffectSO
{
    private Tile tileScript;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<Tile>(out tileScript))
        {

            tileScript.RemoveObstacle();
        }
        EndEffect(0);
    }
}
