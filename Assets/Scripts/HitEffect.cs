using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HitEffect : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    public void Play(Vector3 position, Vector3 normal)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(normal);
        particles.Play();
    }
}