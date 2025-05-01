using UnityEngine;

public class ShipAnimation : MonoBehaviour
{

    //FARE ricerca per animator.play ed eventualment fare SO per le animazioni

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator shipAnimator;
    
    void Start()
    {
        shipAnimator = this.transform.Find("Ship")?.gameObject.GetComponent<Animator>();
    }

    public void playDeathAnimation()
    {
        shipAnimator.SetBool("Death", true);
    }
    public void playAttackAnimation()
    {
        shipAnimator.SetTrigger("Attack");
    }
    public void playMoveAnimation()
    {
        shipAnimator.SetTrigger("Move");
    }
    public void playIdleAnimation()
    {
        shipAnimator.SetTrigger("Idle");
    }


}
