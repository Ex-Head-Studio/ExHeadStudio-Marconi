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

        //TODO bisogna controllare questa logica
    
        //chiama un bug sull'if
        if(answer.receiver==ship.name)
        {
            ship.ExecuteInstructions(answer.result, answer.entity);
        }
        else
        {
            if(ship.faction == (int)Entity.ally)
            {
                ship.ExecuteInstructions(false, ship.faction);
            }
            else
            {
                ship.ExecuteInstructions(true, ship.faction);
            }
        }
    }
}