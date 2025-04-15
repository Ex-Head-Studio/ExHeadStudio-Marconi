using UnityEngine;
using System.Collections.Generic;
public class EnemyShip : AShip
{

    public override void ExecuteInstructions(AnswerStruct directives){
        //TODO: Eseguire le istruzioni ricevute da ShipManager
    }

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
   
    
}
