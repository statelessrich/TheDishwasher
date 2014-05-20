using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // BGM audio sources.
    public AudioSource themeBGM;
    public AudioSource scratchBGM;
    
    // One audio source for SFX. Clip will be switched out as needed. 
    // Initially set to the Menu SFX in inspector; otherwise it would be null...
    public AudioSource SFXAudio;
    public AudioSource voicesAudio;
 
    // Clips for SFX AudioSource.
    public AudioClip explosionSFX;
    public AudioClip menuSFX;

    public AudioClip[] voices;
    public ArrayList voicesList;

    private bool playing;

    void Start()
    {
        voicesList = new ArrayList(voices);
    }

    void Update()
    {
        // If voice audio stopped, set flag to false.
        if (playing && !voicesAudio.isPlaying)
        {
            playing = false;
        }
    }

    public void PauseThemeBGM()
    {
        themeBGM.Pause();
    }

    public void PlayThemeBGM()
    {
        if (!themeBGM.isPlaying)
        {
            themeBGM.Play();
        }
    }

    public void PlayMenuSFX()
    {
        SFXAudio.clip = menuSFX;
        SFXAudio.Play();
    }

    public void PlayExplosionSFX()
    {
        SFXAudio.clip = explosionSFX;
        SFXAudio.Play();
    }

    public void PlayRandomVoiceover()
    {
        if (!playing)
        {
            int random = Random.Range(0, voicesList.Count);
            
            if (voicesList.Count == 0) 
            {
                voicesList = new ArrayList(voices);
            }
            voicesAudio.clip = (AudioClip) voicesList[random];
            voicesList.RemoveAt(random);
            voicesAudio.Play();
            playing = true;

            
        }
    }
}
