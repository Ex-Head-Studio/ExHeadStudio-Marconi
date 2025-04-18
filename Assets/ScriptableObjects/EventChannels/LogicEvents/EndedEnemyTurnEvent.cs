using UnityEngine;

[CreateAssetMenu(fileName = "EndedEnemyTurnEvent", menuName = "Events/Ended Enemy Turn Event")]
public class EndedEnemyTurnEvent : AbstractEventSO<VoidEvent>
{
}

//devo dichiare una struct vuota perchè non posso passare dati di tipo void
[System.Serializable]
[Tooltip("Non ha nessun valore il parametro passato")]
public struct VoidEvent
{
    public int value;
    public VoidEvent(int value)
    {
        this.value = value;
    }
}