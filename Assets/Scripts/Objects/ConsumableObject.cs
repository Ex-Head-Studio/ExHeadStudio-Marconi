using UnityEngine;

public class ConsumableObject : AbstractObject, Idroppable
{

    [SerializeField] private DropEntity dropEntity;

   public override void ObjectAction()
   {
      
   }

   public void Drop()
   {
      Debug.Log("Dropped");
   }
}
