using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class NameDisplay : MonoBehaviour 
{
    [SerializeField] private GameObject canvas;
    private Camera mainCamera;
    [SerializeField] private InputActionAsset actionMap;

    void Start() {
        mainCamera = Camera.main;
        Ship ship = gameObject.GetComponent<Ship>();
        if (canvas != null) 
        {
            TMP_Text nameText = canvas.GetComponentInChildren<TMP_Text>();
            if (nameText != null) {
                if (ship != null) {
                    nameText.text = ship.shipName;
                }
            }

        }

        //assegno la main camera come event camera del canvas
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
        canvas.SetActive(false);

        //input modificato
        actionMap.FindActionMap("UI").FindAction("ViewNames").performed += ctx => canvas.SetActive(true);
        actionMap.FindAction("ViewNames").canceled += ctx => canvas.SetActive(false);
    }

    //gestione della billboard
    void Update() 
    {
        if (mainCamera != null && canvas != null) 
        {
            canvas.transform.LookAt(mainCamera.transform);
            canvas.transform.Rotate(0, 180, 0); 
        }
    }
}
