using UnityEngine;

public class Move 
{
   
   string shipName;
   Vector2Int targetPos;
   public float value;
   MessageType messageType;

   public Move( string shipName, Vector2Int targetPos, MessageType messageType, float value)
   {
     
        this.shipName = shipName;
        this.targetPos = targetPos;
        this.messageType = messageType;
        this.value=value;
   }
   
   public string GetShipName(){
     return shipName;
   }
   public Vector2Int GetTargetPos()
   {
       return targetPos;
   }
   public MessageType GetMessageType()
   {
        return messageType;
   }
   public float GetValue(){
        return value;
   }
}
