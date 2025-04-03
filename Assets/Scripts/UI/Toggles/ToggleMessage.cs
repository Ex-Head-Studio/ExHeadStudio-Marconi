using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
[RequireComponent(typeof(ToggleInformations))]
[RequireComponent(typeof(toggleSounds))]


public class ToggleMessage : MonoBehaviour, IPointerEnterHandler, ISubmitHandler
{
    private Toggle toggle;
    private ToggleInformations toggleInfo;

    private bool previousState;
    private ToggleGroup currentGroup;
    private ToggleGroup selectedGroup;

    [Header("Answer Stack ScriptableObject")]
    [SerializeField] private AnswerStack answerStack;


    private AnswerStruct tmpRemovedEnemyAnswer;

    private bool abilitaToggle = true;

    private void OnEnable()
    {
            UIObjectScript.testActionForConsumable += OnToggleDisable;
    }

    private void OnDisable()
    {
        //UIObjectScript.testActionForConsumable -= OnToggleDisable;
    }

    private void OnDestroy()
    {
        
        UIObjectScript.testActionForConsumable -= OnToggleDisable;
    }


    private void Start()
    {
        toggleInfo = GetComponent<ToggleInformations>();
        toggle = GetComponent<Toggle>();

        currentGroup = toggle.group;
        previousState = toggle.isOn;

        //TODO sistemare questa parte
        if(toggleInfo.GetToggleEntity() == (int)Entity.enemy)
        {
            answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
        }
    
        tmpRemovedEnemyAnswer.receiver = "";
    }
    public void SetAnswer(bool toggleValue)
    {
        if (toggle == null || toggleInfo == null) return;

        ToggleGroup selectedGroup = toggle.group;
        if (selectedGroup == null)
        {
            Debug.LogWarning("SelectedGroup non definito.");
            return;
        }

        toggleInfo.PrintInformations();

        // Se il toggle è stato attivato e prima era spento, aggiungo la risposta
        if (toggle.isOn)
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
            else
            {
                //vecchia logica, da sistemare
                /*if(answerStack.CountEntity((int)Entity.enemy) > 0)
                {
                    answerStack.RemoveAnswerByFaction((int)Entity.enemy);
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
                else if(answerStack.CountEntity((int)Entity.enemy) == 0)
                {
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));   
                }*/
            }

        }

        if(toggleInfo.GetToggleEntity() == (int)Entity.enemy)
        {
                //nuova logica. Tutte le volte che un toggle nemico viene selezionato viene rimosso dallo stack delle risposte
                //devo mantenere una reference all'ultima risposta eliminata, per poterle reinserire se elimino un altro messaggio

                if(tmpRemovedEnemyAnswer.receiver != "")
                {
                    answerStack.AddAnswer(tmpRemovedEnemyAnswer);
                }

                tmpRemovedEnemyAnswer = new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()); 
                answerStack.RemoveEnemyAnswer(tmpRemovedEnemyAnswer);
        }



    
    }

    //TODO voglio modificare questa cosa, è un pessimo prototipo
    public void OnToggleDisable(string consumableName)
    {
        if(toggleInfo.GetToggleMessageType() == (int)MessageType.attack)
        {
            toggle.interactable = false;
            toggle.colors.normalColor.Equals(Color.white);
        }

    }

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
