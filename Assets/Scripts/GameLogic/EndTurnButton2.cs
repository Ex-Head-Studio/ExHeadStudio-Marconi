using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Gestisce il bottone di fine fase/turno e le sue animazioni
/// </summary>
[RequireComponent(typeof(PlanningPhaseStartListener))]
[RequireComponent(typeof(ActionPhaseStartListener))]
public class EndTurnButton2 : MonoBehaviour
{
    [Header("Animazione del bottone")]
    [SerializeField] private float rotationDuration = 0.5f;
    [SerializeField] private Ease rotationEase = Ease.OutBack;
    
    [Header("Eventi")]
    [Tooltip("Evento chiamato alla fine di ogni turno")] 
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private PlanningPhaseEndEvent planningPhaseEndEvent;
    [SerializeField] private ActionPhaseEndEvent actionPhaseEndEvent;
    [SerializeField] private Animator executeAnimator;
    [SerializeField] private Material materialButton;

    [SerializeField] private GameObject buttonGameObject;
    private Button button;
    private TMP_Text buttonTextComponent;
    
    // Flag per evitare chiamate multiple
    private bool isTransitioning = false;
    
    // Eventi per comunicare le transizioni
    public static event Action OnPhaseTransitionStarted;
    public static event Action OnPhaseTransitionEnded;

    /// <summary>
    /// Inizializzazione del bottone
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonTextComponent = GetComponentInChildren<TMP_Text>();
        executeAnimator.SetBool("CanExecute", true);
        
        // All'inizio il bottone è ruotato per il primo turno di planning
        buttonGameObject.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    /// <summary>
    /// Registrazione agli eventi
    /// </summary>
    private void OnEnable()
    {
        GameManager2.OnAllCardsDealt += OnAllCardsDealt;
    }

    /// <summary>
    /// Rimozione degli eventi
    /// </summary>
    private void OnDisable()
    {
        GameManager2.OnAllCardsDealt -= OnAllCardsDealt;
    }

    /// <summary>
    /// Chiamato quando tutte le carte sono state distribuite
    /// </summary>
    private void OnAllCardsDealt()
    {
        Debug.Log("EndTurnButton: Tutte le carte sono state distribuite");
        
        // Termina la fase di transizione
        isTransitioning = false;
        OnPhaseTransitionEnded?.Invoke();
        
        // Abilita il bottone
        button.interactable = true;
        executeAnimator.SetBool("CanExecute", true);
    }

    #region Gestione delle fasi di gioco

    /// <summary>
    /// Chiamato quando inizia la fase Planning
    /// </summary>
    public void OnPlanningPhaseStart()
    {
        Debug.Log("Inizio fase Planning");
        
        // Ruota il bottone per la fase Planning
        buttonGameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), rotationDuration)
            .SetEase(rotationEase);
        
        // Configura il bottone
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PlanningPhaseEnd);
        //buttonTextComponent.text = "Action phase";
        PlayPhaseButton();
    }

    /// <summary>
    /// Chiamato quando il giocatore termina la fase Planning
    /// </summary>
    public void PlanningPhaseEnd()
    {
        // Evita doppi click
        if (isTransitioning)
        {
            Debug.LogWarning("Bottone premuto durante una transizione. Ignorato.");
            return;
        }
        
        Debug.Log("Fine fase Planning richiesta dall'utente");
        
        // Imposta lo stato di transizione
        isTransitioning = true;
        OnPhaseTransitionStarted?.Invoke();
        
        // Disabilita il bottone
        button.interactable = false;
        executeAnimator.SetBool("CanExecute", false);
        
        // Emetti l'evento di fine fase Planning
        planningPhaseEndEvent?.Invoke(new VoidEvent(0));
    }

    /// <summary>
    /// Chiamato quando inizia la fase Action
    /// </summary>
    public void OnActionPhaseStart()
    {
        Debug.Log("Inizio fase Action");
        
        // Ruota il bottone per la fase Action
        buttonGameObject.transform.DOLocalRotate(new Vector3(0, 180, 0), rotationDuration)
            .SetEase(rotationEase);
        
        // Configura il bottone
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ActionPhaseEnded);
        //buttonTextComponent.text = "End Turn";
        PlayPhaseButton();
    }

    /// <summary>
    /// Chiamato quando il giocatore termina la fase Action
    /// </summary>
    public void ActionPhaseEnded()
    {
        // Evita doppi click
        if (isTransitioning)
        {
            Debug.LogWarning("Bottone premuto durante una transizione. Ignorato.");
            return;
        }
        
        Debug.Log("Fine fase Action richiesta dall'utente");
        
        // Imposta lo stato di transizione
        isTransitioning = true;
        OnPhaseTransitionStarted?.Invoke();
        
        // Disabilita il bottone
        button.interactable = false;
        executeAnimator.SetBool("CanExecute", false);
        
        // Emetti l'evento di fine fase Action
        actionPhaseEndEvent?.Invoke(new VoidEvent(0));
    }

    /// <summary>
    /// Riproduce il suono del bottone
    /// </summary>
    private FMOD.Studio.EventInstance bigButton;
    public void PlayPhaseButton()
    {
        bigButton = FMODUnity.RuntimeManager.CreateInstance("event:/UI/BigButton");
        bigButton.start();
        bigButton.release();
    }

    #endregion
}