using UnityEngine;
using System.Collections;

    /// <summary>
    /// Il game manager si occupa della gestione dei tempi e dei turni di gioco
    /// </summary>
public class GameManager2 : MonoBehaviour
{

    [SerializeField] private float timeBeforeStart = 1f;
    [SerializeField] private float timeBetweenRounds = 1f;

    private int numberOfRounds = 0;
    
    [Header("Game Events to call")]
    [Tooltip("Evento di inzio turno")]
    [SerializeField] private StartedTurnEvent startedTurnEvent;

    [Header("Stack dei messaggi")]

    [Tooltip("Messaggi inviati alle navi")]
    [SerializeField] private MessagesStack messagesStack;
    [Tooltip("Risposte inviate dal giocatore")]
    [SerializeField] private AnswerStack answerStack;

    private void Start()
    {
        /*answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();*/
        StartCoroutine(StartGame());
    }


    public void OnTurnEnded()
    {   
        StartCoroutine(WaitNextRound());
    }

    //TODO controllare se gli animator sono tutti in idle
    public void OnTurnStarted()
    {
        /*answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();*/

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

    //TODO: implementare la logica di energia delle carte
}
