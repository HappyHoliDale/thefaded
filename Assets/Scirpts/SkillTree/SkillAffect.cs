using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAffect : MonoBehaviour
{
    [SerializeField] Player player;
    public void Dash()
    {
        player._dash = true;
    }
    public void Attack()
    {
        player._attack = true;
    }
}
