using UnityEngine;
using System.Collections.Generic;

public class ShipVFX : MonoBehaviour
{

    [Header("Ship VFX")]
    [SerializeField] private List<GameObject> attackVFX;
    
    public void PlayAttackVFX()
    {
        foreach (GameObject vfx in attackVFX)
        {
            if (vfx != null)
            {
                vfx.SetActive(true);
            }
        }
    }
}
