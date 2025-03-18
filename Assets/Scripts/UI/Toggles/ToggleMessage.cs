using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMessage : MonoBehaviour
{

    [Header("Bisogna sistemare un problema con le sprite")]
    private Toggle toggle;
    private ToggleInformations toggleInfo;

    private bool previousState;
    private ToggleGroup currentGroup;
    private ToggleGroup selectedGroup;

    [SerializeField] private AnswerStack answerStack;
    private void Start()
    {
        toggleInfo = GetComponent<ToggleInformations>();
        if (toggleInfo == null)
        {
            Debug.LogError("ToggleInformations non trovato nel GameObject.");
            return;
        }

        toggle = GetComponent<Toggle>();

        /*toggle = toggleInfo.GetToggle();
        if (toggle == null)
        {
            Debug.LogError("Toggle non trovato.");
            return;
        }*/

        //currentGroup = toggleInfo.GetToggleGroup();
        currentGroup = toggle.group;
        previousState = toggle.isOn;
    
    }

    //questa va capita un po' meglio
    /*private void Update()
    {
        if (toggle != null && toggle.image != null)
        {
            if (toggle.spriteState.highlightedSprite != null && toggle.spriteState.disabledSprite != null)
            {
                toggle.image.sprite = toggle.isOn ? toggle.spriteState.highlightedSprite : toggle.spriteState.disabledSprite;
            }
        }
    }*/


    public void SetAnswer(bool toggleValue)
    {
        //selectedGroup = toggle.group;
        //devo ragionare sulla logica per i controlli, è fatto in fretta, verificare se serve tenere il previous state
        //se il toggle è attivo aggiungo la risposta alla lista, altrimenti la rimuovo
        /*if(toggle.isOn && !previousState)
        {
            Debug.Log("Toggle changed:\nSender: " + toggleInfo.GetToggleSender() + " Entity: " + toggleInfo.GetToggleEntity());
            answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
        }
        else if(currentGroup == selectedGroup)
        {
            answerStack.RemoveAnswer(toggleInfo.GetToggleSender());
        }

        previousState = toggle.isOn;
        currentGroup = selectedGroup;*/


        if (toggle == null || toggleInfo == null) return;

        // Verifica se il gruppo selezionato è valido (se usi selectedGroup, assicurati che sia definito)
        ToggleGroup selectedGroup = toggle.group;
        if (selectedGroup == null)
        {
            Debug.LogWarning("SelectedGroup non definito.");
            return;
        }

        // Se il toggle è stato attivato e prima era spento, aggiungo la risposta
        if (toggle.isOn)// && !previousState)
        {
            if(toggleInfo.GetToggleEntity() == (int)Entity.ally)
            {
                if(answerStack.CountEntity((int)Entity.ally) == 0)
                {
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
                else if(answerStack.CountEntity((int)Entity.ally) > 0)
                {
                    answerStack.RemoveAnswerByFaction((int)Entity.ally);
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
            }
            else
            {

                if(answerStack.CountEntity((int)Entity.enemy) > 0)
                {
                    answerStack.RemoveAnswerByFaction((int)Entity.enemy);
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
                }
                else if(answerStack.CountEntity((int)Entity.enemy) == 0)
                {
                    answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));   
                }
            }
 

            //Debug.Log($"Toggle changed:\nSender: {toggleInfo.GetToggleSender()} Entity: {toggleInfo.GetToggleEntity()}");
            
        }
        //Aggiorno gli stati
        //previousState = toggle.isOn;
    }
}
