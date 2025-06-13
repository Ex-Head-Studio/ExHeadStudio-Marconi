using UnityEngine;
using System.Collections.Generic;

public class BarrierObstacle : AbstractObstacle, IObstacleHealth
{
    protected override void Start()
    {
        base.Start();

        if (healthBarPrefab != null)
        {
            SetHealth(obstacleHealth);
        }
    }

    #region Obstacle Health
    [Header("Health parameters")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject healthBarCanvasPrefab;
    [SerializeField] private int obstacleHealth;

    private int healthBarCount = 0;
    private List<GameObject> healthObjectsList = new List<GameObject>();

    private GameObject healthBarInstance;

    public void SetHealth(int amount)
    {
        healthBarCount = amount;
        for (int i = 0; i < healthBarCount; i++)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, healthBarCanvasPrefab.transform);
            healthObjectsList.Add(healthBarInstance);
        }
    }

    private FMOD.Studio.EventInstance barrierSound;

    public void PlayBarriera()
    {
        barrierSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Cards/Ting");
        barrierSound.start();
        barrierSound.release();
    }
    public void OnAttacked(ShipAttackStruct attackStruct)
    {
        if (attackStruct.gridPosition == obstaclePosition)
        {
            Debug.Log("Barriera colpita");
            ReduceHealth(attackStruct.damage);
        }
    }
    public void ReduceHealth(int damage)
    {
        obstacleHealth -= damage;

        if (obstacleHealth <= 0)
        {
            tile.RemoveObstacle();
            PlayBarriera();
        }
        if (healthBarPrefab != null && healthBarCanvasPrefab != null)
            {

                for (int i = 0; i < damage; i++)
                {

                    int index = healthBarCount - i - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    if (healthObjectsList.Count > 0)
                    {
                        GameObject healthBar = healthObjectsList[index];
                        healthObjectsList.RemoveAt(index);
                        Destroy(healthBar);
                    }
                    else
                    {
                        break;
                    }

                }
                healthBarCount -= damage;
            }
    }
    #endregion

}