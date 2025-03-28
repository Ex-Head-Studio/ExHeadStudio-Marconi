using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractObject : MonoBehaviour
{
    [SerializeField] private AbstractObjectDataSO objSO;

    public string objectName;
    public string objectDescription;
    public Image objectIllustration;
    private void Start()
    {
        objectName = objSO.objectName;
        objectDescription = objSO.objectDescription;
        objectIllustration = objSO.objectIllustration;
    }


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
