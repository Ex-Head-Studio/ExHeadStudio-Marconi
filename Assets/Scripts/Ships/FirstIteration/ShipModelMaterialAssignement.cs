using UnityEngine;
using System.Collections.Generic;

public class ShipModelMaterialAssignement : MonoBehaviour
{


    [SerializeField] private List<MeshRenderer> shipMeshRenderers = new List<MeshRenderer>();

    private Material currentMaterial;


    public void AssignMaterialToMeshRenderers(Material material)
    {
        currentMaterial = material;
        foreach (var meshRenderer in shipMeshRenderers)
        {
            //            Debug.Log(meshRenderer.material.name);
            if (meshRenderer.material.name != "HolograficShipWeapon")
                meshRenderer.material = material;
        }
    }

    public Material GetMaterial()
    {
        return currentMaterial;
    }


}
