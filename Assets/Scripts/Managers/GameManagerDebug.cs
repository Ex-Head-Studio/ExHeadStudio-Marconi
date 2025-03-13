using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class GameManagerDebug : MonoBehaviour
{
    [SerializeField] private float timeBeforeStart = 10f;
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
    }

    public void OnTurnEnded()
    {
        Debug.Log("Turn Ends, game manager registered");
        StartCoroutine(WaitNextRound());
    }

    public void OnTurnStarted()
    {
        answerStack.RemoveAllAnswers();
        messagesStack.RemoveAllMessages();
        Debug.Log("Turn Started, game manager registered");
        numberOfRounds++;
        Debug.Log("Next turn started, count: " + numberOfRounds);
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
        Debug.Log("Start event called by OnTurnEnded");
    }
}
