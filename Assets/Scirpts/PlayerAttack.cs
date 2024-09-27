using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 3;
    private float curTime;
    private float cooltime = 1f;

    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    private void Attack()
    {
        if (curTime <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                curTime = cooltime;
            }

            else
            {
                curTime = Time.deltaTime;
            }
        }
    }
}
