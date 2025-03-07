using UnityEngine;

[CreateAssetMenu(fileName = "MessageSentEvent", menuName = "Events/Message Sent Event")]
public class MessageSentEvent : AbstractEventSO<MessageStruct>
{
}

[System.Serializable]
public struct MessageStruct
{
    public string message;
    public string sender;

    public MessageStruct(string message, string sender)
    {
        this.message = message;
        this.sender = sender;

        //altrimenti
        /*
        message = _message;
        sender = _sender;

        */
    }
}