using UnityEngine;

public class MessageSentListener : AbstractEventListenerSO<MessageStruct>
{
    public void OnMessageSent(MessageStruct message)
    {
        Debug.Log("Message: " + message.messageType + " from: " + message.sender);
    }
}
