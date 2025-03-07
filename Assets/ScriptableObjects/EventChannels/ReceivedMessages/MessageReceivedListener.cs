using UnityEngine;

public class MessageReceivedListener : AbstractEventListenerSO<AnswerStruct>
{
    public void OnMessageReceived(AnswerStruct answer)
    {
        Debug.Log("Message received: " + answer.result + " by: " + answer.receiver);
    }
}