using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using DG.Tweening;


public class MainMenuButtons : MonoBehaviour
{
    /// <summary>
    /// Lo script si occupa di gestire i pulsanti del menu principale, viene associato al canvas parent
    /// </summary>

    [Header("Main Menu Panels")]
 
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject classSelectionPanel;
    [SerializeField] private GameObject title;

    [Header("Animazione Transizione")]
    [SerializeField] private GameObject cardineScatola1;
    [SerializeField] private GameObject cardineScatola2;
    [SerializeField] private GameObject cameraIniziale;
    [SerializeField] private GameObject cameraTransizione;
    [SerializeField] private float durataTotaleAnimazione = 2f;
    [SerializeField] private float ritardoCambioCamera = 0.5f;
    [SerializeField] private float ritardoDopoAnimazione = 1f; // Ritardo prima di caricare la scena
    [SerializeField] private Ease tipoEasing = Ease.InOutBack;

    [field: Header("FMOD Events")]
    [field: SerializeField] public EventReference startButtonSound { get; private set; }
    [field: SerializeField] public EventReference menuButtonSound { get; private set; }

    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
    public void StartGame()
    {
        this.GetComponent<Canvas>().enabled = false; // Disabilita il canvas per evitare interazioni durante l'animazione
        // Riproduci il suono del pulsante Start
        AudioManager.PlayOneShot(startButtonSound, this.transform.position);
        
        // Disabilita l'interazione con altri pulsanti durante l'animazione
        GetComponent<CanvasGroup>().interactable = false;
        
        // Avvia la sequenza di animazione
        Sequence animazioneTransizione = DOTween.Sequence();
        
        // Usa DOLocalRotate per rotazioni locali del primo cardine
        animazioneTransizione.Join(
            cardineScatola1.transform.DOLocalRotate(new Vector3(0, 360, 0), durataTotaleAnimazione)
            .SetEase(tipoEasing)
        );
        
        // Usa DOLocalRotate per rotazioni locali del secondo cardine
        animazioneTransizione.Join(
            cardineScatola2.transform.DOLocalRotate(new Vector3(0, 0, 0), durataTotaleAnimazione)
            .SetEase(tipoEasing)
        );
        
        // Dopo un breve ritardo, cambia la telecamera
        animazioneTransizione.InsertCallback(ritardoCambioCamera, () => {
            // Spegni la camera iniziale
            cameraIniziale.SetActive(false);
            // Accendi la camera di transizione
            cameraTransizione.SetActive(true);
        });
        
        // Aggiungi un ritardo dopo l'animazione
        animazioneTransizione.AppendInterval(ritardoDopoAnimazione);
        
        // Al termine dell'animazione e del ritardo, carica la scena successiva
        animazioneTransizione.OnComplete(() => {
            SceneManager.LoadScene(1);
        });
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);
        title.SetActive(false);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);
    }

    public void OpensOptions()
    {
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        title.SetActive(false);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);

    }

    public void BackToMainMenu()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        classSelectionPanel.SetActive(false);
        title.SetActive(true);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);
    }

    public void OpenClassSelection()
    {
        mainMenuPanel.SetActive(false);
        title.SetActive(false);
        classSelectionPanel.SetActive(true);
        AudioManager.PlayOneShot(menuButtonSound, this.transform.position);
    }
}
