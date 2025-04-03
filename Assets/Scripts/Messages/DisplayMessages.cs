using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;


/// <summary>
/// La classe si occupa del display dei messaggi nella UI e della compilazione delle risposte
/// </summary>
public class DisplayMessages : MonoBehaviour
{
    [Header("Messages Panel")]
    [Tooltip("Bisogna inserire i pannelli sui quali devono comparire i messaggi")]
    [SerializeField] private Transform allyStackTransform;
    [SerializeField] private Transform enemyStackTransform;
    [Header("Toggle Groups")]
    [SerializeField] private ToggleGroup allyToggleGroup;
    [SerializeField] private ToggleGroup enemyToggleGroup;
    [Header("Toggle values")]
    [SerializeField] Material allyColor;
    [SerializeField] Material enemyColor;
    
    [Header("Stack dei messaggi")]
    [SerializeField] private MessagesStack messagesStack; 
    [SerializeField] private AnswerStack answerStack;

    //cache dei dati
    private string[] directions;    

    //serve per determinare a quale toggle group appartnegono
    private Transform parent;
    private List<Toggle> toggles = new List<Toggle>();
    [SerializeField] private Toggle togglePrefab;
    private Toggle tmpToggle;

    private void Start()
    {
        togglePrefab = messagesStack.togglePrefab;
        directions = messagesStack.directions;
    }


    //qui mi iscrivo agli eventi

    //iscrizione all'evento che, attraverso il consumabile, permette di disabilitare i toggle
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

     public void DisplayMessage(MessageStruct message)
    {
        SetMessageColorAndTransform(message);
        SetMessageText(message);

        togglePrefab?.GetComponent<ToggleInformations>().SetInformations(message.sender, message.moveId, message.entity, message.direction, message.messageType);
        tmpToggle = Instantiate(togglePrefab, parent.position, Quaternion.Euler(90,0,0), parent);

        //Non modificare l'ordine di queste due righe
        tmpToggle?.GetComponent<ToggleInformations>().SetInformations(message.sender, message.moveId, message.entity, message.direction, message.messageType);
        toggles.Add(tmpToggle);
        messagesStack.AddMessage(message, parent);
    }

    private void SetMessageColorAndTransform(MessageStruct message)
    {
        if(message.entity == (int)Entity.ally)
        {
            togglePrefab.GetComponentInChildren<Image>().material = allyColor;
            parent = allyStackTransform;
            togglePrefab.group = allyToggleGroup;        
        }
        else
        {
            togglePrefab.GetComponentInChildren<Image>().material = enemyColor;
            parent = enemyStackTransform;
            togglePrefab.group = enemyToggleGroup;
        }
    }

    private void SetMessageText(MessageStruct message)
    {
        int messageType = message.messageType;
        switch(messageType)
        {
            case (int)MessageType.attack:
                togglePrefab.GetComponentInChildren<TMP_Text>().text = 
                message.sender + " attacks " + directions[message.direction];
                break;

            case (int)MessageType.movement:

                togglePrefab.GetComponentInChildren<TMP_Text>().text = 
                message.sender + " moves " + directions[message.direction]; 
                break;

            default:
                togglePrefab.GetComponentInChildren<TMP_Text>().text = 
                "Error, control switch statement in DisplayMessages"; 
                break;
        }
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

    public void SendAnswers()
    {
        answerStack.SendAnswers();
    }

    /*private void DisableToggles()
    {
        foreach(Toggle t in toggles)
        {
            t.GetComponent<ToggleMessage>().OnToggleDisable();
        }
    }*/
    
}
