using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Lo script deve essere associato al toggle dei messaggii
/// </summary>

[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(ToggleInformations))]
[RequireComponent(typeof(toggleSounds))]
[RequireComponent(typeof(DirectionIndicator))]


public class ToggleMessage : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, IConsumableObjectTest
{
    private Toggle toggle;
    private ToggleInformations toggleInfo;

    private int toggleEntity, toggleMoveId;
    private string toggleSender;


    [Header("Answer Stack ScriptableObject")]
    [SerializeField] private AnswerStack answerStack;


    //struct necessaria per mantenere un clone di ogni risposta nemica eliminata
    private AnswerStruct tmpRemovedEnemyAnswer;

    private void Start()
    {
        toggleInfo = GetComponent<ToggleInformations>();
        toggle = GetComponent<Toggle>();

        toggleEntity = toggleInfo.GetToggleEntity();
        toggleMoveId = toggleInfo.GetToggleMoveId();
        toggleSender = toggleInfo.GetToggleSender();

        tmpRemovedEnemyAnswer.receiver = "";

        //se il messaggio è nemico viene automaticamente aggiunto allo stack
        if(toggleEntity  == (int)Entity.enemy)
        {
            answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleMoveId, toggleSender, toggleEntity));
        }

    }

    #region Iscrivione agli eventi

    private void OnEnable()
    {
        IConsumableObjectTest.testActionForConsumable += OnConsumableObjectAction;

    }

    private void OnDestroy()
    {
        IConsumableObjectTest.testActionForConsumable -= OnConsumableObjectAction;
    }

    #endregion

    #region Creazione delle risposte (Set Answer)
    public void SetAnswer(bool toggleValue)
    {
        if(toggleEntity == (int)Entity.ally)
        {
            SetAllyAnswer();
        }
        else
        {
            SetEnemyAnswer();
        }
    }

    private void SetAllyAnswer()
    {
        // Se il toggle è stato attivato e prima era spento, aggiungo la risposta
        if (toggle != null && toggle.isOn)
        {
            if(toggleInfo.GetToggleEntity() == (int)Entity.ally)
            {
                if(answerStack.CountEntity((int)Entity.ally) == 0)
                {
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
                else if(answerStack.CountEntity((int)Entity.ally) > 0)
                {
                    answerStack.RemoveAnswerByFaction((int)Entity.ally);
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
            }
        
        }
    }

    //TODO verificare se questa logica funziona correttamente e se nel caso può essere estesa alle altre navi
    private void SetEnemyAnswer()
    {
        //Tutte le volte che un toggle nemico viene selezionato viene rimosso dallo stack delle risposte.
        //Devo mantenere una copia all'ultima risposta eliminata, per poterle reinserire se elimino un altro messaggio

        if(tmpRemovedEnemyAnswer.receiver != "")
        {
            answerStack.AddAnswer(tmpRemovedEnemyAnswer);
        }

        //l'azione selezionata deve passare la risposta negata
        tmpRemovedEnemyAnswer = new AnswerStruct(!toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()); 
        answerStack.RemoveEnemyAnswer(tmpRemovedEnemyAnswer);
    }
    #endregion

    #region  Consumabili
    public void OnConsumableObjectAction(string testString)
    {
        if(toggleInfo.GetToggleMessageType() == (int)MessageType.attack)
        {
            toggle.interactable = false;
            //TODO sistemare questa cosa del colore
            toggle.GetComponent<Image>().material.SetFloat("_EMISSION", 0.5f);
        }
    }
    #endregion

    //TODO capire perchè queste funzioni non possono essere invocate da toggleSounds
    #region Suoni
    public void OnPointerEnter(PointerEventData eventData)
    {
       Playhover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlaySelect();
    }

    private FMOD.Studio.EventInstance hoverMessage;

    public void Playhover()
    {
        hoverMessage = FMODUnity.RuntimeManager.CreateInstance("event:/UI/MessageHover");
        hoverMessage.start();
        hoverMessage.release();
    }

    private FMOD.Studio.EventInstance selectMessage;
    public void PlaySelect()
    {
        selectMessage = FMODUnity.RuntimeManager.CreateInstance("event:/UI/MessageSelection");
        selectMessage.start();
        selectMessage.release();
    }

    #endregion
}
