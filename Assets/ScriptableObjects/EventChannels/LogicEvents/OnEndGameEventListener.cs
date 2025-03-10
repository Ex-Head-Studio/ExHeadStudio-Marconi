using UnityEngine;

public class OnEndGameEventListener : AbstractEventListenerSO<int>
{
    public void OnEndGame()
    {
        Debug.Log("End Game, EndGameEventListener");
    }
}
