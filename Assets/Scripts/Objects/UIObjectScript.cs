using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIObjectScript : MonoBehaviour, ISubmitHandler, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private GameObject nameChildren;
    [SerializeField] private GameObject descriptionChildren;

    private AbstractObject abstractObject;

    //inserire reference al pannello di conferma
    //inserire i bottoni e gli eventi legati per chiamare la funzione del consumabile



    private TMP_Text objectNameText;
    private TMP_Text objectDescriptionText;
    private Image objectIllustrationImage;


    private void Awake()
    {
        objectNameText = nameChildren.GetComponent<TMP_Text>();
        objectDescriptionText = descriptionChildren.GetComponent<TMP_Text>();
        objectIllustrationImage = GetComponent<Image>();
    }


    public void SetObjectDisplay(string objectName, string objectDescription, Image objectIllustration)
    {
        objectNameText.text = objectName;
        objectDescriptionText.text = objectDescription;
        objectIllustrationImage.sprite = objectIllustration.sprite;
    }

    public void SetDescriptionParent(Transform parent)
    {
        descriptionChildren.transform.SetParent(parent);

        //copilot che fai?
        descriptionChildren.transform.localPosition = Vector3.zero;
        descriptionChildren.transform.localScale = Vector3.one;
    }

    public void SetObjectReference(AbstractObject obj)
    {
        abstractObject = obj;
    }


    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //cambiare colore dell'immagine
        //attivare immagine

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //cambiare colore dell'imagine
        //disattivare immagine
        
    }

    //vedere quale delle due funzioni è migliore, ma secondo me fanno proprio due cose diverse

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //aprire pannello di conferma
    }

    public void OnSubmit(BaseEventData baseEventData)
    {
        Debug.Log("Oggetto attivato");
        //attiva pannello di conferma
    }

}
