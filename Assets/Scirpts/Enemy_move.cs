using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_move : MonoBehaviour
{
    [Header("속도")]
    public float idle_speed = 1f;
    public float chasing_speed = 3f;

    [Header("감지")]
    public float findDist = 5f;
    public float attackDist = 3f;
    public float missDist = 8f;
    private bool isChasing = false;
    private bool startCoroutine = false;

    Transform target;
    Rigidbody2D rb;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        CheckPlayerDistance();
        Move();
    }
    void Move()
    {
        if (isChasing)
        {
            StopCoroutine(IdleMove());
            startCoroutine = false;
            if (Vector2.Distance(transform.position, target.position) > attackDist)
                rb.velocity = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y).normalized * chasing_speed;
        }
        else if (!startCoroutine)
        {
            startCoroutine = true;
            StartCoroutine(IdleMove());
        }
    }
    // void OnCollisionEnter2D(Collision2D collision)
    // {
    // }
    IEnumerator IdleMove()
    {
        while (!isChasing)
        {
            yield return new WaitForSeconds(Random.Range(1f, 4f));
            Vector2 moveTo = (Vector2)transform.position + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            float rnd = Random.Range(-1f, 1f);
            rb.velocity = rnd < 0 ? new Vector2(0, 0) : new Vector2(moveTo.x - transform.position.x, moveTo.y - transform.position.y).normalized * idle_speed;
        }
    }
    void CheckPlayerDistance()
    {
        if (Vector2.Distance(transform.position, target.position) > missDist)
            isChasing = false;
        if (Vector2.Distance(transform.position, target.position) < findDist)
            isChasing = true;
    }
}
