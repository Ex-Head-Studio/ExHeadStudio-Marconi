using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Cinemachine;

public abstract class AShip : MonoBehaviour
{
        protected enum StrategyState{
        InRange,
        Patrolling
    }
    [Header("Scriptable Objects")]
    [SerializeField] public ShipSO shipSO;

    [Header("Ship Parameters")]
    [SerializeField] protected float nearbyShipSearchRadius;
    [SerializeField] public Vector2Int position;
    public float shipInfluence;

    [SerializeField] protected GridManager gridManager;
    
    protected int mapWidth;
    protected int mapHeight;
    public LayerMask adversaryShipLayer;
    public enum ShipState{
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
    public bool moveDone;

    protected List<Vector2Int> nextPos;
    public ShipState currentState=ShipState.Waiting;
    public List<Vector2Int> targetPos;
    public List<Move> shipMoves;
    protected bool canMove;
    protected bool canAttack;
    public int faction;

    //per il camera shake
    private CinemachineImpulseSource impulseSource;

    private ParticleSystem particleSystemInstance;

    //la nave per gli eventi di movimento e attacco ha bisogno di sapere quale effetto sta usando
    protected AbstractEffectSO effectSO;



    void Awake()
    {

        shipMoves = new List<Move>();
        //TODO come detto in altri script, questa cosa va sistemata facendo un singleton corretto
        gridManager= FindFirstObjectByType<GridManager>();
        mapHeight=FindAnyObjectByType<GridManager>()._height;
        mapWidth=FindFirstObjectByType<GridManager>()._width;

        GetComponent<OnShipAttackEventListener>().AddMethodToExecute(OnAttacked);

        //camera shake
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public virtual void ExecuteMove(){}
    public virtual void ExecuteInstructions(AnswerStruct answer){
        
    }

    public abstract bool LookForMovement();
    public abstract bool LookForAttacks();
    public abstract bool LookForAttacks(List<AShip> nearbyShips);

    public void OnAttacked(ShipAttackStruct attackStruct)
    {

        if(position.x == attackStruct.gridPosition.x && position.y == attackStruct.gridPosition.y)
        {
            //Test per la creazione dei particle
            //calcolo dell'angolo
            Quaternion correctAngle = Quaternion.FromToRotation(shipSO.attackReceivedParticle.gameObject.transform.up, gameObject.transform.up);
            particleSystemInstance = Instantiate(shipSO.attackReceivedParticle, transform.position, correctAngle);
            particleSystemInstance.Play();

            PlayShipDamage();

            //camera shake
            CameraShakeManager.instance.CameraShake(impulseSource);

            health-=attackStruct.damage;
            if(health<=0)
            {
                GetComponentInChildren<Animator>().SetTrigger("Death");
            }
        }
    }

    private FMOD.Studio.EventInstance shipDamage;

    public void PlayShipDamage()
    {
        shipDamage = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/ShipDamage");
        shipDamage.start();
        shipDamage.release();
    }

    public void SetFaction(int faction)
    {
        this.faction=faction;
        if(faction==0)
        {
            adversaryShipLayer=LayerMask.GetMask("Enemy");
        }
        else if(faction==1)
        {
            adversaryShipLayer=LayerMask.GetMask("Ally");
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
            this.manager= shipManager2;
            this.SetFaction(faction);
            
            if(faction == (int)Entity.ally) shipInfluence = 1;
            else shipInfluence = -1;

            attackRange = shipData.attackRange;
            movementRange = shipData.movementRange;
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

        //cambiare il modello della nave

        //aggiungere particellare/suono/animazione di cambio classe
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Damage received");
        shipSO.attackEvent?.Invoke(new ShipAttackStruct(this.position, damage));
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
}




