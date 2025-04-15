using UnityEngine;
using System.Collections.Generic;
public class AllyShip : AShip
{
    
    public override void ExecuteInstructions(AnswerStruct directives)
    {
        
    }

    //Le funzioni che seguono non servono, direi che possono essere virtual e non abstract
    public override bool LookForMovement()
    {
        throw new System.NotImplementedException();
    }
    public override bool LookForAttacks()
    {
        throw new System.NotImplementedException();
    }
    public override bool LookForAttacks(List<AShip> nearbyShips)
    {
        throw new System.NotImplementedException();
    }
    public override void SendMessage(Move move)
    {
        throw new System.NotImplementedException();
    }

    public void PerformAttack()
    {

    }

    public void PerformMovement()
    {

    }
}
