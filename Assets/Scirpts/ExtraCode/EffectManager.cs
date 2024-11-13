using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] ParticleSystem attackEf;
    [SerializeField] ParticleSystem attackedEf;
    public void OnAttack(float x, float y)
    {
        ParticleSystem particle = Instantiate(attackEf);
        particle.transform.position = new Vector2(x, y);
        StartCoroutine(DeleteParticle(particle, 0.5f));
    }
    public void OnAttacked(float x, float y)
    {
        ParticleSystem particle = Instantiate(attackedEf);
        particle.transform.position = new Vector2(x, y);
        StartCoroutine(DeleteParticle(particle, 0.5f));
    }
    IEnumerator DeleteParticle(ParticleSystem particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(particle);
    }
}
