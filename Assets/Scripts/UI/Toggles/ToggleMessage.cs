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
    private void Start()
    {
        toggleInfo = GetComponent<ToggleInformations>();
        toggle = GetComponent<Toggle>();

        currentGroup = toggle.group;
        previousState = toggle.isOn;
    
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

                if(answerStack.CountEntity((int)Entity.enemy) > 0)
                {
                    answerStack.RemoveAnswerByFaction((int)Entity.enemy);
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
                else if(answerStack.CountEntity((int)Entity.enemy) == 0)
                {
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleMoveId(), toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));   
                }
            }

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

    public void OnToggleDisable()
    {
        //da modificare con la grafica
        toggle.interactable = false;
        toggle.colors.normalColor.Equals(Color.white);
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
