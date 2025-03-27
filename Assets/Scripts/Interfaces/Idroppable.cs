using UnityEngine;

     public enum DropEntity{
        ally,
        enemy,
        obstacle
    }

public interface Idroppable
{


    public abstract void Drop();
}
