using UnityEngine;

public class TileObstacle : Tile
{
    [SerializeField] private GameObject obstaclePrefab;

    private void Start()
    {
        InstantiateObstacle(obstaclePrefab, this);
    }
}