using UnityEngine;

public class OnClearEventListener : AbstractEventListenerSO<VoidEvent>
{
    public void OnClearEvent()
    {
        Debug.Log("Clear Event");
    }
}

