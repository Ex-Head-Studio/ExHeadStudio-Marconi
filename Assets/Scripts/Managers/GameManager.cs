using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Il game manager si occupa della gestione dei tempi e dei turni di gioco
    /// </summary>
    [SerializeField] private float timeBeforeStart = 10f;
    [SerializeField] private float timeBetweenRounds = 2f;

    private int numberOfRounds = 0;
    
    [Header("Game Events to call")]
    [Tooltip("Evento di inzio turno")]
    [SerializeField] private StartedTurnEvent startedTurnEvent;
    [Tooltip("Evento di reset")]
    [SerializeField] private OnClearEvent clearEvent;

    [Header("Stack dei messaggi")]

    [Tooltip("Messaggi inviati alle navi")]
    [SerializeField] private MessagesStack messagesStack;
    [Tooltip("Risposte inviate dal giocatore")]
    [SerializeField] private AnswerStack answerStack;

    private void Start()
    {
        answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();
        StartCoroutine(StartGame());
    }


    public void OnTurnEnded()
    {
        //qui bisogna passare un evento vuoto "fittizio" (stefano)
        clearEvent?.Invoke(new VoidEvent(0));


        //la coroutine serve a dare il tempo al sistema di eseguire tutte le operazioni 
        // prima del prossimo turno
        StartCoroutine(WaitNextRound());
    }

    //TODO controllare se gli animator sono tutti in idle
    public void OnTurnStarted()
    {
        Debug.Log("Turn starts, game manager registered, count: " + numberOfRounds);

        answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();

        numberOfRounds++;
    }

    private IEnumerator StartGame()
    {
        //devo passare una struct vuota perchè il metodo invocato è void (Stefano)
        yield return new WaitForSeconds(timeBeforeStart);
        startedTurnEvent?.Invoke(new VoidEvent(0));
    }

    private IEnumerator WaitNextRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
        startedTurnEvent?.Invoke(new VoidEvent(0));
    }
}
