using UnityEngine;

public class TestMessagesCanvas : MonoBehaviour
{
    [SerializeField] MessageReceivedEvent messageReceivedEvent;
    private TMPro.TextMeshProUGUI text;
    private string receiver;

    private void Start()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        CloseCanvas();
    }
    public void SetUpText(MessageStruct message)
    {
        text.text = "Message: " + message.message + " from: " + message.sender;

        //controllare sintassi
        receiver = message.sender;
    }
    public void OpenCanvas()
    {
        gameObject.SetActive(true);
    }
    public void CloseCanvas()
    {
        gameObject.SetActive(false);
    }

    public void DeclineAction()
    {
        messageReceivedEvent?.Invoke(new AnswerStruct(false, receiver));
        CloseCanvas();
    }

    public void ConfirmAction()
    {
        messageReceivedEvent?.Invoke(new AnswerStruct(true, receiver));
        CloseCanvas();
    }
}
