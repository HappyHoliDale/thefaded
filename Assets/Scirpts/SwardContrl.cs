using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControl : MonoBehaviour
{
    float angle;
    Vector2 mouse;

    public Transform point; // 자식 오브젝트(회전축이 될 요소)

    // Update is called once per frame
    void Update()
    {
        // 마우스 위치를 월드 좌표로 변환
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 자식 오브젝트(pivot)를 기준으로 마우스와의 방향을 계산
        angle = Mathf.Atan2(mouse.y - point.position.y, mouse.x - point.position.x) * Mathf.Rad2Deg;

        // 부모 오브젝트(칼 본체)의 회전을 자식(pivot)을 기준으로 변경
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

    }
}
