using UnityEngine;

public class DirectionDisplay : MonoBehaviour
{

    [SerializeField] private GameObject directionIndicatorImage;
    [SerializeField] private Camera gridCamera;

    private Ship shipScript;
    //TODO da modificare, inserire la rotazione della direzione
    private void OnEnable()
    {
        DirectionIndicatorTest.OnPointerEnterEvent += DisplayDirection;
        DirectionIndicatorTest.OnPointerExitEvent += HideDirection;
    }

    private void OnDisable()
    {
        DirectionIndicatorTest.OnPointerEnterEvent -= DisplayDirection;
        DirectionIndicatorTest.OnPointerExitEvent -= HideDirection;
        
    }

    private void Start()
    {
        directionIndicatorImage.SetActive(false);
        shipScript = GetComponent<Ship>();
    }

    private void DisplayDirection(string shipName)
    {
        if(shipName == shipScript.shipName)
        {
            directionIndicatorImage.SetActive(true);
            directionIndicatorImage.transform.LookAt(gridCamera.transform);
            directionIndicatorImage.transform.Rotate(0, 0, 0);
            Debug.Log("Pointer Enter, ship: " + shipName);

        }

    }

    private void HideDirection(string shipName)
    {
        if(shipName == shipScript.shipName)
        {
            directionIndicatorImage.SetActive(false);
            Debug.Log("Pointer Exit");
        }
    }
}
