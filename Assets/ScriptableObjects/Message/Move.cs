using UnityEngine;

public class Move : ScriptableObject
{
   string idMove;
   Vector2Int targetPos;
   public float value;
   MessageType messageType;

   public Move(string idMove, Vector2Int targetPos, MessageType messageType, float value)
   {
        this.idMove = idMove;
        this.targetPos = targetPos;
        this.messageType = messageType;
        this.value=value;
   }
   public string GetIdMove()
   {
       return idMove;
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
