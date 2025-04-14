using UnityEngine;


public abstract class AbstractEffectSO : ScriptableObject
{
    /// <summary>
    /// La funzione definisce l'effetto della carta.
    /// @note: Questa funzione deve essere implementata nelle classi derivate.
    /// @param name="card">La carta a cui è associato l'effetto.</param>
    /// @param name="obj">L'oggetto a cui si applica l'effetto.</param>
    /// </summary>
    public virtual void PerformEffect(AbstractCard card, GameObject obj)
    {
        // Implementazione di default vuota, può essere sovrascritta dalle classi derivate
    }
}
