
using UnityEngine;

[CreateAssetMenu(fileName = "MessageReceivedEvent", menuName = "Events/Message Received Event")]
public class MessageReceivedEvent : AbstractEventSO<AnswerStruct>
{
}

[System.Serializable]
public struct AnswerStruct
{

    //TODO valutare se cambiare i nomi
    public bool result;
    public string receiver;
    public int idMove;
    public int entity;

    public AnswerStruct(bool result, int idMove, string receiver, int entity)
    {
        this.result = result;
        this.idMove = idMove;
        this.receiver = receiver;
        this.entity = entity;

    }
}
