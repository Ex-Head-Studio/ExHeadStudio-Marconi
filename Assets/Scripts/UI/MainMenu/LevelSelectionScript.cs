using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using DG.Tweening;
using TMPro;

public class LevelSelectionScript : MonoBehaviour
{
    [SerializeField] string[] levels;
    [SerializeField] Transform buttonsParent;
    [SerializeField] GameObject buttonPrefab;

    private void Start()
    {
        GameObject newButton;
        foreach (string level in levels)
        {
            newButton = Instantiate(buttonPrefab, buttonsParent, false);
            newButton.GetComponentInChildren<TMP_Text>().text = level;
            newButton.AddComponent<LevelButtonScript>();
        }
    }

}