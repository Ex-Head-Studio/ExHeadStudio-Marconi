using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMessage : MonoBehaviour
{

    [Header("Bisogna sistemare un problema con le sprite")]
    private Toggle toggle;
    private ToggleGroup toggleGroup;

    [SerializeField] private AnswerStack answerStack;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggleGroup = GetComponent<ToggleGroup>();
    }

    //questa va capita un po' meglio
    private void Update()
    {
        if (toggle.isOn)
            toggle.image.sprite = toggle.spriteState.highlightedSprite;
        else 
            toggle.image.sprite = toggle.spriteState.disabledSprite;
    }


    public void SetAnwer(bool toggleValue)
    {
        //devo prendere il nome della nave e il valore e anche il toggle
    }
}
