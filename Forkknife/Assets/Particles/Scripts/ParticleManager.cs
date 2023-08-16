using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private ParticleSystem shootParticlePrefab;
    private Queue<ParticleSystem> shootParticlesPool = new Queue<ParticleSystem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ParticleSystem GetShootParticle()
    {
        if (shootParticlesPool.Count == 0)
        {
            return Instantiate(shootParticlePrefab);
        }
        else
        {
            return shootParticlesPool.Dequeue();
        }
    }

    public void ReturnShootParticle(ParticleSystem particle)
    {
        particle.Stop();
        shootParticlesPool.Enqueue(particle);
    }
}
