using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            // Auto-create the instance if it doesn't exist
            if (_instance == null)
            {
                // First check if there's one in the scene
                _instance = FindObjectOfType<SoundManager>();

                // If not, create one
                if (_instance == null)
                {
                    GameObject soundManagerObj = new GameObject("SoundManager");
                    _instance = soundManagerObj.AddComponent<SoundManager>();
                    _instance.InitializeDefaultSettings();
                    DontDestroyOnLoad(soundManagerObj);
                    Debug.Log("SoundManager was auto-created.");
                }
            }

            return _instance;
        }
        private set { _instance = value; }
    }


    // Audio mixer
    [Header("Audio Mixer")] [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot duckingSnapshot;
    [SerializeField] private float transitionTime = 0.3f;

    // Audio sources
    [Header("Audio Sources")] [SerializeField]
    private AudioSource musicSource;

    [SerializeField] private AudioSource sfxSource;

    // Audio clips
    [Header("Music")] [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("Sound Effects")] [SerializeField]
    private AudioClip buttonClickSound;

    [SerializeField] private AudioClip towerPlacedSound;
    [SerializeField] private AudioClip enemyDefeatedSound;
    [SerializeField] private AudioClip towerFireSound;

    // Projectile sounds
    [Header("Projectile Sounds")] [SerializeField]
    private AudioClip arrowImpactSound;

    [SerializeField] private AudioClip cannonImpactSound;
    [SerializeField] private AudioClip turretImpactSound;

    // Dictionary to store sounds
    private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();

    // Volume settings
    private const float MIN_DB = -80f; // Minimum decibel value (effectively muted)
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
        //DontDestroyOnLoad(gameObject);

        // Find and disable any other music players in the scene
        //DisableOtherMusicPlayers();

        // Create audio sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Setup sources with mixer groups if mixer is assigned
        if (audioMixer != null)
        {
            AudioMixerGroup[] masterGroups = audioMixer.FindMatchingGroups("Master");
            if (masterGroups.Length > 0)
            {
                AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Music");
                if (musicGroups.Length > 0)
                    musicSource.outputAudioMixerGroup = musicGroups[0];

                AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
                if (sfxGroups.Length > 0)
                    sfxSource.outputAudioMixerGroup = sfxGroups[0];
            }
            else
            {
                Debug.LogError("Missing Master group in Audio Mixer. Check your Audio Mixer settings.");
            }
        }
        else
        {
            Debug.LogError("No Audio Mixer assigned to Sound Manager. Assign an Audio Mixer to enable volume control.");
        }

        // Configure sources
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        // Initialize sound library
        InitializeSoundLibrary();

        // Load saved settings
        LoadSettings();

        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void DisableOtherMusicPlayers()
    {
        // Find all audio sources in the scene that are not ours
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allSources)
        {
            if (source == musicSource || source == sfxSource)
                continue;

            if (source.playOnAwake || source.isPlaying)
            {
                Debug.LogWarning("Found another AudioSource playing on: " + source.gameObject.name +
                                 ". Disabling it as it might conflict with SoundManager.");
                source.playOnAwake = false;
                source.Stop();

                if (source.clip != null && source.loop)
                {
                    AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Music");
                    if (musicGroups.Length > 0)
                        source.outputAudioMixerGroup = musicGroups[0];
                }
            }
        }
    }

    private void InitializeSoundLibrary()
    {
        // Add music
        if (menuMusic != null) soundLibrary["MenuMusic"] = menuMusic;
        if (gameMusic != null) soundLibrary["GameMusic"] = gameMusic;

        // Add SFX
        if (buttonClickSound != null) soundLibrary["ButtonClick"] = buttonClickSound;
        if (towerFireSound != null) soundLibrary["TowerFire"] = towerFireSound;
        if (towerPlacedSound != null) soundLibrary["TowerPlaced"] = towerPlacedSound;
        if (enemyDefeatedSound != null) soundLibrary["EnemyDefeated"] = enemyDefeatedSound;

        // Add projectile sounds
        if (arrowImpactSound != null) soundLibrary["ArrowImpact"] = arrowImpactSound;
        if (cannonImpactSound != null) soundLibrary["CannonImpact"] = cannonImpactSound;
        if (turretImpactSound != null) soundLibrary["TurretImpact"] = turretImpactSound;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
            PlayMenuMusic();
        else if (scene.name == "Game" || scene.name == "Main")
            PlayGameMusic();

       // DisableOtherMusicPlayers();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(soundLibrary.ContainsKey("MenuMusic") ? soundLibrary["MenuMusic"] : null);
    }

    public void PlayGameMusic()
    {
        PlayMusic(soundLibrary.ContainsKey("GameMusic") ? soundLibrary["GameMusic"] : null);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip != null && (musicSource.clip != clip || !musicSource.isPlaying))
        {
            musicSource.clip = clip;
            if (!isMusicMuted)
            {
                musicSource.Play();
                // Use normal snapshot when starting music
                if (normalSnapshot != null)
                    normalSnapshot.TransitionTo(0.5f);
            }
        }
    }

    // SFX methods
    public void PlayButtonSound()
    {
        if (soundLibrary.ContainsKey("ButtonClick"))
        {
            PlaySoundEffect("ButtonClick");
        }
        else
        {
            // Play a simple beep if button sound isn't available
            sfxSource.pitch = 1.0f;
            sfxSource.PlayOneShot(AudioClip.Create("beep", 4410, 1, 44100, false));
        }
    }

    public void PlayTowerFireSound()
    {
        PlaySoundEffect("TowerFire");
    }

    public void PlayTowerPlacedSound()
    {
        PlaySoundEffect("TowerPlaced");
    }

    public void PlayEnemyDefeatedSound()
    {
        PlaySoundEffect("EnemyDefeated");
    }

    // Play projectile impact sounds with ducking
    public void PlayProjectileImpactSound(Projectile.ProjectileType type, Vector3 position = default)
    {
        string soundKey = "";

        switch (type)
        {
            case Projectile.ProjectileType.Arrow:
                soundKey = "ArrowImpact";
                break;
            case Projectile.ProjectileType.Cannon:
                soundKey = "CannonImpact";
                // For louder sounds like cannon, use ducking
                ApplyDucking();
                break;
            case Projectile.ProjectileType.Turret:
                soundKey = "TurretImpact";
                // For louder sounds like turret, use ducking
                ApplyDucking();
                break;
        }

        if (!string.IsNullOrEmpty(soundKey))
        {
            PlaySoundEffect(soundKey, position);
        }
    }

    // Play a sound by key from dictionary
    public void PlaySoundEffect(string soundKey, Vector3 position = default)
    {
        if (soundLibrary.ContainsKey(soundKey) && !isSfxMuted)
        {
            AudioClip clip = soundLibrary[soundKey];

            // Check if this is an impact sound that should trigger ducking
            bool shouldDuck = soundKey.Contains("Impact") &&
                              (soundKey.Contains("Cannon") || soundKey.Contains("Turret"));

            if (shouldDuck)
                ApplyDucking();

            if (position != default)
            {
                // For 3D sounds, create a temporary object with audio source
                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = position;
                AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

                // Set the output group
                if (audioMixer != null)
                {
                    AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
                    if (sfxGroups.Length > 0)
                        tempSource.outputAudioMixerGroup = sfxGroups[0];
                }

                tempSource.clip = clip;
                tempSource.Play();
                Destroy(tempAudio, clip.length + 0.1f);
            }
            else
            {
                // Play through our SFX source
                sfxSource.PlayOneShot(clip);
            }
        }
    }

    // Apply ducking effect using snapshots
    private void ApplyDucking()
    {
        if (duckingSnapshot != null)
        {
            duckingSnapshot.TransitionTo(transitionTime);
            // Return to normal after a delay
            Invoke("RestoreNormalVolume", 0.5f);
        }
    }

    private void RestoreNormalVolume()
    {
        if (normalSnapshot != null)
            normalSnapshot.TransitionTo(transitionTime);
    }

    // Get a sound clip by key
    public AudioClip GetSoundClip(string soundKey)
    {
        if (soundLibrary.ContainsKey(soundKey))
        {
            return soundLibrary[soundKey];
        }

        return null;
    }

    // Volume control methods
    public void SetMasterVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("MasterVolume", dbValue);
        SaveSettings();
    }

    public void SetMusicVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("MusicVolume", dbValue);
        SaveSettings();
    }

    public void SetSfxVolume(float normalizedValue)
    {
        float dbValue = ConvertToDecibel(normalizedValue);
        audioMixer.SetFloat("SFXVolume", dbValue);
        SaveSettings();
    }

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        float db = isMusicMuted ? MIN_DB : ConvertToDecibel(GetMusicVolume());
        audioMixer.SetFloat("MusicVolume", db);

        // Also handle the music source directly
        if (isMusicMuted)
            musicSource.Pause();
        else if (musicSource.clip != null)
            musicSource.Play();

        SaveSettings();
    }

    public void ToggleSfxMute()
    {
        isSfxMuted = !isSfxMuted;
        float db = isSfxMuted ? MIN_DB : ConvertToDecibel(GetSfxVolume());
        audioMixer.SetFloat("SFXVolume", db);
        SaveSettings();
    }

    // Convert normalized 0-1 value to decibel scale for audio mixer
    private float ConvertToDecibel(float normalizedValue)
    {
        // Avoid log(0) which is -infinity
        if (normalizedValue <= 0)
            return MIN_DB;

        // Convert to logarithmic scale that humans perceive
        return Mathf.Log10(normalizedValue) * 20;
    }

    // Convert from decibel to normalized 0-1 value
    private float ConvertFromDecibel(float dbValue)
    {
        if (dbValue <= MIN_DB)
            return 0;

        return Mathf.Pow(10, dbValue / 20f);
    }

    // Settings persistence
    private void SaveSettings()
    {
        float masterVolume, musicVolume, sfxVolume;

        audioMixer.GetFloat("MasterVolume", out masterVolume);
        audioMixer.GetFloat("MusicVolume", out musicVolume);
        audioMixer.GetFloat("SFXVolume", out sfxVolume);

        PlayerPrefs.SetFloat("MasterVolumeDB", masterVolume);
        PlayerPrefs.SetFloat("MusicVolumeDB", musicVolume);
        PlayerPrefs.SetFloat("SfxVolumeDB", sfxVolume);
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SfxMuted", isSfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolumeDB"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolumeDB");
            float musicVolume = PlayerPrefs.GetFloat("MusicVolumeDB");
            float sfxVolume = PlayerPrefs.GetFloat("SfxVolumeDB");
            isMusicMuted = PlayerPrefs.GetInt("MusicMuted") == 1;
            isSfxMuted = PlayerPrefs.GetInt("SfxMuted") == 1;

            audioMixer.SetFloat("MasterVolume", masterVolume);

            // Apply music volume (accounting for mute)
            if (isMusicMuted)
            {
                audioMixer.SetFloat("MusicVolume", MIN_DB);
                if (musicSource.isPlaying)
                    musicSource.Pause();
            }
            else
            {
                audioMixer.SetFloat("MusicVolume", musicVolume);
                if (musicSource.clip != null && !musicSource.isPlaying)
                    musicSource.Play();
            }

            // Apply SFX volume (accounting for mute)
            if (isSfxMuted)
                audioMixer.SetFloat("SFXVolume", MIN_DB);
            else
                audioMixer.SetFloat("SFXVolume", sfxVolume);
        }
        else
        {
            // Default values for first run
            SetMasterVolume(1.0f);
            SetMusicVolume(0.5f);
            SetSfxVolume(0.7f);
        }
    }

    // Getters for UI
    public float GetMasterVolume()
    {
        float dbValue;
        audioMixer.GetFloat("MasterVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }

    public float GetMusicVolume()
    {
        float dbValue;
        audioMixer.GetFloat("MusicVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }

    public float GetSfxVolume()
    {
        float dbValue;
        audioMixer.GetFloat("SFXVolume", out dbValue);
        return ConvertFromDecibel(dbValue);
    }

    public bool IsMusicMuted()
    {
        return isMusicMuted;
    }

    public bool IsSfxMuted()
    {
        return isSfxMuted;
    }

    private void InitializeDefaultSettings()
    {
        // Find and disable other music players
       // DisableOtherMusicPlayers();

        // Create audio sources if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // Try to load the default audio mixer if one isn't assigned
        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("Audio/GameAudioMixer");
            if (audioMixer == null)
            {
                Debug.LogWarning("No Audio Mixer found. Some audio features may be limited.");
            }
            else
            {
                // Try to get the snapshots
                normalSnapshot = audioMixer.FindSnapshot("Normal");
                duckingSnapshot = audioMixer.FindSnapshot("Ducking");
            }
        }

        // Setup sources with mixer groups if mixer is assigned
        if (audioMixer != null)
        {
            AudioMixerGroup[] masterGroups = audioMixer.FindMatchingGroups("Master");
            if (masterGroups.Length > 0)
            {
                AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Music");
                if (musicGroups.Length > 0)
                    musicSource.outputAudioMixerGroup = musicGroups[0];

                AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("SFX");
                if (sfxGroups.Length > 0)
                    sfxSource.outputAudioMixerGroup = sfxGroups[0];
            }
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
        LoadDefaultAudioClips();
        InitializeSoundLibrary();
        LoadSettings();
    }

    private void LoadDefaultAudioClips()
    {
        // Try to load default audio clips from Resources folder if not assigned
        if (menuMusic == null) menuMusic = Resources.Load<AudioClip>("Audio/MenuMusic");
        if (gameMusic == null) gameMusic = Resources.Load<AudioClip>("Audio/GameMusic");
        if (buttonClickSound == null) buttonClickSound = Resources.Load<AudioClip>("Audio/ButtonClick");

        // You could add other default sounds here...
    }
}