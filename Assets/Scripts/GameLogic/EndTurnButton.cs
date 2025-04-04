using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    /// <summary>
    /// Lo script viene associato al tasto di conferma delle azioni
    /// </summary>
    /// /// 

    [Header("Events")]
    [Tooltip("L'evento viene chiamato alla fine di ogni turno, con la conferma del giocatore")] 
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private AnswerStack answerStack;
    [SerializeField] private Animator executeAnimator;
    [SerializeField] private Material materialButton;

    private Button  button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void EndTurn()
    {
        if(answerStack.CountEntity((int)Entity.enemy) >= 1 && answerStack.CountEntity((int)Entity.ally) >= 1)
        {
            //button.image.color = Color.green;
            
            Debug.Log("Eseguiamo l'animazione di fine turno");
            //chiama l'evento di fine turno
            VoidEvent voidEvent =  new VoidEvent(0);
            endedTurnEvent?.Invoke(voidEvent);

            //TODO controllare che sia corretto l'ordine di escuzione
            sendAsnwers();

        }
        else
        {
            executeAnimator.SetBool("CanExecute", false);
            //TODO Rob modifica il colore
            //button.image.color = Color.red;
        }

    }

    void Update(){
         if(answerStack.CountEntity((int)Entity.enemy) >= 1 && answerStack.CountEntity((int)Entity.ally) >= 1)
        {
            button.interactable = true;
            //materialButton.SetColor("_EmissionColor", Color.green);
            button.image.color = Color.green;
            executeAnimator.SetBool("CanExecute", true);
        }
        else
        {
            button.interactable = false;
            //materialButton.SetColor("_EmissionColor", Color.red);
            button.image.color = Color.red;
            executeAnimator.SetBool("CanExecute", false);
        }
    }

    public void sendAsnwers()
    {
        answerStack.SendAnswers();
    }
}
