using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Assicurati di avere questa direttiva using per DOTween

public class StatsPanelScript : MonoBehaviour
{
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_FontAsset customStatsFont;

    [Header("Generic Stat Prefab")]
    [SerializeField] private Transform statsParent;
    [SerializeField] private GameObject genericStatPrefab;
    [SerializeField] private GameObject taccaPrefab;


    [Header("Parameters to display")]

    //classe
    [SerializeField] private TMP_Text shipClassName = null;
    [SerializeField] private Image shipClassImageSprite = null;
    [SerializeField] private TMP_Text _objectDescription = null;
    //vita
    [SerializeField] private TMP_Text shipHealth = null;

    //statistiche

    //effetti


    [Header("Display parameters")]
    [SerializeField] private int fontSize = 3;
    [SerializeField] private float waitTimeBeforeShow = 0.5f;

    //Classi da usare per convertire gli script passati dagli eventi
    AShip shipScript = null;
    AbstractObstacle obstacleScript = null;
    Tile tileScript = null;

    private HorizontalLayoutGroup horizontalLayoutGroup;

    private bool isDisplaying = false;

    // Per tenere traccia del modello correntemente visualizzato
    private MeshRenderer _currentDisplayedModel;
    [SerializeField] private Transform _pivotObjectModel;
    [SerializeField] private Material _wireframeMaterial;

    // Aggiungi queste variabili per tenere traccia dell'animazione di rotazione
    private Tweener _rotationTween;
    private GameObject _currentModelContainer;

    #region Iscrizione agli eventi
    private void OnEnable()
    {
        DisplayStats.OnEntityHoverStarted += ShowStatsPanel;
        DisplayStats.OnEntityHoverEnded += HideStatsPanel;
    }

    private void OnDisable()
    {
        DisplayStats.OnEntityHoverStarted -= ShowStatsPanel;
        DisplayStats.OnEntityHoverEnded -= HideStatsPanel;
    }

    #endregion

    #region  Metodi di display
    private void ShowStatsPanel(DisplayStatsClass displayStats)
    {
        if (!isDisplaying)
        {
            // Qui l'ordine degli if è importante!!

            if (displayStats.script is AShip)
            {
                Debug.Log("Ho preso una nave");
                shipScript = (AShip)displayStats.script;

                // Passa l'oggetto ship direttamente invece che lo ShipSO
                SetShipClass(shipScript);

                // Visualizza il modello della nave se disponibile
                if (shipScript.shipSO.shipModelMesh != null)
                {
                    // Valori predefiniti per il posizionamento e la scala
                    Vector3 repositionOffset = shipScript.shipSO.uiRepositionOffset;
                    Vector3 scaleFactor = shipScript.shipSO.uiScaleFactor;

                    // Aggiungi la rotazione di 90 gradi sull'asse X
                    Quaternion shipRotation = Quaternion.Euler(-90f, 0f, 0f);

                    // Chiamata al metodo con la rotazione specificata
                    SpawnModelInUI(shipScript.shipSO.shipModelMesh, repositionOffset, scaleFactor, shipRotation);
                }
            }
            else if (displayStats.script is AbstractObstacle)
            {
                Debug.Log("Qui ho preso un ostacolo");
                obstacleScript = (AbstractObstacle)displayStats.script;
                shipClassName.text = obstacleScript.GetObstacleName();
                _objectDescription.text = obstacleScript.GetObstacleDescription();

                // Ottieni il modello wireframe dall'ostacolo
                MeshRenderer wireframeModel = obstacleScript.GetWireframeModel();

                // Se il modello esiste, visualizzalo
                if (wireframeModel != null)
                {
                    SpawnModelInUI(
                        wireframeModel.gameObject,
                        obstacleScript.GetWireframeRepositionOffset(),
                        obstacleScript.GetWireframeScaleFactor(),
                        Quaternion.identity // Nessuna rotazione aggiuntiva
                    );
                }
            }
            else if (displayStats.script is Tile)
            {
                Debug.Log("Qui ho preso una tile");
                tileScript = (Tile)displayStats.script;
                _objectDescription.text = tileScript.GetTileDescription();
                shipClassName.text = "Fog";
                MeshRenderer wireframeModel = tileScript.GetWireframeModel();
                if (wireframeModel != null)
                {
                    Quaternion fogRotation = Quaternion.Euler(-90f, 0f, 0f);
                    // Usa il metodo SpawnModelInUI per visualizzare il modello wireframe della tile
                    SpawnModelInUI(
                        wireframeModel.gameObject,
                        tileScript.GetWireframeRepositionOffset(),
                        tileScript.GetWireframeScaleFactor(),
                        fogRotation
                    );
                }
            }


            isDisplaying = true;
        }

    }
    private void HideStatsPanel(DisplayStatsClass displayStats)
    {
        CleanupPreviousModel();
        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject);
        }
        _objectDescription.text = "";
        shipClassName.text = "searching...";
        isDisplaying = false;
    }

    private void SetShipClass(ShipSO shipClass)
    {
        foreach (string statName in shipClass.statNames)
        {
            GameObject statObject = Instantiate(genericStatPrefab, statsParent, false);
            statObject.name = statName;
            statObject.GetComponentInChildren<TMP_Text>().text = statName;
            statObject.GetComponentInChildren<TMP_Text>().fontSize = fontSize;
            statObject.GetComponentInChildren<TMP_Text>().font = customStatsFont;

            horizontalLayoutGroup = statObject.GetComponentInChildren<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.padding.left = 10;
            horizontalLayoutGroup.padding.right = 10;

            // Cerca l'oggetto "Tacche" come figlio del Transform di statObject
            Transform taccheContainer = statObject.transform.Find("Tacche");
            if (taccheContainer == null)
            {
                // Se il contenitore non esiste, crealo
                GameObject taccheObj = new GameObject("Tacche");
                taccheObj.transform.SetParent(statObject.transform, false);
                taccheContainer = taccheObj.transform;
            }

            // Ora istanzia le tacche come figlie di taccheContainer
            for (int i = 0; i < shipClass.statsDictionary[statName]; i++)
            {
                GameObject tacca = Instantiate(taccaPrefab, taccheContainer);
                tacca.transform.localPosition = new Vector3(i * 20, 0, 0);
                tacca.name = "Tacca" + i;
            }
        }
    }

    private void SetShipClass(AShip ship)
    {
        // Usa il nome della nave direttamente dalla nave
        shipClassName.text = ship.shipSO.className;

        // Pulisci eventuali statistiche precedenti
        foreach (Transform child in statsParent)
        {
            Destroy(child.gameObject);
        }

        // Crea le quattro statistiche specifiche richieste
        CreateStatObject("Health", ship.GetHealth());
        CreateStatObject("AttackPower", ship.attackPower);
        CreateStatObject("AttackRange", ship.attackRange);
        CreateStatObject("MovementRange", ship.movementRange);
    }

    // Nuovo metodo helper per creare gli oggetti statistica
    private void CreateStatObject(string statName, int value)
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

        horizontalLayoutGroup = statObject.GetComponentInChildren<HorizontalLayoutGroup>();
        if (horizontalLayoutGroup != null)
        {
            horizontalLayoutGroup.childScaleHeight = true;
            horizontalLayoutGroup.childScaleWidth = true;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.padding.left = 10;
            horizontalLayoutGroup.padding.right = 10;
        }

        // Cerca l'oggetto "Tacche" come figlio del Transform di statObject
        Transform taccheContainer = statObject.transform.Find("Tacche");
        if (taccheContainer == null)
        {
            // Se il contenitore non esiste, crealo
            GameObject taccheObj = new GameObject("Tacche");
            taccheObj.transform.SetParent(statObject.transform, false);
            taccheContainer = taccheObj.transform;
        }

        // Ora istanzia le tacche come figlie di taccheContainer
        for (int i = 0; i < value; i++)
        {
            GameObject tacca = Instantiate(taccaPrefab, taccheContainer);
            tacca.transform.localPosition = new Vector3(i * 20, 0, 0);
            tacca.name = "Tacca" + i;
        }
    }
    #endregion

    // Modifica anche la funzione di pulizia per fermare l'animazione
    public void HidePanel()
    {
        // Interrompi l'animazione di rotazione
        if (_rotationTween != null)
        {
            _rotationTween.Kill();
            _rotationTween = null;
        }

        // Distruggi il contenitore se esiste
        if (_currentModelContainer != null)
        {
            Destroy(_currentModelContainer);
            _currentModelContainer = null;
            _currentDisplayedModel = null;
        }

        // Resto del codice esistente...
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

    // Nuovo metodo per applicare il materiale wireframe a tutti i renderer nell'oggetto e suoi figli
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

        // Applica il materiale anche a tutti gli SkinnedMeshRenderer (spesso usati nei modelli di personaggi/navi)
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
        // Imposta il layer dell'oggetto corrente
        obj.layer = newLayer;

        // Imposta il layer su tutti i figli in modo ricorsivo
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
