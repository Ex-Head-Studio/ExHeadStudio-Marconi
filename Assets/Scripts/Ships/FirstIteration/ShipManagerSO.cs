using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random=UnityEngine.Random;

[CreateAssetMenu(fileName = "ShipManagerSO", menuName = "Scriptable Objects/ShipManagerSO")]
public class ShipManagerSO : ScriptableObject
{
    [SerializeField] private int initialEnemyShips;
    [SerializeField] private int initialAllyShips;

    [SerializeField] public int numberOfMessages;
    public List<string> startingShips;
    [SerializeField] public float influenceDecay;
    [SerializeField] public float decayMomentum;
    [SerializeField] public EndedEnemyTurnEvent onEndEnemyTurn;

    [Header("Materials")]
    [SerializeField] public Material allyMaterial;
    [SerializeField] public Material enemyMaterial;

    [SerializeField] public List<ShipSO> shipSOarray;

    [NonSerialized]
    public int enemyShips;
     [NonSerialized]
    public int allyShips;

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

    public void RandomizeShips()
    {
        startingShips.OrderBy(x=>Random.value);
    }
}
