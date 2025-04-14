using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShipSO", menuName = "Scriptable Objects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] public int movementRange;
    [SerializeField] public int attackRange;
    [SerializeField] public int attackPower;
    [SerializeField] public int health;
    public string[] statNames = { "Movement Range", "Attack Range", "Attack Power", "Health" };
    // per ogni variabile aggiunta allo scriptable object, aggiungere un nome alla lista statNames

    public Dictionary<string, int> statsDictionary =
    new Dictionary<string, int>();

    public float shipInfluence;
    [SerializeField] public GameObject shipModelPrefab;
    [SerializeField] public GameObject shipModelMesh;
    
    [SerializeField] public Mesh shipNameMesh;
    [SerializeField] public Mesh shipClassMesh;

     private void OnEnable()
    {
        statsDictionary["Movement Range"] = movementRange;
        statsDictionary["Attack Range"] = attackRange;
        statsDictionary["Attack Power"] = attackPower;
        statsDictionary["Health"] = health;
    }
    private void OnDisable()
    {
        statsDictionary.Clear();
    }
}
