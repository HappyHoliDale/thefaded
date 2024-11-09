using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Update is called once per frame
    public float smooth;
    public GameObject player;
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, smooth);
    }
}
