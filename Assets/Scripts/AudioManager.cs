using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip clip;
    static AudioSource audioSourcez;
    public static AudioClip clipz;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSourcez = this.GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        audioSource.PlayOneShot(clip, GameManager.volume);
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    public static void AudioStart(string clipName, bool stop = false)
    {
        if (stop)
        {
            audioSourcez.Stop();
        }
        clipz = Resources.Load<AudioClip>(clipName);
        audioSourcez.PlayOneShot(clipz, GameManager.volume);
    }

    public static void AudioStop()
    {
        audioSourcez.Stop();
    }
}
