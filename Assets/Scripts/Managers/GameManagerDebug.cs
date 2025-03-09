using UnityEngine;

public class GameManagerDebug : MonoBehaviour
{
    public void OnTurnEnded()
    {
        Debug.Log("Turn Ends");
    }

    public void OnTurnStarted()
    {
        Debug.Log("Turn Started");
    }
}
