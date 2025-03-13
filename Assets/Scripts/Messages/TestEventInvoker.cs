using UnityEngine;

public class TestEventInvoker : MonoBehaviour
{
    [SerializeField] private MessageSentEvent messageSentEvent;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MessageStruct message = new MessageStruct();
            message.messageType = 1;
            message.sender = "TestEventInvoker";
            messageSentEvent?.Invoke(message);
        }
    }
}
