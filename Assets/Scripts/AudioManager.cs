using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip cardFlipSound;
    public AudioClip cardMatchSound;
    public AudioClip cardMismatchSound;
    public AudioClip gameWinSound;
    public AudioClip backgroundMusic;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
            
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
            
        SetupAudioSources();
        PlayBackgroundMusic();
    }
    
    private void SetupAudioSources()
    {
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        
        sfxSource.volume = sfxVolume;
    }
    
    public void PlayCardFlip()
    {
        if (cardFlipSound != null)
            sfxSource.PlayOneShot(cardFlipSound);
    }
    
    public void PlayCardMatch()
    {
        if (cardMatchSound != null)
            sfxSource.PlayOneShot(cardMatchSound);
    }
    
    public void PlayCardMismatch()
    {
        if (cardMismatchSound != null)
            sfxSource.PlayOneShot(cardMismatchSound);
    }
    
    public void PlayGameWin()
    {
        if (gameWinSound != null)
            sfxSource.PlayOneShot(gameWinSound);
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
    
    public void MuteMusic(bool mute)
    {
        musicSource.mute = mute;
    }
    
    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }
} 