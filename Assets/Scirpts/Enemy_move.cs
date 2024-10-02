using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_move : MonoBehaviour
{
    [Header("속도")]
    public float idle_speed = 2f;
    public float chasing_speed = 3f;

    [Header("감지")]
    public float findDist = 5f;
    public float attackDist = 3f;
    public float missDist = 8f;
    private bool isChasing = false;
    private bool startCoroutine = false;

    Transform target;

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
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
                transform.position = Vector2.MoveTowards(transform.position, target.position, chasing_speed * Time.deltaTime);
        }
        else if (!startCoroutine)
        {
            startCoroutine = true;
            StartCoroutine(IdleMove());
        }
    }

    IEnumerator IdleMove()
    {
        while (!isChasing)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            Vector2 moveTo = (Vector2)transform.position - new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            while ((Vector2)transform.position == moveTo)
                transform.position = Vector2.MoveTowards(transform.position, moveTo, idle_speed * Time.deltaTime);
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
