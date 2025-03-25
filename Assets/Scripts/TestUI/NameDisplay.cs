using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class NameDisplay : MonoBehaviour 
{

    //Modifica: lo script non lavora direttamente sul shipNameText, ma sull'elemento della UI (Stefano)
    [SerializeField] private GameObject shipNameText;
    private Camera mainCamera;
    [SerializeField] private InputActionAsset actionMap;


    //TODO possiamo aggiungere anche un modificatore del colore della nave in base alla fazione


    //input modificato


    private void OnEnable() 
    {
        actionMap.FindActionMap("UI").FindAction("ViewNames").performed += ctx => shipNameText?.SetActive(true);
        actionMap.FindAction("ViewNames").canceled += ctx => shipNameText.SetActive(false);
    }

    private void OnDisable() 
    {
        actionMap.FindActionMap("UI").FindAction("ViewNames").performed -= ctx => shipNameText?.SetActive(true);
        actionMap.FindAction("ViewNames").canceled -= ctx => shipNameText.SetActive(false); 
    }
    void Start() {
        mainCamera = Camera.main;
        Ship ship = gameObject.GetComponent<Ship>();
        if (shipNameText != null) 
        {
            TMP_Text nameText = shipNameText.GetComponentInChildren<TMP_Text>();
            if (nameText != null) {
                if (ship != null) {
                    nameText.text = ship.shipName;
                }
            }

        }

        //assegno la main camera come event camera del shipNameText
        //Modifica: aggiunta ricerca del componente nel padre (Stefano)
        shipNameText.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        shipNameText.SetActive(false);



         
    }

    //gestione della billboard
    void Update() 
    {
        /*if (mainCamera != null && shipNameText != null) 
        {
            shipNameText.transform.LookAt(mainCamera.transform);
            shipNameText.transform.Rotate(0, 0, 180); 
        }*/
    }



    //aggiunta sezione per il display della direzione



}
