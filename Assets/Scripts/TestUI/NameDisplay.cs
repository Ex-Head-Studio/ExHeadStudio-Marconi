using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;

public class NameDisplay : MonoBehaviour {
    public GameObject shipPrefab;
    public Canvas canvas;
    private Camera mainCamera;
    private InputAction showCanvasAction;

    void Start() {
        mainCamera = Camera.main;
        Ship ship = shipPrefab.GetComponent<Ship>();
        if (shipPrefab != null && canvas != null) {
            TMP_Text nameText = canvas.GetComponentInChildren<TMP_Text>();
            if (nameText != null) {
                if (ship != null) {
                    nameText.text = ship.shipName;
                }
            }
        }
        canvas.enabled = false;

        showCanvasAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftShift");
        showCanvasAction.performed += ctx => canvas.enabled = true;
        showCanvasAction.canceled += ctx => canvas.enabled = false;
        showCanvasAction.Enable();
    }

    void Update() {
        if (mainCamera != null && canvas != null) {
            canvas.transform.LookAt(mainCamera.transform);
            canvas.transform.Rotate(0, 180, 0); 
        }
    }
}
