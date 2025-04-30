using UnityEngine;

public class PushParticleInPool : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particle.isStopped)
            PoolManager.instance.PushObject(gameObject);
    }

    public void Stop()
    {
        particle.Stop();
    }
}
