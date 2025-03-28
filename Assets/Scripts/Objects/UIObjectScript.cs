using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectScript : MonoBehaviour
{
    [SerializeField] private GameObject nameChildren;
    [SerializeField] private GameObject descriptionChildren;

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

}
