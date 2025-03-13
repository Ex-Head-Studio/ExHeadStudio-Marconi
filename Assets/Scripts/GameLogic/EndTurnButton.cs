using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private AnswerStack answerStack;

    private Button  button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void EndTurn()
    {
        if(answerStack.answers.Count <2)
        {
            button.image.color = Color.red;
            Debug.Log("You cannot end the turn without sending at least 2 answers");
        }
        else
        {
            button.image.color = Color.green;
            VoidEvent voidEvent =  new VoidEvent(0);
            endedTurnEvent?.Invoke(voidEvent);
            sendAsnwers();
        }

    }

    public void sendAsnwers()
    {
        Debug.Log("Ordini confermati, invio le risposte");
        answerStack.SendAnswers();
    }
}
