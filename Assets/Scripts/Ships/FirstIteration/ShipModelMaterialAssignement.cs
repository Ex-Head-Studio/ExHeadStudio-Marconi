using UnityEngine;
using System.Collections.Generic;

public class ShipModelMaterialAssignement : MonoBehaviour
{


    [SerializeField] private List<MeshRenderer> shipMeshRenderers = new List<MeshRenderer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AssignMaterialToMeshRenderers(Material material)
    {
        foreach (var meshRenderer in shipMeshRenderers)
        {
            Debug.Log(meshRenderer.material.name);
            if(meshRenderer.material.name != "HolograficShipWeapon")
                meshRenderer.material = material;
        }
    }


}
