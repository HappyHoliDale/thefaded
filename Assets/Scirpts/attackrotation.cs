using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackrotation : MonoBehaviour
{
    public GameObject attackbox;
    public float minX = -5f; // X�� �ּҰ�
    public float maxX = 5f;  // X�� �ִ밪
    public float minY = -3f; // Y�� �ּҰ�
    public float maxY = 3f;  // Y�� �ִ밪

    // Start�� ó���� �� �� ����˴ϴ�.
    void Start()
    {

    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        ObjectMove();
    }

    private void ObjectMove()
    {
        // ���콺�� ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ�մϴ�.
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ���콺 ��ǥ�� ���� ���� �����մϴ�.
        float clampedX = Mathf.Clamp(mousepos.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousepos.y, minY, maxY);

        // ���ѵ� ��ǥ�� ����� ���� ������ ��ġ�� �̵���ŵ�ϴ�.
        attackbox.transform.position = new Vector3(clampedX, clampedY, 0);
    }
}
