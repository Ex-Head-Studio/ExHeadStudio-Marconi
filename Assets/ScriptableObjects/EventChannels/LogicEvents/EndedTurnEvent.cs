using UnityEngine;

[CreateAssetMenu(fileName = "EndedTurnEvent", menuName = "Events/Ended Turn Event")]
public class EndedTurnEvent : AbstractEventSO<VoidEvent>
{
}



//devo dichiare una struct vuota perchè non posso passare dati di tipo void
[System.Serializable]
public struct VoidEvent
{
    public int value;
    public VoidEvent(int value)
    {
        this.value = value;
    }
}
