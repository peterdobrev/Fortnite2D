using UnityEngine;

public class AutoReturnParticle : MonoBehaviour
{
    private ParticleSystem particleSys;

    private void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particleSys.isPlaying)
        {
            ParticleManager.Instance.ReturnShootParticle(particleSys);
        }
    }
}
