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
            message.message = "Hello World!";
            message.sender = "TestEventInvoker";
            messageSentEvent?.Invoke(message);
        }
    }
}
