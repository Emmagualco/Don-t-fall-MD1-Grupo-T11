using UnityEngine;


/// <summary>
/// Zona invisible que muestra un mensaje de ayuda en pantalla y controla la música 
/// cuando el jugador la atraviesa. Requiere un Collider configurado como Trigger.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlayMusic : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [Header("Audio (Emptys con AudioSource)")]
    [Tooltip("El AudioSource que está reproduciendo la música principal del nivel (para detenerla).")]
    [SerializeField] private AudioSource mainMusicAudioSource;

    [Tooltip("El AudioSource propio de esta zona que contiene la nueva música a reproducir.")]
    [SerializeField] private AudioSource zoneMusicAudioSource;

    private bool hasBeenShownOnce;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider != null && !zoneCollider.isTrigger)
        {
            zoneCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PlayerTag))
        {
            return;
        }


        // Gestión de la música
        ManejarMusica();
    }

    private void ManejarMusica()
    {
        // 1. Detener la música principal si existe y está sonando
        if (mainMusicAudioSource != null && mainMusicAudioSource.isPlaying)
        {
            mainMusicAudioSource.Stop();
            // Nota: También puedes usar mainMusicAudioSource.Pause(); si quieres retomarla después.
        }

        // 2. Reproducir la música de esta zona de ayuda
        if (zoneMusicAudioSource != null && !zoneMusicAudioSource.isPlaying)
        {
            zoneMusicAudioSource.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.35f);
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            return;
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(boxCollider.center, boxCollider.size);
    }
}
   

