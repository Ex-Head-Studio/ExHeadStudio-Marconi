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
