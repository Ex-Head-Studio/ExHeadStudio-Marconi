using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 
using System;
using UnityEngine.UI;   

public enum DeckType
{
    CommandDeck,
    SupportDeck,

    SpyDeck,
}

public class DeckDraw : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    //IMPORTANTE!! Aggiungere un raycaster alla camera perchè funzioni
    [SerializeField] private DeckType deckTypeEnum;

    [Tooltip("Bool che seleziona se la pescata dal mazzo ha un costo")]
    [SerializeField] private bool hasDrawingCost;

    [SerializeField] private GameObject deckObject;

    public static event Action<DeckType, bool> cardDrawed;
    public static event Action<DeckType, bool> deckOvering;
    public static event Action<DeckType, bool> deckOvered;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        cardDrawed?.Invoke(deckTypeEnum, hasDrawingCost);
        Debug.Log("Click on deck: " + gameObject.name);

        //gestire il caso del costo

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log("Hovering on deck: " + gameObject.name);
        deckOvering?.Invoke(deckTypeEnum, hasDrawingCost);
        //deckObject.GetComponent<Material>().color = Color.blue;
    }

    
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        deckOvered?.Invoke(deckTypeEnum, hasDrawingCost);
        //deckObject.GetComponent<Material>().color = Color.green;
    }

    

}
