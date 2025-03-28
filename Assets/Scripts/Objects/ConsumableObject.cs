using UnityEngine;

public class ConsumableObject : AbstractObject
{
   [SerializeField] private ConsumableObjectData consumableObjectData;

   private int dropEntity;

   private void Start()
   {
      //sono variabili ereditate
      objectName = consumableObjectData.objectName;
      objectDescription = consumableObjectData.objectDescription;
      objectIllustration = consumableObjectData.objectIllustration;
      dropEntity = consumableObjectData.GetEntityDrop();
   }

   public override void ObjectAction()
   {
      Debug.LogWarning("Consumable object used, non so neache io come");
   }
   public int GetEntityDrop()
   {
      return dropEntity;
   }
}
