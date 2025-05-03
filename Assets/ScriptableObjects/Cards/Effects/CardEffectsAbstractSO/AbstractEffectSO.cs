using UnityEngine;
using System;

    public struct EffectStruct
    {
        public AbstractCard card;
        public GameObject obj;
        

        public EffectStruct(AbstractCard card = null, GameObject obj = null)
        {
            this.card = card;
            this.obj = obj;
        }
    }


public abstract class AbstractEffectSO : ScriptableObject
{

    public static event Action<int> effectStartedEvent;
    public static event Action<int> effectEndedEvent;

    /// <summary>
    /// La funzione definisce l'effetto della carta.
    /// @note: Questa funzione deve essere implementata nelle classi derivate.
    /// @param name="effectStruct"> La classe wrapper che contiene i parametri necessari all'effetto.</param>
    /// </summary>
    public virtual void PerformEffect(EffectStruct effectStruct)
    {
        // Implementazione di default vuota, può essere sovrascritta dalle classi derivate
    }

    public void StartEffect(int id)
    {
        effectStartedEvent?.Invoke(id);
    }
    public void EndEffect(int id)
    {
        effectEndedEvent?.Invoke(id);
    }
}
