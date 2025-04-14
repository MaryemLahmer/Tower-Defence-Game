using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // Singleton instance
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

    // Audio sources
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    // Audio clips
    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip towerPlacedSound;
    [SerializeField] private AudioClip enemyDefeatedSound;
    
    // Volume settings
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;
    
    private bool isMusicMuted = false;
    private bool isSfxMuted = false;

    private void Awake()
    {
        // Singleton pattern implementation
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Create audio sources if not assigned
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        
        // Configure sources
        musicSource.loop = true;
        
        // Load saved settings
        LoadSettings();
        
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play appropriate music based on scene
        if (scene.name == "Menu")
            PlayMenuMusic();
        else if (scene.name == "Main")
            PlayGameMusic();
    }
    
    // Music control methods
    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }
    
    public void PlayGameMusic()
    {
        PlayMusic(gameMusic);
    }
    
    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            if (!isMusicMuted)
                musicSource.Play();
        }
    }
    
    // SFX methods
    public void PlayButtonSound()
    {
        PlaySFX(buttonClickSound);
    }
    
    public void PlayTowerPlacedSound()
    {
        PlaySFX(towerPlacedSound);
    }
    
    public void PlayEnemyDefeatedSound()
    {
        PlaySFX(enemyDefeatedSound);
    }
    
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && !isSfxMuted)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }
    
    // Volume control methods
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveSettings();
    }
    
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    
    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        musicSource.mute = isMusicMuted;
        SaveSettings();
    }
    
    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted;
        sfxSource.mute = isSfxMuted;
        SaveSettings();
    }
    
    private void ApplyVolumeSettings()
    {
        musicSource.volume = musicVolume * masterVolume;
    }
    
    // Settings persistence
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume");
            isMusicMuted = PlayerPrefs.GetInt("MusicMuted") == 1;
            isSfxMuted = PlayerPrefs.GetInt("SfxMuted") == 1;
            
            ApplyVolumeSettings();
            musicSource.mute = isMusicMuted;
            sfxSource.mute = isSfxMuted;
        }
    }
    
    // Getters for UI
    public float GetMasterVolume() { return masterVolume; }
    public float GetMusicVolume() { return musicVolume; }
    public float GetSfxVolume() { return sfxVolume; }
    public bool IsMusicMuted() { return isMusicMuted; }
    public bool IsSfxMuted() { return isSfxMuted; }
}