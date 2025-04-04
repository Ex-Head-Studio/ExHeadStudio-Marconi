using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class TutorialManager : MonoBehaviour
{

    [Header("UI Elements")]
    [Tooltip("TextMeshProUGUI component to display the tutorial text.")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject backgroundPanel;

    [Header("Tutorial Messages")]
    public string[] tutorialMessages;
    [Header("Typing Values")]
    [SerializeField] private float Speed;

    [SerializeField] private GameObject tutorialCanvas;

    private Coroutine typingCoroutine; 
    private bool isTyping = false; 
    private int currentMessageIndex = 0;

    private void Start()
    {
        tutorialCanvas.SetActive(true);
    }

    void Update()
    {
        //TODO sistemare input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        if (isTyping)
        {
            // Funzione che permette di fermare la coroutine e finire completamente la frase cliccando spazio mentre si sta scrivendo
            StopCoroutine(typingCoroutine);
            tutorialText.text = tutorialMessages[currentMessageIndex];
            currentMessageIndex++; 
            isTyping = false;
        }
        else if (currentMessageIndex < tutorialMessages.Length)
        {
            tutorialText.text = "";
            typingCoroutine = StartCoroutine(TypeSentence());
            PerformActionBasedOnMessage(currentMessageIndex); // Serve per cambiare lo sfondo in base a quale messaggio è visualizzato
        }
        else
        {
            tutorialText.transform.parent.gameObject.SetActive(false); 
        }
    }

    void PerformActionBasedOnMessage(int messageIndex)
    {
        RectTransform rectTransform = backgroundPanel.GetComponent<RectTransform>();

        switch (messageIndex)
        {
            case 2:
            case 3:
                rectTransform.offsetMin = Vector2.zero; 
                rectTransform.offsetMax = Vector2.zero; 
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -300); // Set top to 300
                break;
            case 4:
            case 5:
                rectTransform.offsetMin = Vector2.zero; 
                rectTransform.offsetMax = Vector2.zero; 
                rectTransform.offsetMin = new Vector2(215, rectTransform.offsetMin.y); // Set left to 215
                break;
            case 6:
            case 7:
                rectTransform.offsetMin = Vector2.zero; 
                rectTransform.offsetMax = Vector2.zero; 
                rectTransform.offsetMax = new Vector2(400, rectTransform.offsetMax.y); // Set left to 400
                break;

            case 8:
                tutorialCanvas.SetActive(false); // Disattiva il canvas del tutorial
                break;
            default:
                rectTransform.offsetMin = Vector2.zero; 
                rectTransform.offsetMax = Vector2.zero; 
                break;
        }
    }

    IEnumerator TypeSentence()
    {
        isTyping = true;
        tutorialText.text = "";
        foreach (char letter in tutorialMessages[currentMessageIndex].ToCharArray())
        {
            tutorialText.text += letter;
            yield return new WaitForSeconds(Speed);
        }
        isTyping = false;
        currentMessageIndex++;
    }
}
