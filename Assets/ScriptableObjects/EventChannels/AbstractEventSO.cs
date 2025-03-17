using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractEventSO<T>: ScriptableObject
{
    //TODO qui c'è un errore con la lista alla fine, bisogna trovare una soluzione
    public T testingValue;
    private List<AbstractEventListenerSO<T>> listeners;

    public void RegisterListener(AbstractEventListenerSO<T> listener)
    {
        if (listeners == null)
            listeners = new List<AbstractEventListenerSO<T>>();
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(AbstractEventListenerSO<T> listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }

    public void Invoke(T value)
    {
        /*foreach(AbstractEventListenerSO<T> listener in listeners)
        {
            listener.Listen(value);
        }*/
        for (int i = 0; i < listeners.Count; i++)
        {
            AbstractEventListenerSO<T> listener = listeners[i];
            listener.Listen(value);

        }
    }
}
