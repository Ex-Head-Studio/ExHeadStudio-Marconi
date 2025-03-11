using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "AnswersStack", menuName = "Scriptable Objects/AnswersStack")]
public class AnswerStack : ScriptableObject
{

    //inserimento in coda e cancellazione in testa per le Liste
    [SerializeField] private MessageReceivedEvent answerSentEvent;
    public List<AnswerStruct> answers = new List<AnswerStruct>();
    public void AddAnswer(AnswerStruct answer)
    {
        answers.Add(answer);
    }

    
    public void RemoveAnswer(string sender)
    {
        answers.Remove(answers.Find(x => x.receiver == sender));
    }

    public void SendAnswers()
    {
        foreach(AnswerStruct answer in answers)
        {
            answerSentEvent?.Invoke(answer);
        }
        RemoveAllAnswers();
    }
    public void RemoveAllAnswers()
    {
        answers.Clear();
    }
}

