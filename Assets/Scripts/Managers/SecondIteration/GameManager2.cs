using UnityEngine;
using System.Collections;
using FMODUnity;
using System.Collections.Generic;
using Mono.Cecil;

/// <summary>
/// Il game manager si occupa della gestione dei tempi e dei turni di gioco
/// </summary>

public class GameManager2 : MonoBehaviour
{

    [SerializeField] private float timeBeforeStart = 1f;

    //questo parametro andrà modificato se introdurremo uno stack delle azioni
    [SerializeField] private float timeBetweenRounds = 1f;

    private int numberOfRounds = 0;

    //NOTA: l'evento di fine turno conicide con quello di inizio turno del nemico
    //Allo start del gioco, il primo a fare le sue azioni è il giocatore. Il nemico pianifica.
    
    [Header("Game Events to call")]
    [Tooltip("Evento di inzio turno")]
    //alla struct passo il numero di round
    [SerializeField] private StartedTurnEvent startedTurnEvent;
    private void Start()
    {
        StartCoroutine(StartGame());
    }


    public void OnTurnEnded()
    {   
        //Al termine del turno del giocatore, il nemico esegue le sue mosse
        //StartCoroutine(WaitNextRound());
    }

    //TODO controllare se gli animator sono tutti in idle
    public void OnTurnStarted()
    {
        numberOfRounds++;
    }

    public void OnEnemyTurnEnded()
    {
        Debug.Log("Turno nemico finito");
        StartCoroutine(WaitNextRound());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(timeBeforeStart);
        /// <summary>
        //Aggiungere il nuovo event
        /// </summary>
        /// <param name="VoidEvent(numberOfRounds)"></param>
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
        Debug.Log("Inizio partita, turno " + numberOfRounds);
    }

    private IEnumerator WaitNextRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startedTurnEvent?.Invoke(new VoidEvent(numberOfRounds));
        Debug.Log("Turno del giocatore numero " + numberOfRounds);

    }

    //TODO: implementare la logica di energia delle carte
}
