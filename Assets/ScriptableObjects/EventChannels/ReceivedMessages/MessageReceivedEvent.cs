using UnityEditor.Rendering.LookDev;
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

        public AnswerStruct(bool result, string receiver)
    {
        this.result = result;
        this.receiver = receiver;
    }
}
