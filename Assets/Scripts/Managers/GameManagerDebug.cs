using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class GameManagerDebug : MonoBehaviour
{
    [SerializeField] private float timeBeforeStart = 2f;
    [SerializeField] private float timeBetweenRounds = 2f;

    private int numberOfRounds = 0;
    [SerializeField] private StartedTurnEvent startedTurnEvent;
    [SerializeField] private MessagesStack messagesStack;
    [SerializeField] private AnswerStack answerStack;
    private void Start()
    {
        answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();
        StartCoroutine(StartGame());
        //devo passare una struct vuota perchè il metodo invocato è void (Stefano)
        startedTurnEvent?.Invoke(new VoidEvent(0));
        OnTurnStarted();

    }

    public void OnTurnEnded()
    {
        Debug.Log("Turn Ends, game manager registered");
        StartCoroutine(WaitNextRound());

        //bisogna verificare il corretto ordine di esecuzione delle chiamate
        answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();
        startedTurnEvent?.Invoke(new VoidEvent(0));
        Debug.Log("Next turn started, count: ");
    }

    public void OnTurnStarted()
    {
        Debug.Log("Turn Started, game manager registered");
        numberOfRounds++;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(timeBeforeStart);
    }

    private IEnumerator WaitNextRound()
    {
        yield return new WaitForSeconds(timeBetweenRounds);
    }
}
