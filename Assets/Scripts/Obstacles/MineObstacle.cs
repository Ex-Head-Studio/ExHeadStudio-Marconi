using UnityEngine;
using System.Collections.Generic;

public class MineObstacle : AbstractObstacle, IObstacleHealth, IObstacleAreaDamage
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
    public void OnAttacked(ShipAttackStruct attackStruct)
    {
        if (attackStruct.gridPosition == obstaclePosition)
        {
            Debug.Log("Roccia colpita");
            ReduceHealth(attackStruct.damage);
        }
    }
    public void ReduceHealth(int damage)
    {
        obstacleHealth -= damage;

        if (obstacleHealth <= 0)
        {
            AreaDamage();
            tile.RemoveObstacle();
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

    #region Obstacle Area Damage

    /// La funzione di danno ad area viene chiamata quando l'ostacolo viene distrutto
    [Header("Area Damage parameters")]
    [SerializeField] private int areaDamage;
    [Range(1, 10)]
    [Tooltip("The radius of the area damage around the obstacle, if the mode is \"Square\" the radius is 1.")]
    [SerializeField] private int areaDamageRadius;

    private enum AreaDamageType
    {
        Circle,
        Cross, 
        Square
    }

    [SerializeField] private AreaDamageType areaDamageType;

    [SerializeField] private OnShipAttackEvent attackEvent;

    [Header("Effects")]
    [SerializeField] private ParticleSystem explosionPrefab;

    public void AreaDamage()
    {
        switch (areaDamageType)
        {
            case AreaDamageType.Circle:

                CicloX();
                CicloY();
                CicloDiagonali();
                break;

            case AreaDamageType.Cross:
                CicloX();
                CicloY();
                break;
            case AreaDamageType.Square:

                areaDamageRadius = 1; // Forza il raggio a 1 per il quadrato

                CicloX();
                CicloY();
                CicloDiagonaliQuadrato();
                break;
            default:
                Debug.LogError("Area Damage Type not set");
                break;
        }
    }

    private void CicloX()
    {
        for (int x = (int)obstaclePosition.x - areaDamageRadius; x < (int)obstaclePosition.x + areaDamageRadius; x++)
        {
            if (x != (int)obstaclePosition.x)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, (int)obstaclePosition.y), areaDamage));
                InstantiateEffect(new Vector2(x, (int)obstaclePosition.y));
            }
        }
    }

    private void CicloY()
    {
        for (int y = (int)obstaclePosition.y - areaDamageRadius; y < (int)obstaclePosition.y + areaDamageRadius; y++)
        {
            if (y != (int)obstaclePosition.y)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2((int)obstaclePosition.x, y), areaDamage));
                InstantiateEffect(new Vector2((int)obstaclePosition.x, y));
            }
        }
    }

    private void CicloDiagonali()
    {

        //diagonale
        for (int x = (int)obstaclePosition.x - areaDamageRadius + 1, y = (int)obstaclePosition.y - areaDamageRadius + 1;
            x < (int)obstaclePosition.x + areaDamageRadius - 1 && y < (int)obstaclePosition.y + areaDamageRadius - 1;
            x++, y++)
        {
            if (x != (int)obstaclePosition.x && y != (int)obstaclePosition.y)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), areaDamage));
                InstantiateEffect(new Vector2(x, y));
            }
        }

        //antidiagonale
        for (int x = (int)obstaclePosition.x - areaDamageRadius + 1, y = (int)obstaclePosition.y + areaDamageRadius - 1;
            x < (int)obstaclePosition.x + areaDamageRadius - 1 && y > (int)obstaclePosition.y - areaDamageRadius + 1;
            x++, y--)
        {
            if (x != (int)obstaclePosition.x && y != (int)obstaclePosition.y)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), areaDamage));
                InstantiateEffect(new Vector2(x, y));
            }
        }
    }
    private void CicloDiagonaliQuadrato()
    {

        //diagonale
        for (int x = (int)obstaclePosition.x - areaDamageRadius, y = (int)obstaclePosition.y - areaDamageRadius;
            x < (int)obstaclePosition.x + areaDamageRadius && y < (int)obstaclePosition.y + areaDamageRadius;
            x++, y++)
        {
            if (x != (int)obstaclePosition.x && y != (int)obstaclePosition.y)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), areaDamage));
                InstantiateEffect(new Vector2(x, y));
            }
        }

        //antidiagonale
        for (int x = (int)obstaclePosition.x - areaDamageRadius, y = (int)obstaclePosition.y + areaDamageRadius;
            x < (int)obstaclePosition.x + areaDamageRadius && y > (int)obstaclePosition.y - areaDamageRadius;
            x++, y--)
        {
            if (x != (int)obstaclePosition.x && y != (int)obstaclePosition.y)
            {
                attackEvent.Invoke(new ShipAttackStruct(new Vector2(x, y), areaDamage));
                InstantiateEffect(new Vector2(x, y));
            }
        }
    }
    
    private void InstantiateEffect(Vector2 position)
    {
        if (explosionPrefab != null)
        {
            ParticleSystem explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 2f);
        }
    }   
    #endregion
}



