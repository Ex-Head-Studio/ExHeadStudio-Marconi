using UnityEngine;

public class ShipVFX : MonoBehaviour
{

    [Header("Ship VFX")]
    [SerializeField] private GameObject attackVFX;
    
    public void PlayAttackVFX()
    {
        if (attackVFX != null)
        {
            attackVFX.GetComponent<ParticleSystem>().Play();
        }
    }
}
