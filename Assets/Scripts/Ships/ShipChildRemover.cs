using UnityEngine;

public class ShipChildRemover : MonoBehaviour
{
    [SerializeField] private NewShip parentShipScript;

    public void RemoveShip()
    {
        parentShipScript.ParentRemoveShip();
    }
}
