using UnityEngine;

[CreateAssetMenu(fileName = "ShipSO", menuName = "Scriptable Objects/ShipSO")]
public class ShipSO : ScriptableObject
{
    [SerializeField] public int movementRange;
    [SerializeField] public int attackRange;
    [SerializeField] public int attackPower;
    [SerializeField] public int health;
    [SerializeField] public GameObject shipModelPrefab;
}
