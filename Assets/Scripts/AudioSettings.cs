using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Mute Buttons")]
    [SerializeField] private Button musicMuteButton;
    [SerializeField] private Button sfxMuteButton;
    
    [Header("Volume Text")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    
    [Header("Mute Icons")]
    [SerializeField] private Image musicMuteIcon;
    [SerializeField] private Image sfxMuteIcon;
    [SerializeField] private Sprite audioOnSprite;
    [SerializeField] private Sprite audioOffSprite;
    
    private SoundManager soundManager;
    
    private void Start()
    {
        // Check if SoundManager exists
        soundManager = SoundManager.Instance;
        if (soundManager == null)
        {
            Debug.LogError("No SoundManager found! Make sure it's in your scene.");
            return;
        }
        
        // Set up listeners for sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        
        // Set up listeners for mute buttons
        if (musicMuteButton != null)
            musicMuteButton.onClick.AddListener(OnMusicMuteToggled);
        if (sfxMuteButton != null)
            sfxMuteButton.onClick.AddListener(OnSfxMuteToggled);
            
        // Initialize UI with current values
        RefreshUI();
    }
    
    private void OnEnable()
    {
        // Refresh UI whenever the panel becomes active
        if (soundManager != null)
            RefreshUI();
    }
    
    private void RefreshUI()
    {
        // Update sliders
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = soundManager.GetMasterVolume();
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = soundManager.GetMusicVolume();
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = soundManager.GetSfxVolume();
        
        // Update mute button icons
        UpdateMuteIcons();
        
        // Update volume text displays
        UpdateVolumeText();
    }
    
    // Methods to connect to UI events
    public void OnMasterVolumeChanged(float value)
    {
        soundManager.SetMasterVolume(value);
        UpdateVolumeText();
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        soundManager.SetMusicVolume(value);
        UpdateVolumeText();
    }
    
    public void OnSfxVolumeChanged(float value)
    {
        soundManager.SetSfxVolume(value);
        UpdateVolumeText();
        
        // Play test sound when adjusting SFX volume
        if (value > 0 && !soundManager.IsSfxMuted())
            soundManager.PlayButtonSound();
    }
    
    public void OnMusicMuteToggled()
    {
        soundManager.ToggleMusicMute();
        UpdateMuteIcons();
    }
    
    public void OnSfxMuteToggled()
    {
        soundManager.ToggleSfxMute();
        UpdateMuteIcons();
        
        // Play test sound when unmuting SFX
        if (!soundManager.IsSfxMuted())
            soundManager.PlayButtonSound();
    }
    
    private void UpdateMuteIcons()
    {
        if (musicMuteIcon != null && audioOnSprite != null && audioOffSprite != null)
            musicMuteIcon.sprite = soundManager.IsMusicMuted() ? audioOffSprite : audioOnSprite;
            
        if (sfxMuteIcon != null && audioOnSprite != null && audioOffSprite != null)
            sfxMuteIcon.sprite = soundManager.IsSfxMuted() ? audioOffSprite : audioOnSprite;
    }
    
    private void UpdateVolumeText()
    {
        if (masterVolumeText != null)
            masterVolumeText.text = Mathf.RoundToInt(soundManager.GetMasterVolume() * 100) + "%";
        
        if (musicVolumeText != null)
            musicVolumeText.text = Mathf.RoundToInt(soundManager.GetMusicVolume() * 100) + "%";
        
        if (sfxVolumeText != null)
            sfxVolumeText.text = Mathf.RoundToInt(soundManager.GetSfxVolume() * 100) + "%";
    }
}