using UnityEngine;
using UnityEngine.UI;

public class AbstractObjectDataSO : ScriptableObject
{
    public string objectName;
    [TextArea(3, 10)]
    public string objectDescription;
    public Image objectIllustration;
}
