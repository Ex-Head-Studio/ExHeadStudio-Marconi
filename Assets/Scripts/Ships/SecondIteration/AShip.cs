using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Cinemachine;
using DG.Tweening;

public abstract class AShip : MonoBehaviour
{
    protected enum StrategyState
    {
        InRange,
        Patrolling
    }
    [Header("Scriptable Objects")]
    [SerializeField] public ShipSO shipSO;

    [Header("Ship Parameters")]
    [SerializeField] protected float nearbyShipSearchRadius;
    [SerializeField] public Vector2Int position;
    public float shipInfluence;

    [Header("Moves visualization")]
    [SerializeField] protected bool canVisualizeMoves = true;

    protected int mapWidth;
    protected int mapHeight;
    public LayerMask adversaryShipLayer;
    public enum ShipState
    {
        Attacking,
        Moving,
        Waiting
    }

    public string shipName;
    public IShipManager manager;


    // questa va inserita nella logica delle navi
    protected int health;


    //Servono per alterare il testo nel display dei comandi, vedi commento in start
    public int attackRange;
    public int movementRange;
    public int attackPower;
    public bool moveDone;

    protected List<Vector2Int> nextPos;
    public ShipState currentState = ShipState.Waiting;
    public List<Vector2Int> targetPos;
    public List<Move> shipMoves;
    protected bool canMove;
    protected bool canAttack;
    public int faction;
    public bool startMovement = false;

    //per il camera shake
    private CinemachineImpulseSource impulseSource;

    private ParticleSystem particleSystemInstance;

    //la nave per gli eventi di movimento e attacco ha bisogno di sapere quale effetto sta usando
    protected AbstractEffectSO effectSO;

    protected int oldStatValue;
    protected string oldStatName;
    public float timeToMove = 3f;
    protected bool hasStatChanged = false;


    //ANIMAZIONE DI MOVIMENTO
    protected bool isMoving = false;


    //script di display della salute
    protected DisplayHealth displayHealthScript;

    [Header("Ship Chances")]

    [Tooltip("Parametro per gestire la probabilità di essere colpiti da un attacco")]
    [SerializeField] public int dodgeChance = -1;
    [SerializeField] ParticleSystem dodgeEffect;

    [SerializeField] protected Animator shipAnimator;

    void Start()
    {
        mapHeight = GridManager.Instance._height;
        mapWidth = GridManager.Instance._width;

        shipMoves = new List<Move>();
        displayHealthScript = GetComponent<DisplayHealth>();

        if (shipAnimator == null)
        {
            // Prima provo a trovarlo su questo oggetto
            shipAnimator = GetComponent<Animator>();

            // Se non lo trovo, cerco nei figli
            if (shipAnimator == null)
            {
                shipAnimator = GetComponentInChildren<Animator>();

                if (shipAnimator == null)
                {
                    Debug.LogWarning("Animator non trovato per la nave: " + name);
                }
                else
                {
                    Debug.Log("Animator trovato nei figli per la nave: " + name);
                }
            }
        }

        GetComponent<OnShipAttackEventListener>().AddMethodToExecute(OnAttacked);
        GetComponent<StartedTurnEventListener>().AddMethodToExecute(RestoreStat);

        //camera shake
        impulseSource = GetComponent<CinemachineImpulseSource>();
        displayHealthScript = GetComponent<DisplayHealth>();
    }

    public virtual void ExecuteMove() { }
    public virtual void ExecuteInstructions(AnswerStruct answer)
    {

    }
    public abstract void LookForMoves();

    public abstract bool LookForMovement();
    public abstract bool LookForAttacks();
    public abstract bool LookForAttacks(List<AShip> nearbyShips);

    private FMOD.Studio.EventInstance dodgeSound;

    public void PlayDodge()
    {
        dodgeSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Dodge");
        dodgeSound.start();
        dodgeSound.release();
    }

    public void OnAttacked(ShipAttackStruct attackStruct)
    {
        if (position.x != attackStruct.gridPosition.x || position.y != attackStruct.gridPosition.y)
        {
            return;
        }
        int dodge = Random.Range(0, 4);
        if (dodgeChance >= 0 && dodge <= dodgeChance )
        {
            Debug.Log("Dodge chance: " + dodgeChance);
            Debug.Log(dodgeChance);
            particleSystemInstance = Instantiate(shipSO.dodgeEffect, transform.position + new Vector3(0f,5f,0f), Quaternion.identity);
            particleSystemInstance.Play();
            PlayDodge();  // Play the dodge sound
            return;
        }


        //Test per la creazione dei particle
        //calcolo dell'angolo
        Quaternion correctAngle = Quaternion.FromToRotation(shipSO.attackReceivedParticle.gameObject.transform.up, gameObject.transform.up);
        particleSystemInstance = Instantiate(shipSO.attackReceivedParticle, transform.position, correctAngle);
        particleSystemInstance.Play();
        DoShakeDamageAnimation();

        PlayShipDamage();

        //camera shake
        CameraShakeManager.instance.CameraShake(impulseSource);

        health -= attackStruct.damage;
        displayHealthScript.UpdateHealthBar(attackStruct.damage);
        Debug.Log("health: " + health);
        if (health <= 0)
        {
            GetComponentInChildren<Animator>().SetTrigger("Death");
            shipSO.shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(shipName, faction, position, this));
        }

    }

    private FMOD.Studio.EventInstance shipDamage;

    // Play the ship damage sound
    public void PlayShipDamage()
    {
        shipDamage = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ShipDamage");
        shipDamage.start();
        shipDamage.release();
    }

    public void SetFaction(int faction)
    {
        this.faction = faction;
        if (faction == 0)
        {
            adversaryShipLayer = LayerMask.GetMask("Enemy");
        }
        else if (faction == 1)
        {
            adversaryShipLayer = LayerMask.GetMask("Ally");
        }
    }

    public abstract void SendMessage(Move move);


    /// <summary>
    /// This method is called when the death animation ends. It's called by a script attached to a child gameObject
    /// </summary>
    public virtual void ParentRemoveShip()
    {
        shipSO.shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(shipName, faction, position, this));
    }

    public void SetupShip(ShipSO shipData, string name, int faction, ShipManager2 shipManager2)
    {

        this.shipSO = shipData;
        this.shipName = name;
        this.gameObject.name = this.shipName;
        this.manager = shipManager2;
        this.SetFaction(faction);

        if (faction == (int)Entity.ally) shipInfluence = 1;
        else shipInfluence = -1;

        attackRange = shipData.attackRange;
        movementRange = shipData.movementRange;
        attackPower = shipData.attackPower;
        health = shipData.health;
    }

    public virtual void ReceiveEffect(AbstractEffectSO effectSO)
    {
        this.effectSO = effectSO;
    }


    public void ChangeClass(ShipSO newClass)
    {
        shipSO = newClass;
        attackRange = shipSO.attackRange;
        movementRange = shipSO.movementRange;
        health = shipSO.health;
        attackPower = shipSO.attackPower;

        //cambiare il modello della nave
        ChangeClassModel(newClass);

        //cambiare la salute nel display
        

        //aggiungere particellare/suono/animazione di cambio classe
        if (newClass.changeClassParticle != null)
        {
            ParticleSystem tmpParticle = Instantiate(newClass.changeClassParticle, transform.position, Quaternion.identity);
            tmpParticle.Play();
            
        }

    }

    


    private void ChangeClassModel(ShipSO newClass)
    {
        // Verifica se esiste un animator
        if (gameObject.GetComponentInChildren<Animator>() != null)
        {
            // Ottieni l'animator attuale e il suo GameObject
            Animator shipAnim = gameObject.GetComponentInChildren<Animator>();
            GameObject animatorObject = shipAnim.gameObject;
            
            Debug.Log($"Animator corrente: {shipAnim.name} su GameObject: {animatorObject.name}");
            
            // Cerca l'oggetto "Ship" nella gerarchia
            Transform shipTransform = null;
            
            // Prima cerca tra i figli diretti di questo GameObject
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.ToLower().Contains("ship"))
                {
                    shipTransform = transform.GetChild(i);
                    break;
                }
            }
            
            // Se non lo troviamo direttamente, cerchiamo di risalire la gerarchia dell'animator
            if (shipTransform == null)
            {
                Transform current = animatorObject.transform;
                while (current != null && current != transform)
                {
                    if (current.name.ToLower().Contains("ship"))
                    {
                        shipTransform = current;
                        break;
                    }
                    current = current.parent;
                }
            }
            
            // Se ancora non troviamo Ship, usiamo il parent dell'animator come fallback
            if (shipTransform == null)
            {
                shipTransform = animatorObject.transform.parent;
                Debug.Log($"Oggetto Ship non trovato, usando parent dell'animator: {shipTransform.name}");
            }
            else
            {
                Debug.Log($"Trovato oggetto Ship: {shipTransform.name}");
            }
            
            // Troviamo il modello attuale (con il ShipModelMaterialAssignement)
            ShipModelMaterialAssignement shipModelMaterial = animatorObject.GetComponentInChildren<ShipModelMaterialAssignement>();
            GameObject oldModelObject = shipModelMaterial ? shipModelMaterial.gameObject : animatorObject;
            
            // Prendi il materiale originale
            Material oldMat = shipModelMaterial ? shipModelMaterial.GetMaterial() : null;
            
            // Trova tutti i vecchi modelli sotto Ship e distruggili
            List<GameObject> oldModelsToDestroy = new List<GameObject>();
            for (int i = 0; i < shipTransform.childCount; i++)
            {
                Transform child = shipTransform.GetChild(i);
                // Non distruggere i componenti che non fanno parte del modello
                if (child.GetComponentInChildren<ShipModelMaterialAssignement>() != null)
                {
                    oldModelsToDestroy.Add(child.gameObject);
                }
            }
            
            // Istanzia il nuovo modello sotto l'oggetto Ship
            GameObject newModel = Instantiate(newClass.shipClassModel, shipTransform, false);
            
            Debug.Log($"Nuovo modello istanziato: {newModel.name} con parent: {newModel.transform.parent.name}");
            
            // Applica il materiale al nuovo modello
            ShipModelMaterialAssignement newMaterialAssigner = newModel.GetComponentInChildren<ShipModelMaterialAssignement>();
            if (newMaterialAssigner != null && oldMat != null)
            {
                newMaterialAssigner.AssignMaterialToMeshRenderers(oldMat);
            }
            else if (newMaterialAssigner == null)
            {
                Debug.LogError($"ShipModelMaterialAssignement non trovato nel nuovo modello: {newModel.name}");
            }
            
            // Cerca l'animator nel nuovo modello
            Animator newAnimator = newModel.GetComponent<Animator>();
            if (newAnimator == null)
            {
                newAnimator = newModel.GetComponentInChildren<Animator>();
            }
            
            // Se abbiamo trovato un animator nel nuovo modello, aggiorna il riferimento
            if (newAnimator != null)
            {
                Debug.Log($"Trovato nuovo animator: {newAnimator.name} su GameObject: {newAnimator.gameObject.name}");
                
                // Trasferisci il controller dall'animator vecchio al nuovo
                if (shipAnim.runtimeAnimatorController != null)
                {
                    Debug.Log($"Controller trasferito: {shipAnim.runtimeAnimatorController.name}");
                    newAnimator.runtimeAnimatorController = shipAnim.runtimeAnimatorController;
                }
                
                // Aggiorna il riferimento all'animator nella classe
                shipAnimator = newAnimator;
                
                Debug.Log($"Animator aggiornato con quello del nuovo modello: {newModel.name}");
            }
            else
            {
                Debug.LogError($"Nessun animator trovato nel nuovo modello: {newModel.name}");
            }
            
            // Ora che abbiamo sostituito il modello e aggiornato l'animator, 
            // possiamo distruggere i vecchi modelli
            foreach (GameObject objToDestroy in oldModelsToDestroy)
            {
                Debug.Log($"Distruggo vecchio modello: {objToDestroy.name}");
                Destroy(objToDestroy);
            }
        }
        else
        {
            Debug.LogError("Nessun animator trovato nella nave corrente");
        }
    }


    /// <remark><summary>
    /// This function acts as an attack on the ship
    /// </summary>
    /// <param name="damage"></param> 
    /// <summary>
    /// The damage the ship has to take
    /// </summary></remark>
    public void TakeDamage(int damage)
    {
        Debug.Log("Damage received");


       

        DoShakeDamageAnimation();
        
        shipSO.attackEvent?.Invoke(new ShipAttackStruct(this.position, damage));
    }

    public void DoShakeDamageAnimation()
    {

        Transform shipTransform = transform;
        // Aggiungi effetto vibrazione con DOTween
        transform.DOShakePosition(0.5f, 0.3f, 10, 90, false, true)
            .SetEase(Ease.OutElastic)
            .OnComplete(() =>
            {
                // Assicurati che la nave torni esattamente alla posizione originale
                transform.DOKill(false);
                transform.localPosition = shipTransform.localPosition;

        });
    }


    public void ChangeStat(string statName, int amount)
    {
        hasStatChanged = true;
        oldStatName = statName;
        oldStatValue = shipSO.statsDictionary[statName];

        if (statName == "Movement Range")
        {
            movementRange = oldStatValue + amount;
        }
        else if (statName == "Attack Range")
        {
            attackRange = oldStatValue + amount;
        }
        else if (statName == "Attack Power")
        {
            attackPower = oldStatValue + amount;
            shipSO.statsDictionary[statName] = attackPower;
        }
    }


    //Bisogna associare questa funzione all'ascoltatore del turno
    public void RestoreStat(VoidEvent voidEvent)
    {
        if (hasStatChanged)
        {
            if (oldStatName == "Movement Range")
            {
                movementRange = oldStatValue;
            }
            else if (oldStatName == "Attack Range")
            {
                attackRange = oldStatValue;
            }
            else if (oldStatName == "Attack Power")
            {
                attackPower = oldStatValue;
                shipSO.statsDictionary[oldStatName] = oldStatValue;
            }
            hasStatChanged = false;
        }

    }
    public int GetHealth()
    {
        return health;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public ShipSO GetShipSO()
    {
        return shipSO;
    }

    public Tile GetTileFromShipPosition()
    {
        return GridManager.Instance.GetTileAtPosition(position);
    }

    public void SetShipAnimator(Animator animator)
    {
        shipAnimator = animator;
        Debug.Log($"Animator assegnato manualmente alla nave: {shipName}");
    }

    public bool IsMoving()
    {
        if(faction == (int)Entity.ally)
        {
            return isMoving;
        }
        return false;
    }
}




