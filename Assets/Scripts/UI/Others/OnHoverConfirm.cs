using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;


[RequireComponent(typeof(Outline))]

public class OnHoverConfirm : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    //Serve davvero associarlo al bottone? Possiamo inserirlo nel manager?
    [SerializeField] private GameObject confirmButton;
    private Outline outline;


    //OCCHIO A USARE FUNZIONI SUGLI INPUT
    //se l'input non è usato correttamente non funziona da controller

    //TODO verificare se si può usare questo attributo
    [EventRef]
    public string confirmButtonSound = "event:/UI/BigButton";

    void Start()
    {
        outline = confirmButton.GetComponent<Outline>();
    } 


    //TODO rimuovere i log
    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.outlineColor = Color.green;
        //Debug.Log("Sto abilitando l'outline");
        outline.enabled = true;
         outline.needsUpdate = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Sto cambiando il colore dell'outline");
        outline.outlineColor = Color.blue;
        outline.needsUpdate = true;
        RuntimeManager.PlayOneShot(confirmButtonSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.outlineColor = Color.green;
        //Debug.Log("Sto disabilitando l'outline");
        outline.enabled = false;
         outline.needsUpdate = true;
    }


}