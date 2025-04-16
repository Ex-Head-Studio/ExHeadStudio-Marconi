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

    /// <summary>
    /// This method adds methods to call via code
    /// </summary>
    /// <summary>
    /// <param name="action"> 
    /// The method you want to add and be called (don't use parenthese)
    /// </param>
    /// </summary>
    public virtual void AddMethodToExecute(UnityAction<T> action)
    {
        onEvent.AddListener(action);
    }
}
