using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LevelButtonScript : MonoBehaviour
{
    private string sceneName;
    private Button buttonScript;

    private void Start()
    {
        buttonScript = GetComponent<Button>();

        sceneName = GetComponentInChildren<TMP_Text>().text;

        buttonScript.onClick.RemoveAllListeners();
        buttonScript.onClick.AddListener(LoadLevel);

    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}