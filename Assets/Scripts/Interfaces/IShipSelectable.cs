using UnityEngine;
using UnityEngine.EventSystems;

public interface IShipSelectable : IEventSystemHandler
{
    /// <summary>
    /// Interface for ships. Can receive a card as parameter
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnShipSelected(PointerEventData pointerEventData);
}