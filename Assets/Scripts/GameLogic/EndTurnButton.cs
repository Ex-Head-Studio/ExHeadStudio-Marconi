using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField] private EndedTurnEvent endedTurnEvent;

    public void EndTurn()
    {
        VoidEvent voidEvent =  new VoidEvent(0);
        endedTurnEvent?.Invoke(voidEvent);
    }
}
