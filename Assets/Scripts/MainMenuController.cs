using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel; 
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        
        if (OptionPanel != null)
        {
            OptionPanel.alpha = 0;
            OptionPanel.blocksRaycasts = false;
        }
    }

    public void PlayGame()
    {
        PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Option()
    {
        PlayButtonSound();
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
    }

    public void Back()
    {
        PlayButtonSound();
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
    }

    public void QuitGame()
    {
        PlayButtonSound();
        Application.Quit();
    }
    
    private void PlayButtonSound()
    {
        if (soundManager != null)
        {
            soundManager.PlayButtonSound();
        }
    }
}
