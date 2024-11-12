using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cam : MonoBehaviour
{
    // Update is called once per frame
    public float smooth;
    public GameObject player;
    bool shaking = false;
    void FixedUpdate()
    {
        if (!shaking)
            transform.position = Vector3.Lerp(transform.position, player.transform.position, smooth);
    }

    [SerializeField] float m_roughness;      //거칠기 정도
    [SerializeField] float m_magnitude;      //움직임 범위

    public void ShakingCam(float duration, float rough, float magin)
    {
        StartCoroutine(Shake(duration, rough, magin));
    }

    IEnumerator Shake(float duration, float rough, float magin)
    {
        Debug.Log("SHAKING!!!");
        shaking = true;
        float halfDuration = duration / 2;
        float elapsed = 0f;
        float tick = Random.Range(-10f, 10f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime / halfDuration;

            tick += Time.deltaTime * rough;
            transform.position = player.transform.position + new Vector3(
                Mathf.PerlinNoise(tick, 0) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f,
                0f) * magin * Mathf.PingPong(elapsed, halfDuration);

            yield return null;
        }
        shaking = false;
    }
}
