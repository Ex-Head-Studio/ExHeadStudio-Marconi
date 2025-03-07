using UnityEngine;

public class TestMessagesCanvas : MonoBehaviour
{

    private TMPro.TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
    }
    public void SetUpText(MessageStruct message)
    {
        text.text = "Message: " + message.message + " from: " + message.sender;
    }
    public void OpenCanvas()
    {
        gameObject.SetActive(true);
    }

    public void CloseCanvas()
    {
        gameObject.SetActive(false);
    }
}
