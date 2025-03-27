using UnityEngine;

public abstract class AbstractObject : MonoBehaviour
{
    public string objectName;
    [TextArea(3, 10)]
    public string objectDescription;

    public abstract void ObjectAction();
    public virtual void ObjectDebug()
    {
        Debug.Log("Object name: " + objectName + " - Object description: " + objectDescription);
    }
}
