using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
        // defaultLayerMask = LayerMask.GetMask("Default");
        rb = GetComponent<Rigidbody2D>();

    }
    void FixedUpdate()
    {
        Move();
        CheckPlayerDistance();
    }
    void Move()
    {
        if (isChasing)
        {
            StopCoroutine(IdleMove());
            startCoroutine = false;
            FindWayAndMove();
        }
        else if (!startCoroutine)
        {
            startCoroutine = true;
            StartCoroutine(IdleMove());
        }
    }
    const float cirslide = 0.41421f;
    Vector2[] findDir = { //16 방향
        new Vector2(-1, 1).normalized,
        new Vector2(-cirslide, 1).normalized,
        new Vector2(0, 1).normalized,
        new Vector2(cirslide, 1).normalized,
        new Vector2(1, 1).normalized,
        new Vector2(-1, cirslide).normalized,
        new Vector2(1, cirslide).normalized,
        new Vector2(-1, 0).normalized,
        new Vector2(1, 0).normalized,
        new Vector2(-1, -cirslide).normalized,
        new Vector2(1, -cirslide).normalized,
        new Vector2(-1, -1).normalized,
        new Vector2(-cirslide, -1).normalized,
        new Vector2(0, -1).normalized,
        new Vector2(cirslide, -1).normalized,
        new Vector2(1, -1).normalized,
    };
    int flag = 0;
    void FindWayAndMove()
    {
        Vector2 lossy = transform.lossyScale;
        Vector2 pos = (Vector2)transform.position;
        Vector2 finPos = new Vector2(0, 0);

        Vector2 direction = new Vector2(target.position.x - pos.x, target.position.y - pos.y).normalized;
        float distance = Vector2.Distance(pos, target.transform.position);
        LayerMask layerMask = LayerMask.GetMask("Obstacle");

        RaycastHit2D laser1 = Physics2D.CapsuleCast(pos + new Vector2(0, lossy.y * 0.5f), lossy * 0.5f, CapsuleDirection2D.Vertical, 0f, direction, distance, layerMask);
        RaycastHit2D laser2 = Physics2D.CapsuleCast(pos + new Vector2(0, lossy.y * -0.5f), lossy * 0.5f, CapsuleDirection2D.Vertical, 0f, direction, distance, layerMask);
        flag = 0;

        if (laser1.collider == null && laser2.collider == null)
        {
            Debug.DrawRay(pos, direction * distance, Color.red);
            if (Vector2.Distance(pos, target.position) > attackDist)
                rb.velocity = new Vector2(target.position.x - pos.x, target.position.y - pos.y).normalized * chasing_speed;
        }
        else
        {
            Vector2 findir = new Vector2(0, 0);
            float findist = 0;
            float finallDist = missDist + 2;
            for (float j = 0.5f; j < missDist; j += 0.5f)
            {

                for (int i = 0; i < 16; i++)
                {
                    Vector2 secPos = pos + findDir[i] * j;
                    Vector2 secDirection = new Vector2(target.position.x - secPos.x, target.position.y - secPos.y).normalized;
                    float secDistance = Vector2.Distance(secPos, target.transform.position);
                    RaycastHit2D secLaser = Physics2D.CapsuleCast(secPos, lossy, CapsuleDirection2D.Vertical, 90f, secDirection, secDistance, layerMask);
                    // Debug.DrawRay(secPos, secDirection * secDistance, Color.yellow);

                    if (secLaser.collider == null && Vector2.Distance(secPos, target.position) < missDist)
                    {
                        Vector2 thrdDirection = new Vector2(secPos.x - pos.x, secPos.y - pos.y).normalized;
                        float thrdDistance = Vector2.Distance(pos, secPos);
                        RaycastHit2D thrdLaser1 = Physics2D.CapsuleCast(pos + new Vector2(0, lossy.y * 0.5f), lossy * 0.5f, CapsuleDirection2D.Vertical, 90f, thrdDirection, thrdDistance, layerMask);
                        RaycastHit2D thrdLaser2 = Physics2D.CapsuleCast(pos + new Vector2(0, lossy.y * -0.5f), lossy * 0.5f, CapsuleDirection2D.Vertical, 90f, thrdDirection, thrdDistance, layerMask);
                        // Debug.DrawRay(pos, thrdDirection * thrdDistance, Color.blue);
                        if (thrdLaser1.collider == null && thrdLaser2.collider == null)
                        {
                            flag++;
                            if (Vector2.Distance(pos, secPos) + Vector2.Distance(secPos, target.transform.position) < finallDist || i == 0)
                            {
                                finPos = secPos;
                                findir = secDirection;
                                findist = secDistance;
                                finallDist = Vector2.Distance(pos, secPos) + Vector2.Distance(secPos, target.transform.position); // 놓칠 경우 그냥 가지 말고 두번째 포인트까진 가게 수정해야할듯
                            }
                        }
                    }
                }

            }
            if (finPos != Vector2.zero)
            {
                Debug.DrawRay(pos, new Vector2(finPos.x - pos.x, finPos.y - pos.y).normalized * Vector2.Distance(pos, finPos), Color.green);
                Debug.DrawRay(finPos, findir * findist, Color.green);
                rb.velocity = new Vector2(finPos.x - pos.x, finPos.y - pos.y).normalized * chasing_speed;
            }
        }
    }
    IEnumerator IdleMove()
    {
        while (!isChasing)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
            Vector2 moveTo = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            float rnd = UnityEngine.Random.Range(-1f, 1f);
            rb.velocity = rnd < 0 ? new Vector2(0, 0) : new Vector2(moveTo.x - transform.position.x, moveTo.y - transform.position.y).normalized * idle_speed;
        }
    }
    void CheckPlayerDistance()
    {
        if (Vector2.Distance(transform.position, target.position) > missDist)
            isChasing = false;
        if (Vector2.Distance(transform.position, target.position) < findDist)
        {
            Vector2 lossy = transform.lossyScale;
            Vector2 pos = (Vector2)transform.position;

            Vector2 direction = new Vector2(target.position.x - pos.x, target.position.y - pos.y).normalized; // 이걸 각각 계산해서 플레이어 중앙으로 함 해보기
            float distance = Vector2.Distance(pos, target.transform.position);
            LayerMask layerMask = LayerMask.GetMask("Obstacle");

            RaycastHit2D laser = Physics2D.Raycast(pos, direction, distance, layerMask);

            if (laser.collider == null)
            {
                isChasing = true;
            }
            else
            {
                Debug.DrawRay(pos, direction * distance, Color.yellow);
            }
        }
    }
}
// 스프라이트 반전 추가할것