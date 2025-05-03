using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton2 : MonoBehaviour
{
    /// <summary>
    /// Lo script viene associato al tasto di fine turno del giocatore
    /// </summary>

    [Header("Events")]
    [Tooltip("L'evento viene chiamato alla fine di ogni turno, con la conferma del giocatore")] 
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private Animator executeAnimator;
    [SerializeField] private Material materialButton;

    private Button  button;



    private AbstractCard cardToWait = null;

    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.image.color = Color.green;
        executeAnimator.SetBool("CanExecute", true);

    }

    private void OnEnable()
    {
        UICard.cardSelectedEvent += DisableButton;
        UICard.cardDeselectedEvent += EnableButton;
        AbstractCard.abstractCardUsed += EnableButton;
    }

    private void OnDisable()
    {
        UICard.cardSelectedEvent -= DisableButton;
        UICard.cardDeselectedEvent -= EnableButton;
        AbstractCard.abstractCardUsed -= EnableButton;
    }

    public void EndTurn()
    {
        endedTurnEvent?.Invoke(new VoidEvent(0));
    }

    public void DisableButton(AbstractCard cardUsed)
    {
            cardToWait = cardUsed;
            button.interactable = false;
            executeAnimator.SetBool("CanExecute", false);
            button.image.color = Color.red;
            materialButton.SetColor("_Color", Color.red);
    }
    public void EnableButton(AbstractCard cardUsed)
    {
        if(cardToWait == cardUsed)
        {
            button.interactable = true;
            executeAnimator.SetBool("CanExecute", true);
            button.image.color = Color.green;
            materialButton.SetColor("_Color", Color.green);
        }

    }
}