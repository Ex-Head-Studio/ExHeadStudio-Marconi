using UnityEngine;

public class ShipMessageReceiver : MonoBehaviour
{
    private Ship shipScript;
    public void OnMessageReceived(AnswerStruct answer)
    {
        shipScript = GetComponent<Ship>();
        if(shipScript == null)
        {
            Debug.LogError("Ship script not found on object: " + gameObject.name);
            return;
        }
        else if(shipScript.shipName == answer.receiver)
        {
            Debug.Log("Answer received by:" + gameObject.name + "with value: " + answer.result);
            //entity è un parametro ridondante
            shipScript.ExecuteInstructions(answer.result, answer.entity);
        }

    }
}
