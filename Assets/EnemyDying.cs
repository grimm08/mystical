using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDying : MonoBehaviour
{

    public Animator animator;
   
    public void EnemyIsDying()
    {
        animator.SetTrigger("dead");
        Invoke("DestroyEnemy", 5f);
        this.gameObject.GetComponent<MovingEnemyController>().enabled = false;

    }



    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }


}
