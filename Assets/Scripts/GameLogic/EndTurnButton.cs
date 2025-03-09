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
            Debug.Log("You cannot end the turn without sending at least 2 answers");
            return;
        }
        VoidEvent voidEvent =  new VoidEvent(0);
        endedTurnEvent?.Invoke(voidEvent);
    }
}
