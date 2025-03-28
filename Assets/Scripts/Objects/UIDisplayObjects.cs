using System.Collections;
using UnityEngine;

public class UIDisplayObjects : MonoBehaviour
{
/// <summary>
/// Lo script, da associare a Game Menu manager, si occupa della raccolta e visualizzazione degli oggetti consumabili
/// </summary>
    [Header("Objects Panel")]
    [SerializeField] private Transform objectsStackTransform;
    [SerializeField] private Transform objectsDescriptionTransform;


    [Header("Stack degli oggeti")]
    [SerializeField] private ObjectsStack objectsStack;


    //prefab dell'oggetto da visualizzare in UI, è diverso dalla classe AbstractObject
    private GameObject UIobjectPrefab;
    private GameObject tmpObject;
    private UIObjectScript tmpObjectScript;

    private void Awake()
    {
        UIobjectPrefab = objectsStack.UIobjectPrefab;
    }

    public void UIReceiveObject(AbstractObject obj)
    {
        objectsStack.AddObject(obj);
        DisplayObject(obj);
    }
    private void DisplayObject(AbstractObject obj)
    {
        // TODO QUESTA COSA VA SISTEMATA, bisogna asseganre uno script al prefab che faccia le operazioni
        //devo settare i parametri del prefab
        //devo prendere i children e settare i parametri
        //il figlio zero è il nome e la parte intergibile
        //il figlio uno è la descrizione
        tmpObject = Instantiate(UIobjectPrefab, objectsStackTransform);
        tmpObject.name = obj.objectName;
        tmpObjectScript = tmpObject.GetComponent<UIObjectScript>();
        tmpObjectScript.SetObjectReference(obj);
        tmpObjectScript.SetObjectDisplay(obj.GetObjectName(), obj.GetObjectDescription(), obj.GetObjectIllustration());
        tmpObjectScript.SetDescriptionParent(objectsDescriptionTransform);
    }

    public void RemoveObject()
    {
        //TODO eliminazione dalla UI
        StartCoroutine(WaitBeforeDestroy(this.GetComponent<AbstractObject>()));
    }

    public void UseObject()
    {
        //uso dell'oggetto
        //objScript.ObjectAction();
        RemoveObject();
    }
    private IEnumerator WaitBeforeDestroy(AbstractObject obj)
    {
        yield return new WaitForSeconds(1);
        objectsStack.RemoveObject(obj);
    }
}
