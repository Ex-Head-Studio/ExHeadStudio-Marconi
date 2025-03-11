using UnityEngine;

public class MessageReceivedListener : AbstractEventListenerSO<AnswerStruct>
{
    Ship ship;
    public void OnMessageReceived(AnswerStruct answer)
    {
    
        //chiama un bug sull'if
        if(answer.receiver==ship.name)
        {
            ship.ExecuteInstructions(answer.result, answer.entity);
        }
    }
}