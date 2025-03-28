using UnityEngine;

[CreateAssetMenu(fileName = "ShipManagerSO", menuName = "Scriptable Objects/ShipManagerSO")]
public class ShipManagerSO : ScriptableObject
{
    [SerializeField] private int initialEnemyShips = 3;
    [SerializeField] private int initialAllyShips = 3;


    //vedi tu se usare questi
    [SerializeField] private string[] shipNames; 

    private int enemyShips;
    private int allyShips;

    private void OnEnable()
    {
        enemyShips = initialEnemyShips;
        allyShips = initialAllyShips;
    }

    public int GetEnemyShips()
    {
        return enemyShips;
    }

    public int GetAllyShips()
    {
        return allyShips;
    }


}
