using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDefenseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI multiplierText;
    [SerializeField] public TextMeshProUGUI moneyText;
    [SerializeField] public TextMeshProUGUI waveNumberText;
    [SerializeField] public TextMeshProUGUI enemiesRemainingText;
    
    [Header("Phase Information")]
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Tower Selection")]
    [SerializeField] private GameObject towerSelectionPanel;
    [SerializeField] private GameObject[] towerButtons;
    
    // Start is called with placeholder values
    void Start()
    {
        // Initialize UI with placeholder values
        UpdateScore(0);
        UpdateMultiplier(1);
        UpdateMoney(100);
        UpdateWaveNumber(1);
        UpdateEnemiesRemaining(20);
        UpdatePhaseStatus("PLACEMENT PHASE");
        UpdatePhaseTimer(5);
    }
    
    // Public methods to update UI values
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
    
    public void UpdateMultiplier(int multiplier)
    {
        // Display + sign for positive multipliers
        string prefix = multiplier > 0 ? "+" : "";
        multiplierText.text = "Mult: " + prefix + multiplier.ToString() + "x";
        
        // Optional: Change color based on if multiplier is positive or negative
        multiplierText.color = multiplier >= 0 ? Color.green : Color.red;
    }
    
    public void UpdateMoney(int money)
    {
        moneyText.text = money.ToString() + " $";
    }
    
    public void UpdateWaveNumber(int waveNumber)
    {
        waveNumberText.text = "Wave: " + waveNumber.ToString();
    }
    
    public void UpdateEnemiesRemaining(int enemies)
    {
        enemiesRemainingText.text = "Enemies: " + enemies.ToString();
    }
    
    // New methods for phase information
    public void UpdatePhaseStatus(string phase)
    {
        if (phaseText != null)
        {
            phaseText.text = phase;
            
            // Optional: Change color based on phase
            if (phase.Contains("PLACEMENT"))
            {
                phaseText.color = new Color(0.2f, 0.6f, 1f); // Blue for placement
            }
            else if (phase.Contains("DEFENSE"))
            {
                phaseText.color = new Color(1f, 0.5f, 0.2f); // Orange for defense
            }
        }
    }
    
    public void UpdatePhaseTimer(int seconds)
    {
        if (timerText != null)
        {
            timerText.text = seconds.ToString() + "s";
            
            // Optional: Change color as time runs out
            if (seconds <= 3)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }
    
    // Toggle tower selection panel visibility
    public void ToggleTowerSelectionPanel(bool isVisible)
    {
        towerSelectionPanel.SetActive(isVisible);
    }
    
    // Select a tower for placement
    public void SelectTower(int towerIndex)
    {
        // Highlight the selected tower button
        for (int i = 0; i < towerButtons.Length; i++)
        {
            // Add visual feedback for selected tower
            Image buttonImage = towerButtons[i].GetComponent<Image>();
            buttonImage.color = (i == towerIndex) ? new Color(1f, 0.8f, 0.2f) : Color.white;
        }
        
        // In a full implementation, this would notify a tower placement system
        Debug.Log("Selected tower " + towerIndex);
    }
}