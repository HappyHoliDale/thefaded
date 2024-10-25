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
        AttackPosition();
    }

    private void Attack()
    {
        if (curTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds) 
                {
                    Debug.Log(collider.tag);
                }
                curTime = cooltime;
            }
       
        }
        else
        {
            curTime -= Time.deltaTime;
        }
    }

    public void AttackPosition()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pos.localPosition = new Vector2(0, 0.1f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pos.localPosition = new Vector2(0, -0.1f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos.localPosition = new Vector2(0.1f, 0f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos.localPosition = new Vector2(-0.1f, 0f);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
