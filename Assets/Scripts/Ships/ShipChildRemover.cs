using UnityEngine;

public class ShipChildRemover : MonoBehaviour
{
    [SerializeField] private Ship parentShipScript;

    public void RemoveShip()
    {
        parentShipScript.ParentRemoveShip();
    }
}
