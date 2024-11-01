using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    public float hp;
    public float damage;
    public Slider hpBar;
    public void getDamage(float damage)
    {
        Debug.Log("¿€µø");
        hp -= damage;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.6f);
        gameObject.layer = 6;
        if (hp <= 0)
        {
            hpBar.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            Destroy(gameObject);
        }
        hpBar.value = hp;
    }

    public void Update()
    {
        hp -= damage;
        hpBar.value = hp;
    }
}
