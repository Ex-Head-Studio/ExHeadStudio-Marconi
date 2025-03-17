using UnityEngine;

[CreateAssetMenu(fileName = "MessageStructSO", menuName = "Scriptable Objects/MessageStructSO")]
public class MessageStructSO : ScriptableObject
{
    public struct Message
    {
        public string message;
        public string sender;
    }
}
