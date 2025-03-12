using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnHoverConfirm : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] private GameObject confirmButton;
    private Outline outline;

    void Start()
    {
        outline = confirmButton.GetComponent<Outline>();
        
    } 

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.outlineColor = Color.green;
        Debug.Log("Sto abilitando l'outline");
        outline.enabled = true;
         outline.needsUpdate = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Sto cambiando il colore dell'outline");
        outline.outlineColor = Color.blue;
        outline.needsUpdate = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.outlineColor = Color.green;
        Debug.Log("Sto disabilitando l'outline");
        outline.enabled = false;
         outline.needsUpdate = true;
    }


}