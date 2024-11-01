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
        if(Input.GetKey(KeyCode.W)
        if (Input.GetKey(KeyCode.W))
        {
            pos.localPosition = new Vector2(0, 0.1f);
            pos.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (Input.GetKey(KeyCode.S))
        {
            pos.localPosition = new Vector2(0, -0.1f);
            pos.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos.localPosition = new Vector2(0.1f, 0f);
            pos.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            pos.localPosition = new Vector2(-0.1f, 0f);
            pos.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // ���� Matrix�� ����
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // pos�� ��ġ�� ȸ���� �ݿ��� Matrix ����
        Gizmos.matrix = Matrix4x4.TRS(pos.position, pos.rotation, Vector3.one);

        // ȸ���� ���·� WireCube �׸���
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // Matrix�� ���� ���·� �ǵ�����
        Gizmos.matrix = originalMatrix;
    }

}
