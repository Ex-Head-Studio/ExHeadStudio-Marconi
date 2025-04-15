using UnityEngine;
using System.Collections.Generic;
public class AllyShip : AShip
{
    
    public override void ExecuteInstructions(AnswerStruct directives)
    {
        // TODO: implementare la logica per eseguire le istruzioni ricevute tramite carta
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
