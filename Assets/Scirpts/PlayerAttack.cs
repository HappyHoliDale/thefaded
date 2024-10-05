using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    private float curTime;
    private float cooltime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb=GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (curTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Collider2D[] collider2Ds = Physic2D.OverlapBoxAll()
                animator.SetTrigger("atk");
                curTime = cooltime;
            }
            

        }
        else
        {
            curTime -= Time.deltaTime;
        }
    }
}
