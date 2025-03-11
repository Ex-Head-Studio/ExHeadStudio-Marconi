using UnityEngine;

public class MessageReceivedListener : AbstractEventListenerSO<AnswerStruct>
{
    Ship ship;
    public void OnMessageReceived(AnswerStruct answer)
    {
        if(answer.receiver==ship.name)
        {
            ship.ExecuteInstructions(answer.result);
        }
    }
}