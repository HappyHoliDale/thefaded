using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwardContrl : MonoBehaviour
{
    float angle;
    Vector2 target, mouse;
    void Start()
    {
        target = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //angle = 
    }
}
