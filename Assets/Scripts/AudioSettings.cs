using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
   /* [Header("Mute Toggles")]
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;
    */
    [Header("Volume Text (Optional)")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    
    private SoundManager soundManager;
    
    private void OnEnable()
    {
        soundManager = SoundManager.Instance;
        if (soundManager != null)
        {
            // Initialize UI with current values
            masterVolumeSlider.value = soundManager.GetMasterVolume();
            musicVolumeSlider.value = soundManager.GetMusicVolume();
            sfxVolumeSlider.value = soundManager.GetSfxVolume();
            
          //  musicMuteToggle.isOn = soundManager.IsMusicMuted();
          //  sfxMuteToggle.isOn = soundManager.IsSfxMuted();
            
            UpdateVolumeText();
        }
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
    
   /* public void OnMusicMuteToggled(bool muted)
    {
        soundManager.ToggleMusicMute();
    }
    
    public void OnSfxMuteToggled(bool muted)
    {
        soundManager.ToggleSfxMute();
        
        // Play test sound when unmuting SFX
        if (!muted)
            soundManager.PlayButtonSound();
    }
    */
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