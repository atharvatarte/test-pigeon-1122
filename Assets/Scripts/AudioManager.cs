using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    
    [Header("Audio Clips")]
    public AudioClip cardFlipSound;
    public AudioClip cardMatchSound;
    public AudioClip cardMismatchSound;
    public AudioClip gameWinSound;
    
    [Header("Audio Settings")]
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
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
            
        SetupAudioSources();
    }
    
    private void SetupAudioSources()
    {
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
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }
    
    public void MuteSFX(bool mute)
    {
        sfxSource.mute = mute;
    }
} 