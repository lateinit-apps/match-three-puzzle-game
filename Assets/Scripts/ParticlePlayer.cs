using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] allParticles;

    public float lifetime = 1f;

    public bool destroyImmediately = true;

    private void Start()
    {
        allParticles = GetComponentsInChildren<ParticleSystem>();

        if (destroyImmediately)
        {
            Destroy(gameObject, lifetime);
        }
    }

    public void Play()
    {
        foreach (ParticleSystem particleSystem in allParticles)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }

        Destroy(gameObject, lifetime);
    }
}
