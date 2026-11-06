using UnityEngine;
using UnityEngine.Audio;

public class MixerPitchvariator : MonoBehaviour
{
    [SerializeField] private AudioMixer targetMixer;
    [SerializeField] private float minPitch = 0.85f;
    [SerializeField] private float maxPitch = 1.15f;

    void Update()
    {
        // Elige un tono aleatorio en cada fotograma para el siguiente sonido
        float randomPitch = Random.Range(minPitch, maxPitch);
        targetMixer.SetFloat("PitchEfectos", randomPitch);
    }
}
