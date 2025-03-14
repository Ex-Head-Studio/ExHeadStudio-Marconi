using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Entity
{
    ally,
    enemy
}

public enum MessageType
{
    attack,
    movement
}

public enum Directions
{
    up,
    down, 
    left,
    right
}

[CreateAssetMenu(fileName = "MessagesStack", menuName = "Scriptable Objects/MessagesStack")]
public class MessagesStack : ScriptableObject
{
    public string[] directions = new string[] {"up", "down", "left", "right"};
    //[SerializeField] public Button buttonPrefab;
    [SerializeField] public Toggle togglePrefab;

    public List<MessageStruct> messages = new List<MessageStruct>();

    public void AddMessage(MessageStruct message, Transform parent)
    {
        messages.Add(message);
    }
    public void RemoveAllMessages()
    {
        messages.Clear();
    }
}
