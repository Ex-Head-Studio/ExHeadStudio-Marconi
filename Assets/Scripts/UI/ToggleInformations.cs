using UnityEngine;
using UnityEngine.UI;

public class ToggleInformations : MonoBehaviour
{
    [SerializeField] private string sender;
    private int enemy, direction, messageType;

    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void SetInformations(string sender, int enemy, int direction, int messageType)
    {
        this.sender = sender;
        this.enemy = enemy;
        this.direction = direction;
        this.messageType = messageType;
    }

    public void PrintInformations(bool value)
    {
        if(toggle.isOn)
        {
             Debug.Log("Sender: " + sender + " Enemy: " + enemy + " Direction: " + direction + " MessageType: " + messageType);
        }
       
    }

    public int GetToggleEntity()
    {
        return enemy;
    }
    public string GetToggleSender()
    {
        return sender;
    }
    public int GetToggleDirection()
    {
        return direction;
    }
    public int GetToggleMessageType()
    {
        return messageType;
    }
}
