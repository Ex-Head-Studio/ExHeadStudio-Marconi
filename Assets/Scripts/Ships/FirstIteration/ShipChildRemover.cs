using UnityEngine;

public class ShipChildRemover : MonoBehaviour
{
    private AShip parentShipScript;

    private void Start()
    {
        parentShipScript = GetComponentInParent<AShip>();
    }

    public void RemoveShip()
    {
        parentShipScript.ParentRemoveShip();
    }
}
