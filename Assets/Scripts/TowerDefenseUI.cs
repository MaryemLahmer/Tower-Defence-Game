using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDefenseUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI waveNumberText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    
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
        waveNumberText.text = "Vague: " + waveNumber.ToString();
    }
    
    public void UpdateEnemiesRemaining(int enemies)
    {
        enemiesRemainingText.text = "Enemies: " + enemies.ToString();
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