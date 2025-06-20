using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;

public class DisplayStatsClass
{
    public MonoBehaviour script;

    public DisplayStatsClass(MonoBehaviour script)
    {
        this.script = script;
    }
    

}
public class DisplayStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Script per il display delle statistiche degli oggetti

    //tipi di classe a cui fa riferimento
    private TileElusive tileScript; //TODO fare in modo che funzioni anche su altri tipi di Tile
    private AShip shipScript;
    private AbstractObstacle obstacleScript;

    public static event Action<DisplayStatsClass> OnEntityHoverStarted;
    public static event Action<DisplayStatsClass> OnEntityHoverEnded;

    private bool _isShip = false;

    private void Start()
    {
        //controlli per prendere lo script corretto
        TryGetComponent<AShip>(out shipScript);
        TryGetComponent<TileElusive>(out tileScript);
        TryGetComponent<AbstractObstacle>(out obstacleScript);
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        

        // Se non hai trovato nemici, continua con la logica normale di priorità
        if (shipScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass(shipScript));
        }
        else if (obstacleScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass(obstacleScript));
        }
        else if (tileScript != null)
        {
            OnEntityHoverStarted?.Invoke(new DisplayStatsClass(tileScript));
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (shipScript != null || tileScript != null || obstacleScript != null)
        {
            OnEntityHoverEnded?.Invoke(new DisplayStatsClass(null));
        }
    }


}
