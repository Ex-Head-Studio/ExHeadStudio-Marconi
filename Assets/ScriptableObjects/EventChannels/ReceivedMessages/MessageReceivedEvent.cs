
using UnityEngine;

[CreateAssetMenu(fileName = "MessageReceivedEvent", menuName = "Events/Message Received Event")]
public class MessageReceivedEvent : AbstractEventSO<AnswerStruct>
{
}

[System.Serializable]
public struct AnswerStruct
{
    public bool result;
    public string receiver;

    public int entity;

    public AnswerStruct(bool result, string receiver, int entity)
    {
        this.result = result;
        this.receiver = receiver;
        this.entity = entity;

    }
}
