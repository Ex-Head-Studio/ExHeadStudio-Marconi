using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Collections;

[RequireComponent(typeof(PlanningPhaseStartListener))]
[RequireComponent(typeof(ActionPhaseStartListener))]
public class EndTurnButton2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Lo script viene associato al tasto di fine turno del giocatore e gestisce in parte la scansione delle fasi
    /// </summary>

    [Header("Events")]
    [Tooltip("L'evento viene chiamato alla fine di ogni turno, con la conferma del giocatore")] 
    [SerializeField] private EndedTurnEvent endedTurnEvent;
    [SerializeField] private PlanningPhaseEndEvent planningPhaseEndEvent;
    [SerializeField] private ActionPhaseEndEvent actionPhaseEndEvent;
    //[SerializeField] private Animator executeAnimator;
    
    [Header("Rotatable Button")]
    [SerializeField] private GameObject buttonGameObject;
    
    [Header("Materials")]
    [SerializeField] private Material materialButtonTactic; // Materiale per fase tattica
    [SerializeField] private Material materialButtonEngage; // Materiale per fase di combattimento
    [SerializeField] private float normalIntensity = 0f;
    [SerializeField] private float hoverIntensity = 3f;
    [SerializeField] private float transitionDuration = 0.3f;

    [Header("Blinking Settings")]
    [SerializeField] private float blinkRate = 0.5f; // Durata completa di un ciclo di lampeggiamento
    
    [Header("Energy Check")]
    [SerializeField] private EnergySystem energySystem;
    [SerializeField] private PlayerHandManagerScript handManager; // Riferimento alla mano del giocatore

    [Header("Phase Text")]
    [SerializeField] private TextMeshProUGUI phaseText; // Riferimento al testo che mostra la fase corrente
    [SerializeField] private string tacticPhaseText = "TACTIC PHASE";
    [SerializeField] private string actionPhaseText = "ACTION PHASE";
    [SerializeField] private string enemyTurnText = "ENEMY TURN";

    [Header("Camera Background")]
    [SerializeField] private Camera mainCamera; // Riferimento alla camera principale
    [SerializeField] private Color tacticPhaseBackground = new Color(0.05f, 0.05f, 0.2f); // Blu scuro
    [SerializeField] private Color actionPhaseBackground = new Color(0.2f, 0.05f, 0.05f); // Rosso scuro
    [SerializeField] private Color enemyTurnBackground = new Color(0.2f, 0.2f, 0.05f); // Giallo scuro
    [SerializeField] private float backgroundTransitionDuration = 1f; // Durata della transizione del colore

    private Button button;
    private bool isBlinking = false;
    private Tween blinkTween;
    private Tween hoverTween;
    private Material currentActiveMaterial;

    private void Awake()
    {
        button = GetComponent<Button>();
        currentActiveMaterial = materialButtonTactic;
    }

    private void Start()
    {
        // Imposta colori HDR base per i materiali 
        if (materialButtonTactic != null)
        {
            // Imposta colore base blu per la fase tattica
            materialButtonTactic.SetColor("_EmissionColor", new Color(0, 0.5f, 1f, 1f));
            SetMaterialIntensity(materialButtonTactic, normalIntensity);
        }
        
        if (materialButtonEngage != null)
        {
            // Imposta colore base rosso per la fase di combattimento
            materialButtonEngage.SetColor("_EmissionColor", new Color(1f, 0.2f, 0.2f, 1f));
            SetMaterialIntensity(materialButtonEngage, normalIntensity);
        }
        
        // Avvia il controllo periodico
        InvokeRepeating("CheckPlayableCardsState", 0.1f, 0.5f);
    }

    private void CheckPlayableCardsState()
    {
        // Verifica se lo stato è cambiato
        CheckEnergyAndBlink();
    }

    #region Material Effects

    // Aumenta l'intensità quando il mouse è sopra il pulsante
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentActiveMaterial == null) return;

        // Ferma il blinking se attivo
        StopBlinking();

        // Animazione verso l'intensità hover
        hoverTween = DOTween.To(
            () => GetMaterialIntensity(currentActiveMaterial),
            x => SetMaterialIntensity(currentActiveMaterial, x),
            hoverIntensity,
            transitionDuration
        );
        
        Debug.Log("Hover effect started on button: " + gameObject.name);
    }

    // Diminuisce l'intensità quando il mouse esce dal pulsante
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentActiveMaterial == null) return;
        
        // Ferma l'animazione hover se attiva
        if (hoverTween != null && hoverTween.IsActive())
            hoverTween.Kill();
        
        // Ripristina l'intensità normale
        SetMaterialIntensity(currentActiveMaterial, normalIntensity);
        
        // Se l'energia è insufficiente, riavvia il blinking
        CheckEnergyAndBlink();
    }
    
    // Imposta l'intensità dell'emissione di un materiale
    private void SetMaterialIntensity(Material material, float intensity)
    {
        if (material == null) return;
        
        // Ottieni il colore base dell'emissione
        Color baseColor = material.GetColor("_EmissionColor");
        
        // Calcola la magnitudine massima delle componenti RGB
        float maxComponent = Mathf.Max(baseColor.r, Mathf.Max(baseColor.g, baseColor.b));
        
        // Se il massimo componente è maggiore di zero, normalizza
        if (maxComponent > 0)
        {
            // Normalizza mantenendo il rapporto tra i componenti
            float factor = intensity / maxComponent;
            Color newColor = new Color(
                baseColor.r * factor,
                baseColor.g * factor,
                baseColor.b * factor,
                baseColor.a
            );
            
            material.SetColor("_EmissionColor", newColor);
        }
    }
    
    // Ottiene l'intensità corrente dell'emissione di un materiale
    private float GetMaterialIntensity(Material material)
    {
        if (material == null) return 0;
        
        Color emissionColor = material.GetColor("_EmissionColor");
        return Mathf.Max(emissionColor.r, Mathf.Max(emissionColor.g, emissionColor.b));
    }
    
    // Avvia/ferma il lampeggiamento in base all'energia disponibile
    private void CheckEnergyAndBlink()
    {
        if (energySystem == null || currentActiveMaterial == null) return;
        
        // Invece di controllare un valore minimo fisso, verifica se almeno una carta è giocabile
        if (!IsAnyCardPlayable())
            StartBlinking();
        else
            StopBlinking();
    }
    
    // Avvia l'effetto di lampeggiamento
    private void StartBlinking()
    {
        if (isBlinking || currentActiveMaterial == null) return;
        
        isBlinking = true;
        
        // Crea un'animazione di lampeggiamento usando DOTween
        blinkTween = DOTween.Sequence()
            .Append(DOTween.To(
                () => GetMaterialIntensity(currentActiveMaterial),
                x => SetMaterialIntensity(currentActiveMaterial, x),
                hoverIntensity,
                blinkRate / 2
            ))
            .Append(DOTween.To(
                () => GetMaterialIntensity(currentActiveMaterial),
                x => SetMaterialIntensity(currentActiveMaterial, x),
                normalIntensity,
                blinkRate / 2
            ))
            .SetLoops(-1) // Loop infinito
            .Play();
    }
    
    // Ferma l'effetto di lampeggiamento
    private void StopBlinking()
    {
        if (!isBlinking) return;
        
        isBlinking = false;
        
        if (blinkTween != null && blinkTween.IsActive())
            blinkTween.Kill();
        
        if (currentActiveMaterial != null)
            SetMaterialIntensity(currentActiveMaterial, normalIntensity);
    }

    // Aggiungi questo metodo per verificare se almeno una carta è giocabile
    private bool IsAnyCardPlayable()
    {
        if (energySystem == null || handManager == null) return true;
        
        // Ottieni le carte nella mano del giocatore
        List<GameObject> cardsInHand = handManager.GetCardsInHand();
        
        // Se non ci sono carte, nulla da controllare
        if (cardsInHand.Count == 0) return true;
        
        // Verifica se almeno una carta è giocabile
        foreach (GameObject cardObj in cardsInHand)
        {
            AbstractCard card = cardObj.GetComponent<AbstractCard>();
            if (card != null && card.GetCardCost() <= energySystem.currentEnergy)
            {
                // Trovata almeno una carta giocabile
                return true;
            }
        }
        
        // Nessuna carta giocabile
        return false;
    }
    
    private void OnEnable()
    {
        CheckEnergyAndBlink();
    }

    private void OnDisable()
    {
        StopBlinking();
    }

    #endregion

    #region Abilitazione del bottone

    public void DisableButton(AbstractCard cardUsed)
    {
        button.interactable = false;
        //executeAnimator.SetBool("CanExecute", false);
        //button.image.color = Color.red;
    }
    
    public void EnableButton(AbstractCard cardUsed)
    {
        button.interactable = true;
        //executeAnimator.SetBool("CanExecute", true);
        //button.image.color = Color.green;
        
        // Controlla energia dopo aver abilitato il pulsante
        CheckEnergyAndBlink();
    }

    #endregion

    #region Funzioni di callback e invocazione eventi

    public void OnPlanningPhaseStart()
    {
        if (buttonGameObject != null)
            buttonGameObject.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f); // Rotazione LOCALE
        
        PlayPhaseButton();
        
        // Disabilita inizialmente il bottone
        button.interactable = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PlanningPhaseEnd);

        currentActiveMaterial = materialButtonTactic;

        // Aggiorna il testo della fase
        UpdatePhaseText(tacticPhaseText);

        // Aggiorna il colore di sfondo
        UpdateCameraBackground(tacticPhaseBackground);

        

        if (handManager != null)
            StartCoroutine(EnableCardsAndButtonAfterDelay());
    }

    private IEnumerator EnableCardsAndButtonAfterDelay()
    {
        // Attendi che la distribuzione delle carte sia completata
        yield return new WaitForSeconds(1.2f); // Aumentato a 1.2 secondi per garantire che tutte le carte siano distribuite
        
        // Abilita le carte
        handManager.SetAllCardsSelectable(true);
        
        // Verifica se ci sono carte giocabili e abilita il bottone
        button.interactable = true;
        
        // Controlla se il bottone deve lampeggiare
        CheckEnergyAndBlink();
        
        Debug.Log("Carte e bottone abilitati dopo la distribuzione");
    }

    public void PlanningPhaseEnd()
    {
        // Disabilita immediatamente il bottone come prima azione
        button.interactable = false;
        
        // Poi procedi con il resto delle operazioni
        planningPhaseEndEvent?.Invoke(new VoidEvent(0));
        
        if (buttonGameObject != null)
            buttonGameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f); // Rotazione LOCALE

        PlayPhaseButton();

        StopBlinking();

        if (handManager != null)
            handManager.SetAllCardsSelectable(false);
    }

    public void OnActionPhaseStart()
    {
        if (buttonGameObject != null)
            buttonGameObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f); // Rotazione LOCALE

        
        
        // Disabilita inizialmente il bottone
        button.interactable = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ActionPhaseEnded);
        
        currentActiveMaterial = materialButtonEngage;
        
        // Aggiorna il testo della fase
        UpdatePhaseText(actionPhaseText);
        
        // Aggiorna il colore di sfondo
        UpdateCameraBackground(actionPhaseBackground);
        
        
        
        if (handManager != null)
            StartCoroutine(EnableCardsAndButtonAfterDelay());
    }

    public void ActionPhaseEnded()
    {
        // Disabilita immediatamente il bottone come prima azione
        button.interactable = false;
        
        // Poi procedi con il resto delle operazioni
        actionPhaseEndEvent?.Invoke(new VoidEvent(0));
        
        if (buttonGameObject != null)
            buttonGameObject.transform.DOLocalRotate(new Vector3(0, 180, 0), 0.5f);

        PlayPhaseButton();

        // Imposta il testo per il turno nemico
        UpdatePhaseText(enemyTurnText);
        
        // Aggiorna il colore di sfondo per il turno nemico
        UpdateCameraBackground(enemyTurnBackground);
        
        // Disabilita tutte le carte quando finisce la fase di azione
        if (handManager != null)
            handManager.SetAllCardsSelectable(false);
        
        StopBlinking();
    }
    
    // Metodo per aggiornare il testo della fase con animazione
    private void UpdatePhaseText(string text)
    {
        if (phaseText == null) return;
        
        // Animazione di fade out/in per un cambio fluido
        DOTween.Sequence()
            .Append(phaseText.DOFade(0f, 0.2f)) // Fade out
            .AppendCallback(() => {
                phaseText.text = text;
                //phaseText.color = new Color(color.r, color.g, color.b, 0f);
            })
            .Append(phaseText.DOFade(1f, 0.2f)); // Fade in
    }

    // Metodo per cambiare il colore di sfondo della camera con una transizione fluida
    private void UpdateCameraBackground(Color targetColor)
    {
        if (mainCamera == null) return;
        
        // Ottieni il colore corrente
        Color currentColor = mainCamera.backgroundColor;
        
        // Animazione di transizione del colore
        DOTween.To(
            () => currentColor,
            color => mainCamera.backgroundColor = color,
            targetColor,
            backgroundTransitionDuration
        );
    }

    

    private FMOD.Studio.EventInstance bigButton;
    public void PlayPhaseButton()
    {
        bigButton = FMODUnity.RuntimeManager.CreateInstance("event:/UI/BigButton");
        bigButton.start();
        bigButton.release();
    }

    #endregion

  
}