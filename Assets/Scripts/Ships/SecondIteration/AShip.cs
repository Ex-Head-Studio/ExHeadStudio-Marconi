using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] protected MessageSentEvent messageSentEvent;
    [SerializeField] protected OnShipDestroyedEvent shipDestroyedEvent;
    [SerializeField] protected OnShipAttackEvent attackEvent;
    [SerializeField] protected GridManager gridManager;
    [SerializeField] protected Animator shipAnimator;
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

        gridManager= FindFirstObjectByType<GridManager>();
        mapHeight=FindAnyObjectByType<GridManager>()._height;
        mapWidth=FindFirstObjectByType<GridManager>()._width;

    }

    public abstract void ExecuteInstructions(AnswerStruct directives);

    public abstract bool LookForMovement();
    

    public abstract bool LookForAttacks();
    public abstract bool LookForAttacks(List<AShip> nearbyShips);

    public void OnAttacked(ShipAttackStruct attackPosition){
        if(position.x == attackPosition.gridPosition.x && position.y == attackPosition.gridPosition.y)
        {
            //per ora decrementiamo di uno la vita
            health--;
            if(health<=0)
            {
                shipAnimator.SetTrigger("Death");
                //shipDestroyedEvent?.Invoke(new ShipDestroyedStruct(shipName, faction, position));
            }
        }
    }

    public void SetFaction(int faction){
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
}




