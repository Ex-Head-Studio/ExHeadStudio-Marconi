using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractEventListenerSO<T>: MonoBehaviour
{
    public AbstractEventSO<T> eventToListen;
    public UnityEvent<T> onEvent;

    private void OnEnable()
    {
        eventToListen.RegisterListener(this);
    }

    private void OnDisable()
    {
        eventToListen.UnregisterListener(this);
    }

    public void Listen(T value)
    {
        onEvent?.Invoke(value);
    }
}
