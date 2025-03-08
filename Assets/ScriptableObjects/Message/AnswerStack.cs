using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "AnswersStack", menuName = "Scriptable Objects/AnswersStack")]
public class AnswerStack : ScriptableObject
{
    [SerializeField] private MessageReceivedEvent answerSentEvent;
    [SerializeField] public ToggleGroup allyToggleGroup;
    [SerializeField] public ToggleGroup enemyToggleGroup;
    public List<AnswerStruct> answers = new List<AnswerStruct>();

    public void AddAnswer(AnswerStruct answer)
    {
        answers.Add(answer);
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

