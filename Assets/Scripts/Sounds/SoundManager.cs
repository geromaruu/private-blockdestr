using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   public static SoundManager instance;

    public AudioClip pickUpSound;
    public AudioClip placeSound;
    public AudioClip destroyLineSound;
    public AudioClip destroyMultiplieLinesSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}
