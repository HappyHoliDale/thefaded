using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    public int level;
    public int coin;
    public Tree st;
}
public class Player : MonoBehaviour, ISavable
{

    [Header("플레이어 스킬")]
    public bool _attack = false;
    public bool _dash = false;
    delegate void MyFunc();
    public SkillTree skillTreeScript;


    [Header("플레이어 상태")]
    public float hp = 10f;
    public float speed = 5f;
    public float normalAttackDamage = 1f;
    public float getDamageCool = 1f;
    public float attackCool = 1f;
    public Vector2 attackBoxSize;
    float hitboxDir = 0;
    float hitboxFar = 0.5f;
    bool attackable = true;
    bool damaged = false;
    bool lockOn = false;
    GameObject lockedTarget = null;
    Vector2 targetDir;
    Vector2 lastDir = new Vector2(1, 0);
    public int coin = 0;
    public int level = 1;

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
        Debug.Log("hp:" + hp);

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
        Attack();
        if (Input.GetKeyDown(KeyCode.P))
        {
            DataManager.Instance.SaveGame(false);
            SceneManager.LoadScene("Shami");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            DataManager.Instance.SaveGame(true);
            SceneManager.LoadScene("Shami");
        }
    }
    void Attack()
    {
        OnDrawGizmos();
        if (Input.GetMouseButtonDown(0) && _attack && attackable)
        {
            attackable = false;
            ExecuteFunc(() => attackable = true, attackCool);
            Vector2 startPos = transform.position;
            if (lockOn) startPos += targetDir * hitboxFar;
            else startPos += lastDir * hitboxFar;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, attackBoxSize, hitboxDir);
            foreach (Collider2D collider in collider2Ds)
            {
                Debug.Log(collider.gameObject.name);
                if (collider.gameObject.tag == "Enemy")
                {
                    collider.GetComponent<Enemy_>().GetDamage(normalAttackDamage);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!lockOn)
            {
                lockOn = true;
                float minDistance = -1;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.tag == "Enemy")
                    {
                        if (Vector2.Distance(transform.position, enemy.transform.position) < minDistance || minDistance == -1)
                        {
                            minDistance = Vector2.Distance(transform.position, enemy.transform.position);
                            lockedTarget = enemy.gameObject;
                        }
                    }
                }
            }
            else lockOn = false;
        }
        if (lockOn) LockedOn();
    }
    void OnDrawGizmos()
    {
        // 박스의 중심 위치
        Vector2 startPos = (Vector2)transform.position; // Vector2로 변환
        if (lockOn) startPos += targetDir * hitboxFar;
        else startPos += lastDir * hitboxFar;

        // 박스의 네 모서리를 계산하기 위해 회전 각도를 적용합니다.
        Vector2 halfSize = attackBoxSize * 0.5f;
        Quaternion rotation = Quaternion.Euler(0, 0, hitboxDir);

        // 네 모서리 좌표 계산
        Vector2 topLeft = startPos + (Vector2)(rotation * new Vector3(-halfSize.x, halfSize.y, 0));
        Vector2 topRight = startPos + (Vector2)(rotation * new Vector3(halfSize.x, halfSize.y, 0));
        Vector2 bottomLeft = startPos + (Vector2)(rotation * new Vector3(-halfSize.x, -halfSize.y, 0));
        Vector2 bottomRight = startPos + (Vector2)(rotation * new Vector3(halfSize.x, -halfSize.y, 0));

        // Ray를 사용해 경계를 그립니다.
        Debug.DrawRay(topLeft, topRight - topLeft, Color.yellow);
        Debug.DrawRay(topRight, bottomRight - topRight, Color.yellow);
        Debug.DrawRay(bottomRight, bottomLeft - bottomRight, Color.yellow);
        Debug.DrawRay(bottomLeft, topLeft - bottomLeft, Color.yellow);
    }

    void LockedOn()
    {
        if (lockedTarget == null) return;
        targetDir = (lockedTarget.transform.position - transform.position).normalized;
        hitboxDir = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
    }
    IEnumerator ExecuteFunc(MyFunc func, float wait)
    {
        yield return new WaitForSeconds(wait);
        func();
    }
    void Move()
    {
        if (dashing) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(x, y).normalized;
        rb.velocity = dir * speed;
        if (!lockOn && !(x == 0 && y == 0))
        {
            lastDir = dir;
            hitboxDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }
    void Dash()
    {
        if (_dash && dashable && Input.GetKeyDown(KeyCode.Space) && rb.velocity != Vector2.zero)
            StartCoroutine(Dashing());
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
    void Rotate()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float dx = transform.position.x;
        float dy = transform.position.y;

        float angle = Mathf.Atan2(mousePos.y - dy, mousePos.x - dx) * Mathf.Rad2Deg;

        if ((angle >= 90) || (angle <= -90))
        {
            render.flipX = true;
        }
        else if ((angle < 90) || (angle > -90))
        {
            render.flipX = false;
        }
    }
    public void GetDamage(float damage)
    {
        if (damaged) return;
        damaged = true;
        hp -= damage;
        Debug.Log("hp:" + hp);

        Invoke("GetDamageCool", getDamageCool);
    }
    void GetDamageCool()
    {
        damaged = false;
    }
    public void LoadData(Database data)
    {
        coin = data.playerData.coin;
        level = data.playerData.level;
    }

    public void SaveData(ref Database data)
    {
        data.playerData.coin = coin;
        data.playerData.level = level;
    }
}
