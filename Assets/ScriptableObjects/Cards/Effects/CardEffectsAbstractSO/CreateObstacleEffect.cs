using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ChangeStatsEffect", menuName = "Scriptable Objects/Card Effects/Create Obstacle Effect")]
public class CreateObstacleEffect : AbstractEffectSO
{
    private enum GenerationMode
    {
        randomFromList,
        firstOfList,
    }

    [SerializeField] private List<GameObject> obstacleList = new List<GameObject>();
    [SerializeField] private GenerationMode generationMode;

    private Tile tileScript;
    public override void PerformEffect(EffectStruct effectStruct)
    {
        if (effectStruct.obj.TryGetComponent<Tile>(out tileScript))
            switch (generationMode)
            {
                case GenerationMode.randomFromList:

                    int randomIndex = UnityEngine.Random.Range(0, obstacleList.Count - 1);
                    tileScript.InstantiateObstacle(obstacleList[randomIndex]);
                    break;

                case GenerationMode.firstOfList:
                    tileScript.InstantiateObstacle(obstacleList[0]);
                    break;

                default:
                    break;
            }

        EndEffect(0);
    }
}
