using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControl : MonoBehaviour
{
    float angle;
    Vector2 mouse;

    public Transform point; // �ڽ� ������Ʈ(ȸ������ �� ���)

    // Update is called once per frame
    void Update()
    {
        // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �ڽ� ������Ʈ(pivot)�� �������� ���콺���� ������ ���
        angle = Mathf.Atan2(mouse.y - point.position.y, mouse.x - point.position.x) * Mathf.Rad2Deg;

        // �θ� ������Ʈ(Į ��ü)�� ȸ���� �ڽ�(pivot)�� �������� ����
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

    }
}
