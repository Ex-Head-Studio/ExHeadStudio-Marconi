using UnityEngine;

public class MessageReceivedListener : AbstractEventListenerSO<AnswerStruct>
{
    Ship ship;

    private void Start()
    {
        ship = GetComponent<Ship>();
    }
    public void OnMessageReceived(AnswerStruct answer)
    {
    
        //chiama un bug sull'if
        if(answer.receiver==ship.name)
        {
            ship.ExecuteInstructions(answer.result, answer.entity);
        }
        else
        {
            //Devo aggiungere un else, altrimenti tutte le altri navi non sapranno cosa fare
            ship.ExecuteInstructions(true, ship.faction);
        }
    }
}