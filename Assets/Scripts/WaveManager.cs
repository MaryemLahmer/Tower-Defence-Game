using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Phase Settings")]
    [SerializeField] private float placementPhaseDuration = 5f;
    [SerializeField] private float wavePhaseDuration = 18f;
    [SerializeField] private float announcementTime = 3f;
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] private GameObject towerSelectionPanel;
    
    // Current state
    [Header("Current State")]
    [SerializeField] private int currentWave = 0;
    [SerializeField] private bool isPlacementPhase = true;
    [SerializeField] private float currentPhaseTimeRemaining;
    
    // References
    private TowerDefenseUI uiManager;
    private EnemyWaveSpawner waveSpawner;
    
    // Events
    public UnityEvent onPlacementPhaseStart = new UnityEvent();
    public UnityEvent onWavePhaseStart = new UnityEvent();
    public UnityEvent<int> onWaveCompleted = new UnityEvent<int>();
    
    private void Start()
    {
        // Get references
        uiManager = FindObjectOfType<TowerDefenseUI>();
        waveSpawner = FindObjectOfType<EnemyWaveSpawner>();
        
        // Initialize announcement text
        if (announcementText != null)
        {
            announcementText.alpha = 0f;
        }
        
        // Start the game
        StartCoroutine(StartGameWithDelay(2f));
    }
    
    private IEnumerator StartGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartFirstWave();
    }
    
    private void Update()
    {
        // Update timer
        if (currentPhaseTimeRemaining > 0)
        {
            currentPhaseTimeRemaining -= Time.deltaTime;
            
            // Update UI with time remaining
            if (uiManager != null)
            {
                uiManager.UpdatePhaseTimer(Mathf.CeilToInt(currentPhaseTimeRemaining));
            }
            
            // Check if it's time to show announcement for next phase
            if (currentPhaseTimeRemaining <= announcementTime && 
                currentPhaseTimeRemaining > announcementTime - 0.1f)
            {
                if (isPlacementPhase)
                {
                    ShowAnnouncement($"WAVE {currentWave} ENEMIES\nINCOMING IN {Mathf.CeilToInt(announcementTime)} SECONDS!");
                }
                else
                {
                    ShowAnnouncement($"WAVE {currentWave} COMPLETED!\nPLACEMENT PHASE IN {Mathf.CeilToInt(announcementTime)} SECONDS");
                }
            }
            
            // Check if phase is over
            if (currentPhaseTimeRemaining <= 0)
            {
                if (isPlacementPhase)
                {
                    StartWavePhase();
                }
                else
                {
                    CompleteWave();
                }
            }
        }
        
        // Check if all enemies are cleared to end wave early
        if (!isPlacementPhase && waveSpawner != null)
        {
            if (waveSpawner.AreAllEnemiesCleared() && EntitySummoner.enemiesAlive.Count == 0)
            {
                // Only end early if some time has passed (to avoid immediate completion)
                if (currentPhaseTimeRemaining < wavePhaseDuration - 2f)
                {
                    currentPhaseTimeRemaining = Mathf.Min(currentPhaseTimeRemaining, announcementTime);
                }
            }
        }
    }
    
    public void StartFirstWave()
    {
        currentWave = 1;
        ShowAnnouncement($"GAME STARTING\nPREPARE YOUR DEFENSES!");
        
        // Start placement phase after a delay
        StartCoroutine(DelayedStartPlacementPhase(2f));
    }
    
    private IEnumerator DelayedStartPlacementPhase(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPlacementPhase();
    }
    
    private void StartPlacementPhase()
    {
        // Set state
        isPlacementPhase = true;
        currentPhaseTimeRemaining = placementPhaseDuration;
        
        // Enable tower placement
        if (towerSelectionPanel != null)
        {
            towerSelectionPanel.SetActive(true);
        }
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdateWaveNumber(currentWave);
            uiManager.UpdatePhaseStatus("PLACEMENT PHASE");
            uiManager.ToggleTowerSelectionPanel(true);
        }
        
        // Notify listeners
        onPlacementPhaseStart.Invoke();
        
        // Show announcement
        ShowAnnouncement($"WAVE {currentWave}\nPLACEMENT PHASE - {Mathf.CeilToInt(placementPhaseDuration)}s");
        
        Debug.Log($"Wave {currentWave}: Placement phase started. Place your towers! ({placementPhaseDuration} seconds)");
    }
    
    private void StartWavePhase()
    {
        // Set state
        isPlacementPhase = false;
        currentPhaseTimeRemaining = wavePhaseDuration;
        
        // Disable tower placement
        if (towerSelectionPanel != null)
        {
            towerSelectionPanel.SetActive(false);
        }
        
        // Update UI
        if (uiManager != null)
        {
            uiManager.UpdatePhaseStatus("DEFENSE PHASE");
            uiManager.ToggleTowerSelectionPanel(false);
        }
        
        // Notify listeners
        onWavePhaseStart.Invoke();
        
        // Show announcement
        ShowAnnouncement($"WAVE {currentWave} STARTED!\nDEFEND YOUR BASE!");
        
        Debug.Log($"Wave {currentWave}: Defense phase started! Enemies incoming! ({wavePhaseDuration} seconds)");
    }
    
    private void CompleteWave()
    {
        Debug.Log($"Wave {currentWave} completed!");
        
        // Award wave completion bonus
        // You can add bonus rewards here
        
        // Increment wave number
        currentWave++;
        
        // Notify listeners
        onWaveCompleted.Invoke(currentWave - 1);
        
        // Start next placement phase
        StartPlacementPhase();
    }
    
    private void ShowAnnouncement(string message)
    {
        if (announcementText != null)
        {
            StopCoroutine("AnimateAnnouncement");
            StartCoroutine(AnimateAnnouncement(message));
        }
        
        // Also log to console for debugging
        Debug.Log(message);
    }
    
    private IEnumerator AnimateAnnouncement(string message)
{
    // Set text
    announcementText.text = message;
    
    // Fade in
    float fadeInTime = 0.5f;
    float t = 0;
    while (t < fadeInTime)
    {
        announcementText.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
        t += Time.deltaTime;
        yield return null;
    }
    announcementText.alpha = 1;
    
    // Display for full 3 seconds at full opacity
    yield return new WaitForSeconds(3.0f);
    
    // Fade out
    float fadeOutTime = 0.5f;
    t = 0;
    while (t < fadeOutTime)
    {
        announcementText.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
        t += Time.deltaTime;
        yield return null;
    }
    announcementText.alpha = 0;
}
    
    // Utility method for skipping phases (can be connected to UI button)
    public void SkipCurrentPhase()
    {
        currentPhaseTimeRemaining = 0.1f;
    }
}