using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private LayerMask placementCheckMask;
    [SerializeField] private LayerMask placementCollideMask;
    [SerializeField] private Camera playerCamera;
    
    [Header("Tower Settings")]
    [SerializeField] private GameObject[] towerPrefabs;
    [SerializeField] private int[] towerCosts;
    [SerializeField] private Color validPlacementColor = new Color(0.5f, 1f, 0.5f, 0.7f);
    [SerializeField] private Color invalidPlacementColor = new Color(1f, 0.5f, 0.5f, 0.7f);
    
    private GameObject currentPlacingTower;
    private int currentTowerIndex = -1;
    private bool isValidPlacement = false;
    
    // Reference to economy manager
    private EconomyManager economyManager;
    
    void Start()
    {
        // Find camera if not assigned
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        // Get economy manager
        economyManager = FindObjectOfType<EconomyManager>();
        if (economyManager == null)
            Debug.LogError("No EconomyManager found in scene! Tower purchases won't work.");
    }

    void Update()
    {
        if (currentPlacingTower != null)
        {
            // Cast ray from mouse to placement surface
            Ray camRay = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool didHit = Physics.Raycast(camRay, out hit, 100f, placementCollideMask);

            if (didHit)
            {
                // Move tower preview to mouse position
                Vector3 snapPosition = new Vector3(
                    Mathf.Round(hit.point.x), 
                    hit.point.y, 
                    Mathf.Round(hit.point.z)
                );
                currentPlacingTower.transform.position = snapPosition;
                
                // Check if placement is valid
                isValidPlacement = IsValidPlacement(snapPosition);
                UpdatePreviewColor(isValidPlacement);

                // Handle click to place tower
                if (Input.GetMouseButtonDown(0) && isValidPlacement)
                {
                    if (AttemptPurchase())
                    {
                        PlaceTower(snapPosition);
                    }
                    else
                    {
                        Debug.Log("Not enough money to place tower.");
                    }
                }
                
                // Cancel placement with right-click or Escape
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    CancelPlacement();
                }
            }
        }
    }
    
    // Called from UI when tower button is clicked
    public void SetTowerToPlacement(int towerIndex)
    {
        // Cancel any existing placement
        CancelPlacement();
        
        // Validate tower index
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length)
        {
            Debug.LogError($"Invalid tower index: {towerIndex}");
            return;
        }
        
        // Check if we can afford this tower
        int cost = GetTowerCost(towerIndex);
        if (economyManager != null && !economyManager.CanAfford(cost))
        {
            Debug.Log($"Cannot afford tower: {cost} money required");
            return;
        }
        
        // Create preview tower
        currentTowerIndex = towerIndex;
        currentPlacingTower = Instantiate(towerPrefabs[towerIndex], new Vector3(0, -100f, 0), Quaternion.identity);
        
        // Set scale for preview
        currentPlacingTower.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        
        // Disable any tower behavior scripts
        MonoBehaviour[] scripts = currentPlacingTower.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script.GetType().Name != "Transform")
                script.enabled = false;
        }
        
        // Set initial color
        UpdatePreviewColor(true);
    }
    
    // Legacy method for backward compatibility
    public void SetTowerToPlacement(GameObject tower)
    {
        // Find the index of this tower in our prefabs array
        for (int i = 0; i < towerPrefabs.Length; i++)
        {
            if (towerPrefabs[i].name == tower.name.Replace("(Clone)", "").Trim())
            {
                SetTowerToPlacement(i);
                return;
            }
        }
        
        // If we can't find it, just create the preview without economy integration
        Debug.LogWarning("Tower not found in prefabs array, placing without economy check");
        CancelPlacement();
        currentPlacingTower = Instantiate(tower, new Vector3(0, -100f, 0), Quaternion.identity);
        currentPlacingTower.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
    
    // Check if placement is valid at this position
    private bool IsValidPlacement(Vector3 position)
    {
        // Use slightly larger radius than your original code for better detection
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f, placementCheckMask);
        
        foreach (Collider col in hitColliders)
        {
            if (col.gameObject.CompareTag("VirginCell"))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Update tower preview color based on placement validity
    private void UpdatePreviewColor(bool isValid)
    {
        if (currentPlacingTower == null) return;
        
        // Get all renderers
        Renderer[] renderers = currentPlacingTower.GetComponentsInChildren<Renderer>();
        
        // Set color for all materials
        Color previewColor = isValid ? validPlacementColor : invalidPlacementColor;
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                
                // Enable transparency
                SetMaterialTransparent(material);
                
                // Set color
                material.color = previewColor;
            }
            
            renderer.materials = materials;
        }
    }
    
    // Helper to set material to transparent mode
    private void SetMaterialTransparent(Material material)
    {
        material.SetFloat("_Mode", 3); // Transparent mode
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
    
    // Try to purchase the current tower
    private bool AttemptPurchase()
    {
        if (economyManager == null || currentTowerIndex < 0)
            return true; // No economy system, allow placement
            
        int cost = GetTowerCost(currentTowerIndex);
        return economyManager.TryPurchase(cost);
    }
    
    // Get cost for a tower type
    private int GetTowerCost(int index)
    {
        if (index >= 0 && index < towerCosts.Length)
            return towerCosts[index];
            
        return 100; // Default cost
    }
    
    // Place tower at position and update cell
    private void PlaceTower(Vector3 position)
    {
        if (currentPlacingTower == null) return;
        
        // Find the cell at this position
        Collider[] hitColliders = Physics.OverlapSphere(position, 0.5f, placementCheckMask);
        GameObject cellObj = null;
        
        foreach (Collider col in hitColliders)
        {
            if (col.gameObject.CompareTag("VirginCell"))
            {
                cellObj = col.gameObject;
                break;
            }
        }
        
        // Create final tower (not the preview)
        GameObject finalTower = null;
        
        if (currentTowerIndex >= 0 && currentTowerIndex < towerPrefabs.Length)
        {
            finalTower = Instantiate(towerPrefabs[currentTowerIndex], position, Quaternion.identity);
        }
        else
        {
            // If we don't know the index, just clone the preview
            finalTower = Instantiate(currentPlacingTower, position, Quaternion.identity);
            
            // Re-enable scripts that might have been disabled
            MonoBehaviour[] scripts = finalTower.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = true;
            }
        }
        
        // Set final tower scale
        finalTower.transform.localScale = Vector3.one;
        
        // Reset materials to non-transparent
        Renderer[] renderers = finalTower.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.white;
        }
        
        // Update cell status
        if (cellObj != null)
        {
            cellObj.tag = "UsedCell";
        }
        
        // Log placement
        Debug.Log($"Tower placed at {position}");
        
        // Clean up preview
        Destroy(currentPlacingTower);
        currentPlacingTower = null;
        currentTowerIndex = -1;
    }
    
    // Cancel current placement
    private void CancelPlacement()
    {
        if (currentPlacingTower != null)
        {
            Destroy(currentPlacingTower);
            currentPlacingTower = null;
        }
        currentTowerIndex = -1;
    }
}