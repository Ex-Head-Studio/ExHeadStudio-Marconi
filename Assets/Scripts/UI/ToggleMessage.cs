using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMessage : MonoBehaviour
{

    [Header("Bisogna sistemare un problema con le sprite")]
    private Toggle toggle;
    private ToggleInformations toggleInfo;

    private bool previousState;

    [SerializeField] private AnswerStack answerStack;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggleInfo = GetComponent<ToggleInformations>();
        previousState = toggle.isOn;
    }

    //questa va capita un po' meglio
    /*private void Update()
    {
        if (toggle.isOn)
            toggle.image.sprite = toggle.spriteState.highlightedSprite;
        else 
            toggle.image.sprite = toggle.spriteState.disabledSprite;
    }*/


    public void SetAnswer()
    {
        //devo ragionare sulla logica per i controlli, è fatto in fretta
        if(toggle.isOn && !previousState)
        {
            Debug.Log("Toggle changed:\nSender: " + toggleInfo.GetToggleSender() + " Entity: " + toggleInfo.GetToggleEntity());
            answerStack.AddAnswer(new AnswerStruct(toggle.isOn, toggleInfo.GetToggleSender(), toggleInfo.GetToggleEntity()));
        }
        else
        {
            answerStack.RemoveAnswer(toggleInfo.GetToggleSender());
        }

        previousState = toggle.isOn;
    }
}
