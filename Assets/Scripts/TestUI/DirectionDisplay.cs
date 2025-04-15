using TMPro;
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
    [SerializeField] private GameObject upAttack, downAttack, leftAttack, rightAttack;
    [SerializeField] private TMP_Text shipRangeText;

    private AShip shipScript;
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
        shipScript = GetComponent<AShip>();
            upArrow.SetActive(false);
            downArrow.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            upAttack.SetActive(false);
            downAttack.SetActive(false);  
            leftAttack.SetActive(false);
            rightAttack.SetActive(false);      
    }

    private void DisplayDirection(string shipName, int direction, int messageType)
    {
        if(shipName == shipScript.shipName)
        {
            if(messageType == (int)MessageType.attack)
            {
                //setto il testo per il range
                shipRangeText.text = shipScript.attackRange.ToString();

                arrowMaterial.SetColor("_FresnelColor",new Color(15f,1f,1f));

                switch (direction)
                {

                    case 0://up:
                        upAttack.SetActive(true);
                        break;

                    case 1://down:
                        downAttack.SetActive(true);
                        break;

                    case 2://left
                        leftAttack.SetActive(true);
                        break;
                    case 3://right
                        rightAttack.SetActive(true);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                arrowMaterial.SetColor("_FresnelColor",new Color(15f,15f,1f));

                    shipRangeText.text = shipScript.attackRange.ToString();

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
    }



    private void HideDirection(string shipName)
    {
        if(shipName == shipScript.shipName)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(false);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            upAttack.SetActive(false);
            downAttack.SetActive(false);  
            leftAttack.SetActive(false);
            rightAttack.SetActive(false);      
        }


        //serve a nascondere il testo del range quando non si è più sopra la nave
        shipRangeText.text = "";
    }
    public void HideDirection()
    {
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        upAttack.SetActive(false);
        downAttack.SetActive(false);  
        leftAttack.SetActive(false);
        rightAttack.SetActive(false);  

        //serve a nascondere il testo del range quando non si è più sopra la nave
        shipRangeText.text = "";
    }
}
