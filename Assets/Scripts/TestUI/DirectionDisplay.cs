using UnityEngine;
using UnityEngine.UI;

public class DirectionDisplay : MonoBehaviour
{


    //ATTENZIONE: l'immagine dell'inidicatore deve trovarsi sullo stesso piano, o al massimo alle spalle,
    //della nave a cui si riferisce (altrimenti non si vede nella griglia)

    [SerializeField] private GameObject directionIndicatorImage;
    private RawImage image;
    [SerializeField] private Camera gridCamera;
    private Ship shipScript;
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
        image = directionIndicatorImage.GetComponent<RawImage>();
    }

    private void DisplayDirection(string shipName, int direction, int messageType)
    {
        if(shipName == shipScript.shipName)
        {
            if(messageType == (int)MessageType.attack)
            {
                image.color = Color.red;
            }
            else
            {
                image.color = Color.green;
            
            }
            directionIndicatorImage.SetActive(true);

            //ATTENZIONE: la rotazione dell'immagine deve essere fatta attorno all'asse z
            //l'immagine della freccia inizialmente punta verso dx,
            //bisogna gestire la rotazione della freccia, che è relativa al transform del parent

            //TODO correggere il modo in cui ruota l'immagine, perchè altrimenti ad ogni invocazione della funzione
            //la freccia ruota di 90 gradi, invece di ruotare solo la prima volta

            Debug.Log("Direction: " + direction);
            switch (direction)
            {

                case 0://up:
                    directionIndicatorImage.transform.Rotate(0, 0, 90, Space.World);

                    break;

                case 1://down:
                    directionIndicatorImage.transform.Rotate(0, 0, -90, Space.World);
                    break;

                case 2://left

                    directionIndicatorImage.transform.Rotate(0, 0,180,Space.World);
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
