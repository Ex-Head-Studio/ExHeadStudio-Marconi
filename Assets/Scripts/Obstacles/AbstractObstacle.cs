using UnityEngine;

public class AbstractObstacle : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private string _obstacleName;
    [SerializeField] private string _obstacleDescription;
    [SerializeField] private MeshRenderer _wireframeModel;
    
    [Header("Wireframe Display Settings")]
    [Tooltip("Fattore di scala per il modello wireframe quando visualizzato nell'UI")]
    [SerializeField] private Vector3 _wireframeScaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 _wireframeRiposition = new Vector3();
    private MeshRenderer meshRenderer;

    public Vector2 obstaclePosition;

    public Tile tile;

    // Metodo per accedere al nome dell'ostacolo
    public string GetObstacleName()
    {
        return _obstacleName;
    }

    // Metodo per accedere alla descrizione dell'ostacolo
    public string GetObstacleDescription()
    {
        return _obstacleDescription;
    }
    // Metodo per accedere al modello wireframe
    public MeshRenderer GetWireframeModel()
    {
        return _wireframeModel;
    }
    
    // Metodo per accedere al fattore di scala
    public Vector3 GetWireframeScaleFactor()
    {
        return _wireframeScaleFactor;
    }

    // Metodo per accedere all'offset di riposizionamento
    public Vector3 GetWireframeRepositionOffset()
    {
        return _wireframeRiposition;
    }

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
