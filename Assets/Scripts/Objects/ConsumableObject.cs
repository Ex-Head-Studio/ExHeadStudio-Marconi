using UnityEngine;

public class ConsumableObject : AbstractObject
{

   private int enityDrop;

   public override void ObjectAction()
   {
      
   }
   public int GetEntityDrop()
   {
      return enityDrop;
   }

   public ConsumableObject(ConsumableObjectData data)
   {
      objectName = data.objectName;
      objectDescription = data.objectDescription;
      objectIllustration = data.objectIllustration;
      enityDrop = data.GetEntityDrop();

   }
}
