using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DisplayHealth : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject healthBarCanvasPrefab;

    [SerializeField] private RectTransform healthBarBackground;
    [SerializeField] private RectTransform healthPanel;
    [SerializeField] private float segmentWidth = 20f; // Larghezza di ogni segmento della barra della salute
    [SerializeField] private float backgroundPadding = 10f; // Padding per lo sfondo della barra della salute

    private AShip shipScript;
    private Vector2Int position;

    private int healthBarCount = 0;
    private List<GameObject> healthObjectsList = new List<GameObject>();
    private GameObject healthBarInstance;

    private void Start()
    {
        shipScript = GetComponent<AShip>();
        // Inizializza la barra della salute all'avvio dello script
        InitializeHealthBar();
    }

    public void InitializeHealthBar()
    {
        for (int i = 0; i < healthObjectsList.Count; i++)
        {
            Destroy(healthObjectsList[i]);
        }
        healthObjectsList.Clear();
        if (shipScript != null && healthBarPrefab != null)
        {
            int maxHealth = shipScript.GetMaxHealth(); // Calcola la larghezza totale della barra della salute

            healthBarCount = shipScript.GetHealth();
            for (int i = 0; i < healthBarCount; i++)
            {
                healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, healthBarCanvasPrefab.transform);
                healthObjectsList.Add(healthBarInstance);
            }

            // Calcola la larghezza totale della barra della salute
            UpdateBackgroundSize(maxHealth);

        }
    }

    public void AddHealth(int amount)
    {
        healthBarCount += amount;
        for (int i = 0; i < amount; i++)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, healthBarCanvasPrefab.transform);
            healthObjectsList.Add(healthBarInstance);
        }
    }


    //funzione da chiamare quando la nave subisce danni, in concomitanza con l'evento
    public void UpdateHealthBar(int damage)
    {

        int endIndex = healthBarCount - damage - 1;
        if (endIndex < 0) endIndex = 0;

        for (int i = 0; i < damage; i++)
        {

            int index = healthBarCount - i - 1;
            if (index < 0)
            {
                index = 0;
            }
            if (healthObjectsList.Count > 0)
            {
                GameObject healthBar = healthObjectsList[index];
                healthObjectsList.RemoveAt(index);
                Destroy(healthBar);
            }
            else
            {
                break;
            }

        }
        healthBarCount -= damage;

    }

    private void UpdateBackgroundSize(int maxHealth)
    {
        if (healthBarBackground != null)
        {
            float totalWidth = (maxHealth * segmentWidth) + backgroundPadding * 2f;
            Vector2 size = healthBarBackground.sizeDelta;
            size.x = totalWidth;
            healthBarBackground.sizeDelta = size;
        }
    }

}
