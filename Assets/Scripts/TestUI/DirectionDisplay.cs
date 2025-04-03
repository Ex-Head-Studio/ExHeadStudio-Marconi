using UnityEngine;
using UnityEngine.UI;

public class DirectionDisplay : MonoBehaviour
{
    [SerializeField] private Camera gridCamera;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private Material arrowMaterial;

    private Ship shipScript;
    private void OnEnable()
    {
        //tutti gli eventi sotto vengono dichiarati in Directionindicator
        DirectionIndicator.OnPointerEnterEvent += DisplayDirection; 
        DirectionIndicator.OnPointerExitEvent += HideDirection; 
        DirectionIndicator.OnToggleSelectedEvent += DisplayDirection; 
        DirectionIndicator.OnToggleDeselectedEvent += HideDirection; 
    }

    private void OnDisable()
    {
        DirectionIndicator.OnPointerEnterEvent -= DisplayDirection; 
        DirectionIndicator.OnPointerExitEvent -= HideDirection; 
        DirectionIndicator.OnToggleSelectedEvent -= DisplayDirection; 
        DirectionIndicator.OnToggleDeselectedEvent -= HideDirection;
    }

    private void Start()
    {
        shipScript = GetComponent<Ship>();
    }

    private void DisplayDirection(string shipName, int direction, int messageType)
    {
        Debug.Log("bho");
        if(shipName == shipScript.shipName)
        {
            if(messageType == (int)MessageType.attack)
            {
                arrowMaterial.SetColor("_FresnelColor",new Color(15f,1f,1f));
            }
            else
            {
                arrowMaterial.SetColor("_FresnelColor",new Color(15f,15f,1f));
            }

            switch (direction)
            {

                case 0://up:
                    upArrow.SetActive(true);
                    break;

                case 1://down:
                    downArrow.SetActive(true);
                    break;

                case 2://left
                    leftArrow.SetActive(true);
                    break;
                case 3://right
                    rightArrow.SetActive(true);
                    break;

                default:
                    break;
            }
        }
    }



    private void HideDirection(string shipName)
    {
        if(shipName == shipScript.shipName)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            //directionIndicatorImage.SetActive(false);
            Debug.Log("Pointer Exit");
        }
    }

    public void HideDirection()
    {
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
    }
}
