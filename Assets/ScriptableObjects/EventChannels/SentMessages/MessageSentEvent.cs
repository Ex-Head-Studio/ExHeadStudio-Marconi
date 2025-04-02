using UnityEngine;

[CreateAssetMenu(fileName = "MessageSentEvent", menuName = "Events/Message Sent Event")]
public class MessageSentEvent : AbstractEventSO<MessageStruct>
{
}

[System.Serializable]
public struct MessageStruct
{
    public string sender;
    public int moveId;
    public int messageType;
    public int entity;
    public int direction;

    public MessageStruct(string sender, int moveId, int messageType, int entity, int direction)
    {
        this.sender = sender;
        this.moveId=moveId;
        this.messageType = messageType;
        this.entity = entity;
        this.direction = direction;
    }
}