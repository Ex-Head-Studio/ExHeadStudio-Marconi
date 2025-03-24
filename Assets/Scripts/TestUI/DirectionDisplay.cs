using UnityEngine;

public class DirectionDisplay : MonoBehaviour
{

    [SerializeField] private GameObject directionIndicatorImage;
    [SerializeField] private Camera gridCamera;
    private Ship shipScript;
    //TODO da modificare, inserire la rotazione della direzione
    //TODO inserire lettura del parametro che sceglie il tipo di azione (movimento o attacco)
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

    private void DisplayDirection(string shipName, int direction, int messageType)
    {
        if(shipName == shipScript.shipName)
        {
            directionIndicatorImage.SetActive(true);

            //l'immagine della freccia inizialmente punta verso dx,
            //bisogna gestire la rotazione della freccia, che è relativa al transform del parent

            Debug.Log("Direction: " + direction);
            switch (direction)
            {
                //mi da problemi con l'enume delle direzioni, i valori vanno sistemati
                case 0://up:
                    directionIndicatorImage.transform.Rotate(0, 90, 0, Space.World);

                    break;

                case 1://down:
                    directionIndicatorImage.transform.Rotate(0, -90, 0, Space.World);
                    break;

                case 2://left

                    directionIndicatorImage.transform.Rotate(0, 180, 0,Space.World);
                    break;
                case 3://right

                    directionIndicatorImage.transform.Rotate(0, 0, 0, Space.World);

                    break;

                default:
                    directionIndicatorImage.transform.Rotate(0, 0, 0, Space.World);
                    break;
            }
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
