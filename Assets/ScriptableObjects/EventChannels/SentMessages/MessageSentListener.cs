using UnityEngine;

public class MessageSentListener : AbstractEventListenerSO<MessageStruct>
{
    public void OnMessageSent(MessageStruct message)
    {
        Debug.Log("Message: " + message.message + " from: " + message.sender);
    }
}
