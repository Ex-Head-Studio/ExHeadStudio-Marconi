using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

    public void RemoveEnemyAnswer(AnswerStruct answer)
    {
        answers.Remove(answer);
    }
    public void RemoveAnswerByFaction(int faction)
    {
        answers.RemoveAll(x => x.entity == faction);
    }

    public void SendAnswers()
    {
        foreach(AnswerStruct answer in answers)
        {
            answerSentEvent?.Invoke(answer);
        }
    }

    public int CountEntity(int entityEnumValue)
    {
        return answers.Count(x => x.entity == entityEnumValue);
    }
    public void RemoveAllAnswers()
    {
        answers.Clear();
    }
}

