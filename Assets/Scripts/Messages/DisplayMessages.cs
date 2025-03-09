using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DisplayMessages : MonoBehaviour
{

    [SerializeField] private Transform allyStackTransform;
    [SerializeField] private Transform enemyStackTransform;
    [SerializeField] private ToggleGroup allyToggleGroup;
    [SerializeField] private ToggleGroup enemyToggleGroup;
    [SerializeField] private MessagesStack messagesStack; 
    [SerializeField] private AnswerStack answerStack;

    private string[] directions;
    
    private Transform parent;

    private List<Toggle> toggles = new List<Toggle>();
    private Toggle togglePrefab;
    private Toggle tmpToggle;

    private void Start()
    {
        togglePrefab = messagesStack.togglePrefab;
        directions = messagesStack.directions;
    }

     public void DisplayMessage(MessageStruct message)
    {
        if(message.entity == (int)Entity.ally)
        {
            togglePrefab.GetComponentInChildren<Image>().color = Color.green;
            parent = allyStackTransform;
        }
        else
        {
            togglePrefab.GetComponentInChildren<Image>().color = Color.red;
            parent = enemyStackTransform;
        }

        if(message.messageType == (int)MessageType.attack)
        {
            togglePrefab.GetComponentInChildren<TMP_Text>().text = 
            message.sender + " attacks " + directions[message.direction];
        }
        else if(message.messageType == (int)MessageType.movement)
        {
            togglePrefab.GetComponentInChildren<TMP_Text>().text = 
            message.sender + " moves " + directions[message.direction]; 
        }

        if(message.entity == (int)Entity.ally)
        {
            togglePrefab.group = allyToggleGroup;
        }
        else
        {
            togglePrefab.group = enemyToggleGroup;
        }

        togglePrefab?.GetComponent<ToggleInformations>().SetInformations(message.sender, message.entity, message.direction, message.messageType);
        tmpToggle = Instantiate(togglePrefab, parent.position  , Quaternion.identity, parent);
        toggles.Add(tmpToggle);
        messagesStack.AddMessage(message, parent);
    }

    public void RemoveAllMessages()
    {
        messagesStack.RemoveAllMessages();
        if(toggles.Count == 0) return;  
        foreach(Toggle toggle in toggles)
        {
            Destroy(toggle.gameObject);
        }
        toggles.Clear();
    }

    public void sendAsnwers()
    {
        answerStack.SendAnswers();
    }
    
    /*private List<Button> buttons = new List<Button>();
    private Button buttonPrefab;



    private void Start()
    {
        buttonPrefab = messagesStack.buttonPrefab;
        directions = messagesStack.directions;
    }
    public void DisplayMessage(MessageStruct message)
    {
        if(message.entity == (int)Entity.ally)
        {
            buttonPrefab.GetComponentInChildren<Image>().color = Color.green;
            parent = allyStackTransform;
        }
        else
        {
            buttonPrefab.GetComponentInChildren<Image>().color = Color.red;
            parent = enemyStackTransform;
        }

        if(message.messageType == (int)MessageType.attack)
        {
            buttonPrefab.GetComponentInChildren<TMP_Text>().text = 
            message.sender + " attacks " + directions[message.direction];
        }
        else if(message.messageType == (int)MessageType.movement)
        {
            buttonPrefab.GetComponentInChildren<TMP_Text>().text = 
            message.sender + " moves " + directions[message.direction]; 
        }

        buttons.Add(Instantiate(buttonPrefab, parent.position  , Quaternion.identity, parent));
    }

    public void RemoveAllMessages()
    {
        messagesStack.RemoveAllMessages();
        if(buttons.Count == 0) return;  
        foreach(Button button in buttons)
        {
            Destroy(button.gameObject);
        
        }
        buttons.Clear();
    }*/

}
