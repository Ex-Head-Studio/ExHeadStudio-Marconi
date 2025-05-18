using UnityEngine;

public class AbstractObstacle : MonoBehaviour
{
    [SerializeField] private Color color;

    private MeshRenderer meshRenderer;

    public Vector2 obstaclePosition;

    public Tile tile;


    protected virtual void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.color = color;
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }
    
    public void SetPosition(Vector2 position)
    {
        this.obstaclePosition = position;
    }
}
