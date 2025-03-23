using UnityEngine;

public class Move : ScriptableObject
{
   string idMove;
   Vector2 targetPos;
   MessageType messageType;

   public Move(string idMove, Vector2 targetPos, MessageType messageType)
   {
       this.idMove = idMove;
       this.targetPos = targetPos;
       this.messageType = messageType;
   }
   public string GetIdMove()
   {
       return idMove;
   }
   public Vector2 GetTargetPos()
   {
       return targetPos;
   }
   public MessageType GetMessageType()
   {
         return messageType;
   }
}
