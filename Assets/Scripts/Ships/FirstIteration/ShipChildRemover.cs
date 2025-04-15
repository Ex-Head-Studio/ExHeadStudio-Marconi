using UnityEngine;

public class ShipChildRemover : MonoBehaviour
{
    [SerializeField] private AShip parentShipScript;

    public void RemoveShip()
    {
        parentShipScript.ParentRemoveShip();
    }
}
