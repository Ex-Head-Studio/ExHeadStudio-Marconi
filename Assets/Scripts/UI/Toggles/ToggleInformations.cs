    using UnityEngine;
using UnityEngine.UI;

public class ToggleInformations : MonoBehaviour
{
    [SerializeField] private string sender;
    private int enemy, direction, messageType, moveId;

    private Toggle toggle;
    private ToggleGroup toggleGroup;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void SetInformations(string sender,int moveId, int enemy, int direction, int messageType)
    {
        this.sender = sender;
        this.moveId = moveId;
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

    public Toggle GetToggle()
    {
        return toggle;
    }

    public int GetToggleMoveId(){
        return moveId;
    }
    public ToggleGroup GetToggleGroup()
    {
        return toggle.group;
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
