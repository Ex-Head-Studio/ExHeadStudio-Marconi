using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialCarousel : MonoBehaviour
{
    [SerializeField] private Image imageDisplay;    // The UI Image component to display the tutorial images
    [SerializeField] private Sprite[] tutorialImages; // Array of tutorial images to display
    [SerializeField] private Button nextButton; // Button to go to the next image
    [SerializeField] private Button previousButton; // Button to go to the previous image
    [SerializeField] private TextMeshProUGUI imageIndexText; // Text to display the current image index
    [SerializeField] private Slider progressBar; // Slider to indicate progress through the images
    [SerializeField] private RectTransform submarineIndicator; // immagine sottomarino
    [SerializeField] private RectTransform progressBarFillArea; // il rect che contiene la barra

    private int currentIndex = 0; // Current index of the displayed image

    void Start()
    {
        //Imposta primo sprite e aggiungi listener ai bottoni
        UpdateImage();

        nextButton.onClick.AddListener(NextImage);
        previousButton.onClick.AddListener(PreviousImage);
    }

    void UpdateImage()
    {
        imageDisplay.sprite = tutorialImages[currentIndex];

        // Aggiorna il testo dell'indice dell'immagine
        imageIndexText.text = $"{currentIndex + 1}/{tutorialImages.Length}";

        // Aggiorna la barra di progresso
        if (progressBar != null)
        {
            progressBar.maxValue = tutorialImages.Length - 1;
            progressBar.value = currentIndex;

            // Aggiorna la posizione dell'indicatore del sottomarino
            if (submarineIndicator != null && progressBarFillArea != null)
            {
                float t = currentIndex / (float)(tutorialImages.Length - 1);
                float barWidth = progressBarFillArea.rect.width;
                Vector2 pos = submarineIndicator.anchoredPosition;
                pos.x = t * barWidth;
                submarineIndicator.anchoredPosition = pos;
            }
        }

        // Abilita o disabilita i bottoni in base all'indice corrente
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < tutorialImages.Length - 1;
    }

    void PreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateImage();
        }
    }

    void NextImage()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateImage();
        }
    }
}
