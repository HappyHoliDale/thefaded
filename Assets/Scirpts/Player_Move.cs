using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [Header("플레이어 상태")]
    public float hp = 10f;
    public float speed = 5f;


    [Header("대쉬")]
    public float dash_time = 0.2f;
    bool dashing = false;
    public float dash_cooltime = 1f;
    bool dashable = true;
    public float dash_speed_multifly = 3f;

    Rigidbody2D rb;
    SpriteRenderer render;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate()
    {
        Rotate();
        Move();
    }
    void Update()
    {
        Dash();
    }
    void Dash()
    {
        if (dashable && Input.GetKeyDown(KeyCode.Space) && rb.velocity != Vector2.zero)
            StartCoroutine("Dashing");
    }
    IEnumerator Dashing()
    {
        dashing = true;
        dashable = false;
        Vector2 nomalVelocity = rb.velocity;
        rb.velocity = nomalVelocity * dash_speed_multifly;

        yield return new WaitForSeconds(dash_time);
        dashing = false;

        yield return new WaitForSeconds(dash_cooltime);
        dashable = true;
    }
    void Move()
    {
        if (dashing) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(x, y).normalized * speed;
    }
    void Rotate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = transform.position.x;
        float dy = transform.position.y;

        float angle = Mathf.Atan2(mousePos.y - dy, mousePos.x - dx) * Mathf.Rad2Deg;

        if ((angle >= 90) || (angle <= -90))
        {
            render.flipY = true;
        }
        else if ((angle < 90) || (angle > -90))
        {
            render.flipY = false;
        }
    }
}
