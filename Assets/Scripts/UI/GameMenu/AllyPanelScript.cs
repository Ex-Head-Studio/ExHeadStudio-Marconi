using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AllyPanelScript : MonoBehaviour
{
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_FontAsset customStatsFont;

    [Header("Generic Stat Prefab")]
    [SerializeField] private Transform statsParent;
    [SerializeField] private GameObject genericStatPrefab;
    [SerializeField] private GameObject taccaPrefab;

    [Header("Ship Reference")]
    [SerializeField] private AllyShip targetShip;

    [Header("Parameters to display")]
    [SerializeField] private TMP_Text shipClassName = null;
    [SerializeField] private Image shipClassImageSprite = null;
    [SerializeField] private TMP_Text _objectDescription = null;

    [Header("Display parameters")]
    [SerializeField] private int fontSize = 3;

    // Per il modello 3D
    private MeshRenderer _currentDisplayedModel;
    [SerializeField] private Transform _pivotObjectModel;
    [SerializeField] private Material _wireframeMaterial;
    private Tweener _rotationTween;
    private GameObject _currentModelContainer;

    // Riferimenti alle statistiche
    private GameObject healthStat;
    private GameObject attackPowerStat;
    private GameObject attackRangeStat;
    private GameObject movementRangeStat;

    private HorizontalLayoutGroup horizontalLayoutGroup;

    /* void Start()
    {
        // Trova automaticamente una nave alleata nella scena
        targetShip = FindObjectOfType<AllyShip>();
        
        // Se abbiamo trovato una nave, mostra le sue statistiche
        if (targetShip != null)
        {
            Debug.Log("Nave alleata trovata: " + targetShip.name);
            ShowShipStats(targetShip);
        }
        else
        {
            Debug.LogWarning("Nessuna nave alleata trovata nella scena!");
        }
    } */

    // Metodo pubblico per impostare una nuova nave da visualizzare
    public void SetShip(AllyShip ship)
    {
        targetShip = ship;

        // Pulisci eventuali statistiche precedenti
        ClearStats();

        if (targetShip != null)
        {
            ShowShipStats(targetShip);
        }
    }

    // Pulisce tutte le statistiche visualizzate
    private void ClearStats()
    {
        CleanupPreviousModel();

        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject);
        }

        _objectDescription.text = "";
        shipClassName.text = "No ship selected";

        // Reset dei riferimenti alle statistiche
        healthStat = null;
        attackPowerStat = null;
        attackRangeStat = null;
        movementRangeStat = null;
    }

    // Mostra le statistiche della nave specificata
    private void ShowShipStats(AllyShip ship)
    {
        if (ship == null)
            return;

        // Imposta le statistiche della nave direttamente dall'oggetto AllyShip
        SetShipClass(ship);

        // Se serve la descrizione, puoi comunque prenderla da shipSO
        if (ship.shipSO != null)
        {

            // Visualizza il modello 3D della nave se disponibile
            if (ship.shipSO.shipModelMesh != null)
            {
                Vector3 repositionOffset = ship.shipSO.uiRepositionOffset;
                Vector3 scaleFactor = ship.shipSO.uiScaleFactor;
                Quaternion shipRotation = Quaternion.Euler(-90f, 0f, 0f);

                SpawnModelInUI(ship.shipSO.shipModelMesh, repositionOffset, scaleFactor, shipRotation);
            }
        }

        // Mostra il pannello se non è già attivo
        if (!statsPanel.activeSelf)
        {
            statsPanel.SetActive(true);
        }
    }

    // Imposta le informazioni di classe della nave
    private void SetShipClass(AllyShip ship)
    {
        if (ship == null)
            return;

        shipClassName.text = ship.shipSO.className;

        // Crea le quattro statistiche specifiche richieste usando i valori dalla nave
        healthStat = CreateStatObject("Health", ship.GetHealth());
        attackPowerStat = CreateStatObject("AttackPower", ship.attackPower);
        attackRangeStat = CreateStatObject("AttackRange", ship.attackRange);
        movementRangeStat = CreateStatObject("MovementRange", ship.movementRange);
    }

    // Metodo helper per creare un oggetto statistica con tacche
    private GameObject CreateStatObject(string statName, int value)
    {
        GameObject statObject = Instantiate(genericStatPrefab, statsParent, false);
        statObject.name = statName;

        // Imposta il testo della statistica
        TMP_Text statText = statObject.GetComponentInChildren<TMP_Text>();
        if (statText != null)
        {
            statText.text = FormatStatName(statName);
            statText.fontSize = fontSize;
            statText.font = customStatsFont;
        }

        // Configura il layout
        horizontalLayoutGroup = statObject.GetComponentInChildren<HorizontalLayoutGroup>();
        if (horizontalLayoutGroup != null)
        {
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.padding.left = 10;
            horizontalLayoutGroup.padding.right = 10;
        }

        // Cerca o crea il contenitore delle tacche
        Transform taccheContainer = statObject.transform.Find("Tacche");
        if (taccheContainer == null)
        {
            GameObject taccheObj = new GameObject("Tacche");
            taccheObj.transform.SetParent(statObject.transform, false);
            taccheContainer = taccheObj.transform;
        }

        // Crea le tacche
        for (int i = 0; i < value; i++)
        {
            GameObject tacca = Instantiate(taccaPrefab, taccheContainer);
            tacca.transform.localPosition = new Vector3(i * 20, 0, 0);
            tacca.name = "Tacca" + i;
        }

        return statObject;
    }

    // Aggiorna le tacche per una statistica specifica
    private void UpdateStatTacche(GameObject statObject, int value)
    {
        if (statObject == null) return;

        Transform taccheContainer = statObject.transform.Find("Tacche");
        if (taccheContainer == null) return;

        // Rimuovi tutte le tacche esistenti
        foreach (Transform child in taccheContainer)
        {
            Destroy(child.gameObject);
        }

        // Crea nuove tacche in base al valore aggiornato
        for (int i = 0; i < value; i++)
        {
            GameObject tacca = Instantiate(taccaPrefab, taccheContainer);
            tacca.transform.localPosition = new Vector3(i * 20, 0, 0);
            tacca.name = "Tacca" + i;
        }
    }

    // Metodo per aggiornare i valori delle statistiche
    public void UpdateStats()
    {
        if (targetShip == null)
            return;

        // Aggiorna le tacche per ogni statistica in base ai valori attuali della nave
        UpdateStatTacche(healthStat, targetShip.GetHealth());
        UpdateStatTacche(attackPowerStat, targetShip.attackPower);
        UpdateStatTacche(attackRangeStat, targetShip.attackRange);
        UpdateStatTacche(movementRangeStat, targetShip.movementRange);
    }

    // Metodo per lo spawn dei modelli wireframe nell'UI
    private void SpawnModelInUI(GameObject modelObject, Vector3 repositionOffset, Vector3 scaleFactor, Quaternion initialRotation)
    {
        // Pulizia dei modelli precedenti
        CleanupPreviousModel();

        if (modelObject == null || _pivotObjectModel == null || _wireframeMaterial == null)
        {
            Debug.LogWarning("Impossibile creare il modello wireframe: riferimenti mancanti");
            return;
        }

        // Ottieni il RectTransform del pivot
        RectTransform pivotRect = _pivotObjectModel.GetComponent<RectTransform>();
        if (pivotRect == null)
        {
            Debug.LogError("_pivotObjectModel non ha un componente RectTransform!");
            return;
        }

        // Crea il contenitore per il modello 3D
        GameObject container = new GameObject("WireframeContainer");
        container.transform.SetParent(_pivotObjectModel);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        // Aggiungi il RectTransform al container
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(repositionOffset.x, repositionOffset.y);

        // Aggiungi l'offset Z
        Vector3 localPos = container.transform.localPosition;
        container.transform.localPosition = new Vector3(localPos.x, localPos.y, localPos.z + repositionOffset.z);

        // Istanzia il modello
        GameObject modelInstance = Instantiate(modelObject, container.transform);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = initialRotation;
        modelInstance.transform.localScale = scaleFactor;

        // Imposta il layer
        SetLayerRecursively(container, LayerMask.NameToLayer("UIStats"));

        // Applica il materiale wireframe a TUTTI i renderer nell'oggetto e nei suoi figli
        ApplyWireframeMaterial(modelInstance);

        // Memorizza il riferimento al container
        _currentModelContainer = container;

        // Per mantenere il riferimento al renderer principale
        _currentDisplayedModel = modelInstance.GetComponent<MeshRenderer>();

        // Aggiungi la rotazione continua
        _rotationTween = modelInstance.transform.DORotate(new Vector3(0, 360, 0), 8f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetRelative(true);
    }

    // Metodo per applicare il materiale wireframe a tutti i renderer nell'oggetto e suoi figli
    private void ApplyWireframeMaterial(GameObject obj)
    {
        // Applica il materiale a tutti i MeshRenderer
        MeshRenderer[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = _wireframeMaterial;
            }
            renderer.materials = newMaterials;
        }

        // Applica il materiale anche a tutti gli SkinnedMeshRenderer
        SkinnedMeshRenderer[] skinnedRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = _wireframeMaterial;
            }
            renderer.materials = newMaterials;
        }
    }

    // Metodo per pulire i modelli precedenti
    private void CleanupPreviousModel()
    {
        if (_rotationTween != null)
        {
            _rotationTween.Kill();
            _rotationTween = null;
        }

        if (_currentDisplayedModel != null)
        {
            Destroy(_currentDisplayedModel.gameObject);
            _currentDisplayedModel = null;
        }

        if (_currentModelContainer != null)
        {
            Destroy(_currentModelContainer);
            _currentModelContainer = null;
        }
    }

    // Metodo per impostare il layer in modo ricorsivo su un GameObject e tutti i suoi figli
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    
    // Metodo per formattare i nomi delle statistiche
    private string FormatStatName(string statName)
    {
        // Inserisce uno spazio prima di ogni lettera maiuscola (tranne la prima)
        string formattedName = "";
        for (int i = 0; i < statName.Length; i++)
        {
            if (i > 0 && char.IsUpper(statName[i]))
            {
                formattedName += " " + statName[i];
            }
            else
            {
                formattedName += statName[i];
            }
        }
        return formattedName;
    }
}
