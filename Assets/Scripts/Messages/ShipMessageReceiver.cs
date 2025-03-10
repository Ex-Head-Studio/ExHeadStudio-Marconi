using UnityEngine;

public class ShipMessageReceiver : MonoBehaviour
{
    public void OnMessageReceived(AnswerStruct answer)
    {
        Debug.Log("Answer received with by (test):" + gameObject.name + "with value: " + answer.result);
    }
}
