using UnityEngine;

public interface IObstacleHealth
{
    public abstract void SetHealth(int amount);

    /// <remark><summary>
    /// This faction respond to the attack event, reducing the health of the obstacle
    /// </summary>
    /// <param name="attackStruct"></param> <summary>
    /// The "attackStruct" contains the position of the object to hit.
    /// </summary></remark>
    public abstract void OnAttacked(ShipAttackStruct attackStruct);
    public abstract void ReduceHealth(int damage);
} 