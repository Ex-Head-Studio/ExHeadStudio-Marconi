using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;
public class UIObjectScript : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, IConsumableObjectTest
{
    [SerializeField] private GameObject nameChildren;
    [SerializeField] private GameObject descriptionChildren;
    [SerializeField] private GameObject confirmationPanel;

    [Header("Stack degli oggeti")]
    [SerializeField] private ObjectsStack objectsStack;

    private AbstractObject abstractObject;

    //inserire reference al pannello di conferma
    //inserire i bottoni e gli eventi legati per chiamare la funzione del consumabile

    private TMP_Text objectNameText;
    private TMP_Text objectDescriptionText;
    private Image objectIllustrationImage;

    private Image objectImage;

    //serve solo per fare un test sull'azione che spegne i toggles
    private string objName;

    private void Start()
    {
        objectImage = nameChildren.GetComponent<Image>();
    }
    public void SetObjectDisplay(string objectName, string objectDescription, Image objectIllustration)
    {

        //questi getter non vanno spostati, altrimenti non funziona lo script
        objectNameText = nameChildren?.GetComponentInChildren<TMP_Text>();
        objectDescriptionText = descriptionChildren?.GetComponentInChildren<TMP_Text>();
        objectIllustrationImage = descriptionChildren?.GetComponentInChildren<Image>();


        objectNameText.text = objectName;
        objectDescriptionText.text = objectDescription;
        objectIllustrationImage.sprite = objectIllustration.sprite;

        objName = objectName;
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

    public void DisableDescription()
    {
        descriptionChildren.SetActive(false);
    }

    public void DisableConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
    }

    public void EnableConfirmationPanel()
    {
        confirmationPanel.SetActive(true);
    }

    public void EnableDescription()
    {
        descriptionChildren.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        //cambiare colore dell'immagine
        objectImage.enabled = true;
        EnableDescription();

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //cambiare colore dell'imagine
        objectImage.enabled = false;
        DisableDescription();
        
    }

    //vedere quale delle due funzioni è migliore, ma secondo me fanno proprio due cose diverse

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        EnableConfirmationPanel();
    }



    //?
    public void UseObject()
    {
        IConsumableObjectTest.InvokeConsumableObjectEvent(objName);

        abstractObject.ObjectAction();
        
        RemoveObject();
    }

    public void OnConsumableObjectAction(string testString)
    {

    }
    
    public void RemoveObject()
    {
        objectsStack.RemoveObject(this.GetComponent<AbstractObject>());
        //TODO eliminazione dalla UI
        StartCoroutine(WaitBeforeDestroy(this.GetComponent<AbstractObject>()));

    }   
    private IEnumerator WaitBeforeDestroy(AbstractObject obj)
    {
        yield return new WaitForSeconds(1);
        objectsStack.RemoveObject(obj);
        Destroy(descriptionChildren);
        Destroy(this.gameObject);
    }

}
