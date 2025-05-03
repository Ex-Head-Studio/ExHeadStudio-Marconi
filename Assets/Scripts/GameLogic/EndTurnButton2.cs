using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlanningPhaseStartListener))]
[RequireComponent(typeof(ActionPhaseStartListener))]

public class EndTurnButton2 : MonoBehaviour
{
    /// <summary>
    /// Lo script viene associato al tasto di fine turno del giocatore e gestisce in parte la scansione delle fasi
    /// </summary>

    [Header("Events")]
    [Tooltip("L'evento viene chiamato alla fine di ogni turno, con la conferma del giocatore")] 
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private PlanningPhaseEndEvent planningPhaseEndEvent;
    [SerializeField] private ActionPhaseEndEvent actionPhaseEndEvent;
    [SerializeField] private Animator executeAnimator;
    [SerializeField] private Material materialButton;

    private Button  button;
    private TMP_Text buttonTextComponent;

    private AbstractCard cardToWait = null;



    private void Awake()
    {
        button = GetComponent<Button>();
        buttonTextComponent = GetComponentInChildren<TMP_Text>();
        button.image.color = Color.green;
        executeAnimator.SetBool("CanExecute", true);
    }

    #region Iscrizione agli eventi senza Listener
    private void OnEnable()
    {
        UICard.cardSelectedEvent += DisableButton;
        UICard.cardDeselectedEvent += EnableButton;
        AbstractCard.abstractCardUsed += EnableButton;
    }

    private void OnDisable()
    {
        UICard.cardSelectedEvent -= DisableButton;
        UICard.cardDeselectedEvent -= EnableButton;
        AbstractCard.abstractCardUsed -= EnableButton;
    }

    #endregion

    #region Abilitazione del bottone

        public void DisableButton(AbstractCard cardUsed)
    {
            cardToWait = cardUsed;
            button.interactable = false;
            executeAnimator.SetBool("CanExecute", false);
            button.image.color = Color.red;
            materialButton.SetColor("_Color", Color.red);
    }
    public void EnableButton(AbstractCard cardUsed)
    {
        if(cardToWait == cardUsed)
        {
            button.interactable = true;
            executeAnimator.SetBool("CanExecute", true);
            button.image.color = Color.green;
            materialButton.SetColor("_Color", Color.green);
        }

    }

    #endregion

    #region Funzioni di callback e invocazione eventi

    //il bottone risponde agli eventi lanciati dal game manager.
    

    public void OnPlanningPhaseStart()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PlanningPhaseEnd);
        buttonTextComponent.text = "Start Action phase";
    }

    public void PlanningPhaseEnd()
    {
        planningPhaseEndEvent?.Invoke(new VoidEvent(0));
    }

    public void OnActionPhaseStart()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ActionPhaseEnded);
        buttonTextComponent.text = "End Turn";
    }

    public void ActionPhaseEnded()
    {
        actionPhaseEndEvent?.Invoke(new VoidEvent(0));
    }

    #endregion
}