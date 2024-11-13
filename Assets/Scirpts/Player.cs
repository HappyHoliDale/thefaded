using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
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

    public Cam mainCamera;
    [Header("플레이어 스킬")]
    public bool _attack = false;
    public bool _dash = false;
    public delegate void MyFunc();
    public SkillTree skillTreeScript;

    [Header("스텟 증감")]
    public float damageAdd = 0;
    public float damageSub = 0;
    public float damageMultiply = 1;
    public float speedAdd = 0;
    public float speedsub = 0;
    public float speedMultiplay = 1;
    public float attackCooltimeMul = 1;
    public float dashCooltimeMul = 1;
    public float getdmgCooltimeMul = 1;
    public float knockbackCooltimeMul = 1;


    [Header("플레이어 상태")]
    public float hp = 10f;
    public float speed = 5f;
    public float normalAttackDamage = 1f;
    public float getDamageCool = 1f;
    public float attackCool = 1f;
    public Vector2 attackBoxSize;
    float hitboxDir = 0;
    [SerializeField] float hitboxFar = 0.5f;
    bool attackable = true;
    bool damaged = false;
    bool lockOn = false;
    GameObject lockedTarget = null;
    Vector2 targetDir;
    Vector2 lastDir = new Vector2(1, 0);
    public int coin = 0;
    public int level = 1;
    public float knockback = 10;
    public float knockback_time = 0.5f;
    bool isknockback = false;

    [Header("대쉬")]
    public float dash_time = 0.2f;
    bool dashing = false;
    public float dash_cooltime = 1f;
    bool dashable = true;
    public float dash_speed_multifly = 3f;

    Rigidbody2D rb;
    SpriteRenderer render;
    EffectManager em;

    public void Func(MyFunc func)
    {
        func();
    }

    void Awake()
    {
        Debug.Log("hp:" + hp);
        em = GameObject.Find("GameManager").GetComponent<EffectManager>();
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
            bool atk = false;
            Invoke("AttackCool", attackCool * attackCooltimeMul);
            Vector2 startPos = transform.position;
            if (lockOn) startPos += targetDir * hitboxFar;
            else startPos += lastDir * hitboxFar;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(startPos, attackBoxSize, hitboxDir);

            foreach (Collider2D collider in collider2Ds)
            {
                Debug.Log(collider.gameObject.name);
                if (collider.gameObject.tag == "Enemy")
                {
                    atk = true;
                    collider.GetComponent<Enemy_>().GetDamage(normalAttackDamage, this.gameObject);
                }
            }
            if (atk)
            {
                mainCamera.ShakingCam(0.8f, 30, 1f);
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
    void AttackCool()
    {
        attackable = true;
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
    void Move()
    {
        if (dashing || isknockback) return;

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

        yield return new WaitForSeconds(dash_cooltime * dashCooltimeMul);
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

    public void GetDamage(float damage, GameObject attacker)
    {
        if (damaged) return;
        damaged = true;
        hp -= damage;
        Debug.Log("hp:" + hp);
        isknockback = true;
        mainCamera.ShakingCam(0.8f, 20, 1f);
        em.OnAttacked(transform.position.x, transform.position.y);
        rb.AddForce((gameObject.transform.position - attacker.transform.position).normalized * knockback, ForceMode2D.Impulse);
        Invoke("KnockbackCool", knockback_time * knockbackCooltimeMul);
        Invoke("GetDamageCool", getDamageCool * getdmgCooltimeMul);
    }
    void KnockbackCool()
    {
        isknockback = false;
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
