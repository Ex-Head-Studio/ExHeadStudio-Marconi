using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractObject : MonoBehaviour
{
    private AbstractObjectDataSO abstractObjectDataSO;

    public string objectName;
    public string objectDescription;
    public Image objectIllustration;
    private void Start()
    {
        abstractObjectDataSO = Resources.Load<AbstractObjectDataSO>("ScriptableObjects/AbstractObjectDataSO");
        objectName = abstractObjectDataSO.objectName;
        objectDescription = abstractObjectDataSO.objectDescription;
        objectIllustration = abstractObjectDataSO.objectIllustration;
    }

    //funzione che tutti gli oggetti devono implementare
    public abstract void ObjectAction();
    public virtual void ObjectDebug()
    {
        Debug.Log("Object name: " + objectName + " - Object description: " + objectDescription);
    }

    public void SetObjectIllustration(Image image)
    {
        objectIllustration.sprite= image.sprite;
    }

    public Image GetObjectIllustration()
    {
        return objectIllustration;
    }

    public void SetObjectName(string name)
    {
        objectName = name;
    }
    public string GetObjectName()
    {
        return objectName;
    }

    public void SetObjectDescription(string description)
    {
        objectDescription = description;
    }

    public string GetObjectDescription()
    {
        return objectDescription;
    }
}
