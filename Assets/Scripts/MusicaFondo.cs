using UnityEngine;
using UnityEngine.Audio;

public class MusicaFondo : MonoBehaviour
{
   public AudioSource Music;

    private void Start()
    {
        Music = GetComponent<AudioSource>();
    }

}
