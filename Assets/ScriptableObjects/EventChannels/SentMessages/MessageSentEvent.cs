using UnityEngine;

[CreateAssetMenu(fileName = "MessageSentEvent", menuName = "Events/Message Sent Event")]
public class MessageSentEvent : AbstractEventSO<MessageStruct>
{
}

[System.Serializable]
public struct MessageStruct
{
    public string sender;

    public int messageType;
    public int entity;
    public int direction;

    public MessageStruct(string sender, int messageType, int entity, int direction)
    {
        this.sender = sender;
        this.messageType = messageType;
        this.entity = entity;
        this.direction = direction;
    }
}