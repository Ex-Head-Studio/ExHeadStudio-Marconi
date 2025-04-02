using System.Collections;
using FMOD;
using UnityEngine;
using UnityEngine.UI;
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
        string objectName = obj.GetObjectName();
        string objectDescription = obj.GetObjectDescription();
        Image objectIllustration = obj.GetObjectIllustration();

        tmpObject = Instantiate(UIobjectPrefab, objectsStackTransform);
        tmpObject.name = obj.objectName;
        tmpObjectScript = tmpObject.GetComponent<UIObjectScript>();
        tmpObjectScript.SetObjectReference(obj);
        tmpObjectScript.SetObjectDisplay(objectName, objectDescription, objectIllustration);
        tmpObjectScript.SetDescriptionParent(objectsDescriptionTransform);
        tmpObjectScript.DisableDescription();
        tmpObjectScript.DisableConfirmationPanel();
    }


}
