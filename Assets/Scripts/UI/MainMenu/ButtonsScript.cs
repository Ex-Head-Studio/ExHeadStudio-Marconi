using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonsScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {

    }

    public void OpensOptions()
    {

    }

    public void BackToMainMenu()
    {
        
    }
}
