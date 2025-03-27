using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

[CreateAssetMenu(fileName = "ShipManagerSO", menuName = "Scriptable Objects/ShipManagerSO")]
public class ShipManagerSO : ScriptableObject
{
    [SerializeField] private int initialEnemyShips = 3;
    [SerializeField] private int initialAllyShips = 3;
    //vedi tu se usare questi
    public List<string> startingShips;
    [SerializeField] public float influenceDecay;
    [SerializeField] public float decayMomentum;

    public int enemyShips;
    public int allyShips;
    public int numberOfMessages;
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

    public void RandomizeShips(){
        startingShips.OrderBy(x=>Random.value);
    }
}
