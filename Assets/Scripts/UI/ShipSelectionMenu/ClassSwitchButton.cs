using UnityEngine;

public class ClassSwitchButton : MonoBehaviour
{
    [SerializeField] private ShipSO[] shipClass; // The ship class associated with this button
    [SerializeField] private GameObject classPanel; // Reference to the class panel to switch to

    public void OnButtonClick()
    {
        // TODO: Implement the logic to switch to the selected ship class panel
        // incrementa l'indice di un vettore secondo l'aritmetica modulare
    }
}
