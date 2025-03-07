using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugTextEditor : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateText(MessageStruct message)
    {
        debugText.text = "Message: " + message.message + " from: " + message.sender;
    }
}
