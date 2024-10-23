using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackrotation : MonoBehaviour
{
    public GameObject attackbox;
    public float minX = -5f; // X축 최소값
    public float maxX = 5f;  // X축 최대값
    public float minY = -3f; // Y축 최소값
    public float maxY = 3f;  // Y축 최대값

    // Start는 처음에 한 번 실행됩니다.
    void Start()
    {

    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        ObjectMove();
    }

    private void ObjectMove()
    {
        // 마우스의 화면 좌표를 월드 좌표로 변환합니다.
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스 좌표를 범위 내로 제한합니다.
        float clampedX = Mathf.Clamp(mousepos.x, minX, maxX);
        float clampedY = Mathf.Clamp(mousepos.y, minY, maxY);

        // 제한된 좌표를 사용해 공격 상자의 위치를 이동시킵니다.
        attackbox.transform.position = new Vector3(clampedX, clampedY, 0);
    }
}
