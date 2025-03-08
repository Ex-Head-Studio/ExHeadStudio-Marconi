using UnityEngine;

public class ShipMessageInvoker : MonoBehaviour
{
    [SerializeField] private MessageSentEvent messageSentEvent;

    [Header("Valori di debug")]
    [SerializeField] private string sender;
    [Range(0, 1)]
    [SerializeField] private int messageType;   
    [Range(0, 1)]
    [SerializeField] private int entity;
    [Range(0, 3)]
    [SerializeField] private int direction;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MessageStruct message = new MessageStruct();
            message.sender = sender;
            message.messageType = messageType;
            message.entity = entity;
            message.direction = direction;
            messageSentEvent?.Invoke(message);
        }
    }
}