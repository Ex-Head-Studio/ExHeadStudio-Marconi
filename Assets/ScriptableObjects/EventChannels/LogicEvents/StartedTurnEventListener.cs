using UnityEngine;

public class StartedTurnEventListener : AbstractEventListenerSO<VoidEvent>
{

    public void OnTurnStarted(VoidEvent voidEvent)
    {
        Debug.Log("Started Turn");
    }
}
