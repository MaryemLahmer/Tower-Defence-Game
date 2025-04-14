using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

// This class is used to set up the UI in the editor
public class TowerDefenseUISetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("TowerDefense/Create UI")]
    public static void SetupUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("TD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add UI script
        TowerDefenseUI uiScript = canvasObj.AddComponent<TowerDefenseUI>();
        
        // Create top info panel with semi-transparent background
        GameObject topPanel = CreatePanel("TopInfoPanel", canvasObj.transform);
        Image topPanelImage = topPanel.GetComponent<Image>();
        topPanelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.85f);
        
        RectTransform topRect = topPanel.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.pivot = new Vector2(0.5f, 1);
        topRect.sizeDelta = new Vector2(0, 70);
        topRect.anchoredPosition = Vector2.zero;
        
        // Add horizontal layout to top panel
        HorizontalLayoutGroup topLayout = topPanel.AddComponent<HorizontalLayoutGroup>();
        topLayout.padding = new RectOffset(25, 25, 15, 15);
        topLayout.spacing = 30;
        topLayout.childAlignment = TextAnchor.MiddleLeft;
        topLayout.childForceExpandWidth = false;
        
        // Create stat texts for top panel with nicer formatting
        TextMeshProUGUI scoreText = CreateInfoText("ScoreText", "Score: 0", topPanel.transform);
        TextMeshProUGUI multiplierText = CreateInfoText("MultiplierText", "Mult: x1", topPanel.transform);
        TextMeshProUGUI moneyText = CreateInfoText("MoneyText", "100 $", topPanel.transform);
        moneyText.color = new Color(1f, 0.9f, 0.2f); // Gold color for money
        
        // Add flexible space
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(topPanel.transform, false);
        LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
        spacerLayout.flexibleWidth = 1;
        
        // Wave information with improved styling
        TextMeshProUGUI waveText = CreateInfoText("WaveText", "Wave: 1", topPanel.transform);
        TextMeshProUGUI enemiesText = CreateInfoText("EnemiesText", "Enemies: 20", topPanel.transform);
        
        // Create bottom tower selection panel
        GameObject towerPanel = CreatePanel("TowerSelectionPanel", canvasObj.transform);
        Image towerPanelImage = towerPanel.GetComponent<Image>();
        towerPanelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.85f);
        
        RectTransform towerRect = towerPanel.GetComponent<RectTransform>();
        towerRect.anchorMin = new Vector2(0, 0);
        towerRect.anchorMax = new Vector2(1, 0);
        towerRect.pivot = new Vector2(0.5f, 0);
        towerRect.sizeDelta = new Vector2(0, 125);
        towerRect.anchoredPosition = Vector2.zero;
        
        // Add title to the tower panel
        TextMeshProUGUI panelTitle = CreateText("PanelTitle", "Tower Selection", towerPanel.transform);
        RectTransform titleRect = panelTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = new Vector2(0, 0);
        panelTitle.alignment = TextAlignmentOptions.Center;
        panelTitle.fontSize = 18;
        panelTitle.fontStyle = FontStyles.Bold;
        
        // Create tower buttons container (horizontal)
        GameObject buttonsContainer = CreatePanel("TowerButtonsContainer", towerPanel.transform);
        Image containerImage = buttonsContainer.GetComponent<Image>();
        containerImage.color = new Color(0, 0, 0, 0); // Transparent
        
        RectTransform containerRect = buttonsContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(-40, -40);
        containerRect.anchoredPosition = new Vector2(0, -10);
        
        // Container has horizontal layout
        HorizontalLayoutGroup containerLayout = buttonsContainer.AddComponent<HorizontalLayoutGroup>();
        containerLayout.padding = new RectOffset(10, 10, 5, 5);
        containerLayout.spacing = 15;
        containerLayout.childAlignment = TextAnchor.MiddleCenter;
        containerLayout.childForceExpandWidth = false;
        containerLayout.childForceExpandHeight = true;
        
        // Create 4 tower buttons in horizontal layout
        GameObject[] towerButtons = new GameObject[4];
        string[] towerNames = { "Arrow Tower", "Cannon Tower", "Magic Tower", "Catapult Tower" };
        int[] costs = { 100, 150, 200, 250 };
        Color[] towerColors = { 
            new Color(0.3f, 0.7f, 0.9f), // Blue for Arrow
            new Color(0.8f, 0.3f, 0.3f), // Red for Cannon
            new Color(0.7f, 0.3f, 0.9f), // Purple for Magic
            new Color(0.3f, 0.8f, 0.4f)  // Green for Catapult
        };
        
        for (int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i] = CreateModernTowerButton(
                "TowerButton_" + i, 
                buttonsContainer.transform, 
                towerNames[i], 
                costs[i], 
                towerColors[i]
            );
        }
        
        // Set references in the UI script
        SerializedObject so = new SerializedObject(uiScript);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("multiplierText").objectReferenceValue = multiplierText;
        so.FindProperty("moneyText").objectReferenceValue = moneyText;
        so.FindProperty("waveNumberText").objectReferenceValue = waveText;
        so.FindProperty("enemiesRemainingText").objectReferenceValue = enemiesText;
        so.FindProperty("towerSelectionPanel").objectReferenceValue = towerPanel;
        
        SerializedProperty buttonsProp = so.FindProperty("towerButtons");
        buttonsProp.arraySize = towerButtons.Length;
        for (int i = 0; i < towerButtons.Length; i++)
        {
            buttonsProp.GetArrayElementAtIndex(i).objectReferenceValue = towerButtons[i];
        }
        
        so.ApplyModifiedProperties();
        
        Debug.Log("Modern Tower Defense UI created successfully!");
    }
    
    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        
        return panel;
    }
    
    private static TextMeshProUGUI CreateText(string name, string text, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 16;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Left;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 40);
        
        return tmpText;
    }
    
    private static TextMeshProUGUI CreateInfoText(string name, string text, Transform parent)
    {
        TextMeshProUGUI tmpText = CreateText(name, text, parent);
        tmpText.fontSize = 20;
        tmpText.fontStyle = FontStyles.Bold;
        
        // Add background for better readability
        GameObject bg = new GameObject(name + "_BG");
        bg.transform.SetParent(tmpText.transform.parent, false);
        bg.transform.SetAsFirstSibling(); // Place behind text
        
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(-5, -5);
        bgRect.offsetMax = new Vector2(5, 5);
        
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.3f);
        
        return tmpText;
    }
    
    private static GameObject CreateModernTowerButton(string name, Transform parent, string towerName, int cost, Color towerColor)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 90);
        
        Image image = button.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.25f, 0.9f);
        
        Button buttonComp = button.AddComponent<Button>();
        buttonComp.targetGraphic = image;
        
        // Create tower icon at the top
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(button.transform, false);
        
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 1);
        iconRect.anchorMax = new Vector2(0.5f, 1);
        iconRect.pivot = new Vector2(0.5f, 1);
        iconRect.sizeDelta = new Vector2(60, 60);
        iconRect.anchoredPosition = new Vector2(0, -10);
        
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = towerColor;
        
        // Create tower name text
        TextMeshProUGUI nameText = CreateText("NameText", towerName, button.transform);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0);
        nameRect.pivot = new Vector2(0.5f, 0);
        nameRect.sizeDelta = new Vector2(-10, 20);
        nameRect.anchoredPosition = new Vector2(0, 10);
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.fontSize = 14;
        
        // Create cost text
        TextMeshProUGUI costText = CreateText("CostText", cost + " $", button.transform);
        RectTransform costRect = costText.GetComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(1, 0);
        costRect.pivot = new Vector2(0.5f, 0);
        costRect.sizeDelta = new Vector2(-10, 20);
        costRect.anchoredPosition = new Vector2(0, 30);
        costText.alignment = TextAlignmentOptions.Center;
        costText.fontSize = 16;
        costText.fontStyle = FontStyles.Bold;
        costText.color = new Color(1f, 0.9f, 0.2f); // Gold color for cost
        
        return button;
    }
#endif
}