using TMPro;
using UnityEngine;

public class StatsPanelScript : MonoBehaviour
{
    [SerializeField] private TMP_Text allyText;
    [SerializeField] private TMP_Text enemyText;

    [SerializeField] private OnEndGameEvent endGameEvent;

    //queste sono da cambiare, bisogna linkarli allo SO dell navi;
    //bisogna essere sicuri di ridurre il numero anche nello SO
    [SerializeField] private int enemyShips = 3;
    [SerializeField] private int allyShips = 3;

    private void Start()
    {
        allyText.text = "Ally ships: " + allyShips;
        enemyText.text = "Enemy ships: " + enemyShips;
    }
    public void UpdateStatsPanel(ShipDestroyedStruct shipDestroyed)
    {
        if(shipDestroyed.entity == (int)Entity.ally)
        {
            allyShips -= 1;
            allyText.text = "Ally ships: " + allyShips;

        }
        else
        {
            enemyShips -= 1;
            enemyText.text = "Enemy ships: " + enemyShips;
        }

        if(enemyShips == 0 || allyShips == 0)
        {
            Debug.Log("EndGame: " + (enemyShips == 0 ? "Ally" : "Enemy") + " wins");
            //passiamo il perdente
            endGameEvent?.Invoke(enemyShips == 0 ? (int)Entity.enemy : (int)Entity.ally);
        }
    }

}
