using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random=UnityEngine.Random;
public class ShipManager2 : MonoBehaviour, IShipManager
{

    [Header("Parameters")]
    [SerializeField] private ShipManagerSO shipManagerSO;
    [SerializeField] private float timeBeforeGeneration = 1f;
    [SerializeField] private float timeBeforeEndEnemyTurn;
    private Dictionary<string, AShip> shipsD;

    //per vedere le liste in inspector, usare la modalità di debug

    public InfluenceMap influenceMap;


    private List<AShip> enemies = new List<AShip>();
    private List<AShip> allies = new List<AShip>();

    private static int allyCount = 0;
    private static int enemyCount = 0;

    private int modelIndex;

    //Attacca i componenti di ship manager a ship manager
    void Awake()
    {


        shipsD = new Dictionary<string, AShip>();
        shipManagerSO.RandomizeShips();

        influenceMap = new InfluenceMap(GridManager.Instance._width, GridManager.Instance._height, shipManagerSO.influenceDecay, shipManagerSO.decayMomentum);
    }


    //Funzione che viene chiamata all'inizio del gioco per generare le navi
    public void GenerateShips()
    {
        StartCoroutine(ShipGeneration());
    }

    private IEnumerator ShipGeneration()
    {
        yield return new WaitForSeconds(timeBeforeGeneration);
        foreach (string ship in shipManagerSO.startingShips)
        {
            InstantiateInMap(ship);
        }
        influenceMap.Propagate();
    }

    public void InstantiateInMap(string shipName)
    {
        if (shipManagerSO.allyShips + shipManagerSO.enemyShips > shipManagerSO.startingShips.Count)
        {
            Debug.LogError("Not enough shipsD for the number of allies and enemies");
            return;
        }

        if (allyCount < shipManagerSO.allyShips)
        {
            InstantiateAllyShip(shipName, (int)Entity.ally);
            return;
        }
        if (enemyCount < shipManagerSO.enemyShips)
        {
            InstantiateEnemyShip(shipName, (int)Entity.enemy);
            return;
        }
    }

    /// <summary>
    ///  Instantiates an ally ship and adds it to the dictionary of shipsD.
    /// </summary>
    /// <param name="shipName"></param>
    /// 


    //Possiamo invocare questo metodo da parte di una carta supporto
    public void InstantiateAllyShip(string shipName, int faction)
    {
        AShip newShip = SetupShip(shipName, faction);
        shipsD.Add(newShip.shipName, newShip);
        allies.Add(newShip);
        allyCount++;
        GridManager.Instance.InsertShips(newShip);
        influenceMap.RegisterPropagator(newShip);
    }

    /// <summary>
    ///  Instantiates an anemy ship and adds it to the dictionary of shipsD.
    /// <param name="shipName"></param>
    /// </summary>

    public void InstantiateEnemyShip(string shipName, int faction)
    {
        AShip newShip = SetupShip(shipName, faction);
        shipsD.Add(newShip.shipName, newShip);
        enemies.Add(newShip);
        enemyCount++;
        GridManager.Instance.InsertShips(newShip);
        influenceMap.RegisterPropagator(newShip);
    }



    //La funzione viene chiamata per dire alle navi di calcolare le loro azioni, fatto questo, le navi poi potranno eseguire la loro azione migliore
    //una volta che il turno verrà effettuato.
    public void EnemyMovesSelection()
    {
        //Debug.Log("Ricerca mosse del nemico");


    }
    //Il metodo viene chiamato dall'evento di fine turno giocatore e fa eseguire alle navi la loro mossa preferita
    public void EnemyMovesExecution(){
       
        Debug.Log("Esecuzione turno nemico");
        for(int j=0 ;j<shipManagerSO.initialEnemyShips; j++){
            enemies[j].LookForMoves();
        }
        StartCoroutine(EndEnemyTurn());
    }

    public IEnumerator EndEnemyTurn()
    {
        yield return new WaitForSeconds(timeBeforeEndEnemyTurn);
        shipManagerSO.onEndEnemyTurn.Invoke(new VoidEvent(0));
        foreach (EnemyShip enemy in enemies)
        {
            //enemy.ResetAction();
        }
        Debug.Log("Fine turno nemico");
    }

    //Questa serve ancora?
    //Finito il turno, aggiorna la influence map con le nuove posizioni delle navi, così i calcoli nei turni successivi sono corretti
    public void EndTurn()
    {
        influenceMap.Propagate();

    }


    /// <summary>
    /// Callback functiont which removes the ship from the map, the dictionary of shipsD and the infuence map.
    /// It also destroys the ship game object.
    /// <param name="shipDestroyedStruct"> The Struct passed by the event channel</param> 
    /// </summary>
    public void RemoveShip(ShipDestroyedStruct shipDestroyedStruct)
    {
        if (shipsD.ContainsKey(shipDestroyedStruct.shipName))
        {
            if (shipDestroyedStruct.entity == (int)Entity.ally)
            {
                allies.Remove(shipsD[shipDestroyedStruct.shipName]);
                allyCount--;
            }
            else if (shipDestroyedStruct.entity == (int)Entity.enemy && shipsD.ContainsKey(shipDestroyedStruct.shipName))
            {
                enemies.Remove(shipsD[shipDestroyedStruct.shipName]);
                enemyCount--;
            }


            influenceMap.UnregisterPropagator(shipDestroyedStruct.shipScript);
            influenceMap.Propagate();
            GridManager.Instance.RemoveShip(shipDestroyedStruct.shipScript);

            shipsD.Remove(shipDestroyedStruct.shipName);
            Destroy(shipDestroyedStruct.shipScript.gameObject);
        }
    }


    private AShip SetupShip(string shipName, int faction)
    {
        // Selezione del modello
        if (faction == (int)Entity.ally && shipManagerSO.allyShips == 1)
        {
            modelIndex = shipManagerSO.shipSOarray.Count - 1;
        }
        else
        {
            modelIndex = Random.Range(0, shipManagerSO.shipSOarray.Count);
        }

        // Istanzia il prefab principale
        GameObject newShip = Instantiate(shipManagerSO.shipSOarray[modelIndex].shipModelPrefab, transform.position, Quaternion.Euler(90, 0, 0));

        // Trova Ship nella gerarchia
        Transform shipTransform = null;
        foreach (Transform child in newShip.transform)
        {
            if (child.name.ToLower().Contains("ship"))
            {
                shipTransform = child;
                break;
            }
        }

        // Variabile che terrà il riferimento al GameObject del modello della classe
        GameObject shipClassModelInstance = null;

        // Istanzia SEMPRE il modello nell'oggetto Ship, se lo troviamo
        if (shipTransform != null)
        {
            shipClassModelInstance = Instantiate(shipManagerSO.shipSOarray[modelIndex].shipClassModel, shipTransform, false);
            Debug.Log($"Modello istanziato in: {shipTransform.name}");
        }
        else
        {
            // Fallback: istanzia direttamente sulla nave
            shipClassModelInstance = Instantiate(shipManagerSO.shipSOarray[modelIndex].shipClassModel, newShip.transform, false);
            Debug.LogWarning("Oggetto Ship non trovato, modello istanziato direttamente sulla nave");
        }

        // Tolgo il component NewShip
        if (newShip.TryGetComponent<NewShip>(out NewShip oldScript))
        {
            Destroy(oldScript);
        }

        // ORA che tutto è istanziato, cerca l'animator
        Animator shipAnimator = null;

        // Cercalo direttamente nel GameObject istanziato dentro Ship
        if (shipTransform != null)
        {
            // Prima cerca direttamente nel figlio immediato di Ship (il primo GameObject istanziato dentro)
            if (shipTransform.childCount > 0)
            {
                // Cerca l'animator nel primo figlio di Ship
                Transform firstChild = shipTransform.GetChild(0);
                shipAnimator = firstChild.GetComponent<Animator>();

                Debug.Log("ShipAnimator: " +shipAnimator.name);
                
                if (shipAnimator != null)
                {
                    Debug.Log($"Trovato animator nel primo figlio di Ship: {firstChild.name}");
                }
                else
                {
                    // Se non l'abbiamo trovato sul primo figlio diretto, non cercare ricorsivamente
                    Debug.LogWarning($"Nessun animator trovato nel primo figlio di Ship: {firstChild.name}");
                }
            }
            else
            {
                Debug.LogWarning("Ship non ha figli, impossibile trovare animator");
            }
        }

        // Se non abbiamo trovato l'animator con il metodo specifico, NON usare fallback
        if (shipAnimator == null)
        {
            Debug.LogError("Animator non trovato nel figlio diretto di Ship");
        }
        else
        {
            // Assegna il controller all'animator
            //shipAnimator.runtimeAnimatorController = shipManagerSO.shipSOarray[modelIndex].shipAnimatorController;
            //Debug.Log($"Controller assegnato all'animator: {shipAnimator.gameObject.name}");
        }

        // Ora crea il componente nave appropriato
        if (faction == (int)Entity.ally)
        {
            newShip.AddComponent<AllyShip>();
            newShip.AddComponent<CardAllyShip>();

            AllyShip shipScript = newShip.GetComponent<AllyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);

            // Assegna l'animator trovato
            if (shipAnimator != null)
            {
                shipScript.SetShipAnimator(shipAnimator);
                Debug.Log($"Animator assegnato alla nave alleata {shipName}: {shipAnimator.gameObject.name}");
            }
            else
            {
                Debug.LogError($"Nessun animator da assegnare alla nave alleata {shipName}");
            }

            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.allyMaterial);
            return shipScript;
        }
        else
        {
            // Codice per le navi nemiche (stesso pattern)
            newShip.AddComponent<EnemyShip>();
            newShip.AddComponent<CardEnemyShip>();

            EnemyShip shipScript = newShip.GetComponent<EnemyShip>();
            shipScript.SetupShip(shipManagerSO.shipSOarray[modelIndex], shipName, faction, this);

            // Assegna l'animator trovato
            if (shipAnimator != null)
            {
                shipScript.SetShipAnimator(shipAnimator);
                Debug.Log($"Animator assegnato alla nave nemica {shipName}: {shipAnimator.gameObject.name}");
            }

            newShip.GetComponentInChildren<ShipModelMaterialAssignement>().AssignMaterialToMeshRenderers(shipManagerSO.enemyMaterial);
            return shipScript;
        }
    }


    public int NumberOfMessages
    {
        get { return shipManagerSO.numberOfMessages; }
    }

    public InfluenceMap InfluenceMap
    {
        get { return influenceMap; }
    }
}