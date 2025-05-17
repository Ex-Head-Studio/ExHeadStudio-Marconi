using UnityEngine;

public class AbstractObstacle : MonoBehaviour
{
    [SerializeField] private Color color;

    private MeshRenderer meshRenderer;

    public Vector2 obstaclePosition;

    protected virtual void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.color = color;
    }

}
