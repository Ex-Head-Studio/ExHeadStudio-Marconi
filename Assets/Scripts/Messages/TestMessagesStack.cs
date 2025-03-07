using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMessagesStack : MonoBehaviour
{
    [SerializeField] private Button buttonPrefab;
    public List<MessageStruct> messages = new List<MessageStruct>();
    private List<Button> stackMessages = new List<Button>();

    public void AddMessage(MessageStruct message)
    {
        messages.Add(message);
        Instantiate(buttonPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
    }

    public void RemoveMessage()
    {
        messages.RemoveAt(0);
    }

}
