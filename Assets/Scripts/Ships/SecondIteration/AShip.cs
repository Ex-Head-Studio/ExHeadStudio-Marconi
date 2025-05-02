using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
    

    protected List<Vector2Int> nextPos;
    public ShipState currentState=ShipState.Waiting;
    public List<Vector2Int> targetPos;
    public List<Move> shipMoves;
    protected bool canMove;
    protected bool canAttack;
    public int faction;



    void Awake()
    {

        shipMoves = new List<Move>();
        //TODO come detto in altri script, questa cosa va sistemata facendo un singleton corretto
        gridManager= FindFirstObjectByType<GridManager>();
        mapHeight=FindAnyObjectByType<GridManager>()._height;
        mapWidth=FindFirstObjectByType<GridManager>()._width;

        GetComponent<OnShipAttackEventListener>().AddMethodToExecute(OnAttacked);

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
            health-=attackStruct.damage;
            if(health<=0)
            {
                GetComponentInChildren<Animator>().SetTrigger("Death");
            }
        }
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

    public void ChangeClass(ShipSO newClass)
    {
        shipSO = newClass;
        attackRange = shipSO.attackRange;
        movementRange = shipSO.movementRange;
        health = shipSO.health;
    }

    public int GetHealth()
    {
        return health;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }
}




