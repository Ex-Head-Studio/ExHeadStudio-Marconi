using UnityEngine;
using System.Collections;
using FMODUnity;
using System.Collections.Generic;

/// <summary>
/// Il game manager si occupa della gestione dei tempi e dei turni di gioco
/// </summary>
/// 
/// 


//Aggiungo gli ascoltatori agli eventi
[RequireComponent(typeof(StartedGameListener))]
[RequireComponent(typeof(StartedTurnEventListener))]
[RequireComponent(typeof(PlanningPhaseEndListener))]
[RequireComponent(typeof(ActionPhaseEndListener))]

public class GameManager2 : MonoBehaviour
{

    [Header("Timing")]
    [SerializeField] private float timeBeforeStartGame = 1f;
    [SerializeField] private float timeBetweenRounds = 1f;
    
    [Header("Game Events to call")]

    [Tooltip("Evento di inzio partita/gioco")]
    [SerializeField] private StartedGameEvent startedGameEvent;

    [Tooltip("Evento di inzio turno")]
    [SerializeField] private StartedTurnEvent startedTurnEvent;
    [Tooltip("Evento di fine turno")]
    [SerializeField] private EndedTurnEvent endedTurnEvent;

    [Tooltip("Eventi per le fasi del turno del giocatore")]
    [SerializeField] private PlanningPhaseStartEvent planningPhaseStartEvent;
    [SerializeField] private ActionPhaseStartEvent actionPhaseStartEvent;

    private int numberOfRounds = 0;
    private void Awake()
    {
        StartCoroutine(StartGame());
    }

    #region Funzioni di callback per gli eventi
    /// <summary>
    /// La fine delle fasi e del turno del giocatore vengono gestiti dal bottone EndTurnButton2
    /// </summary>
    public void OnGameStarted()
    {
        Debug.Log("Game started");
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
    }

    public void OnTurnStarted()
    {
        numberOfRounds++;
        planningPhaseStartEvent?.Invoke(new VoidEvent(0));
    }

    public void OnPlanningPhaseEnded()
    {
        actionPhaseStartEvent?.Invoke(new VoidEvent(0));
    }

    public void OnActionPhaseEnded()
    {
        endedTurnEvent?.Invoke(new VoidEvent(0));
    }

    public void OnEnemyTurnEnded()
    {
        StartCoroutine(WaitNextRound());
    }

    #endregion

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(timeBeforeStartGame);
        startedGameEvent?.Invoke(new VoidEvent(0));
    }

    private IEnumerator WaitNextRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
    }
}
