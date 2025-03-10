using UnityEngine;

public class EndedTurnEventListener : AbstractEventListenerSO<VoidEvent>
{
    public void OnMessageReceived(VoidEvent voidEvent)
    {
        Debug.Log("Void event received with value: " + voidEvent.value);
    }
}
