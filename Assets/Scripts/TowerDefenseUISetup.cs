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
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add UI script
        TowerDefenseUI uiScript = canvasObj.AddComponent<TowerDefenseUI>();
        
        // Create top info panel
        GameObject topPanel = CreatePanel("TopInfoPanel", canvasObj.transform);
        RectTransform topRect = topPanel.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.pivot = new Vector2(0.5f, 1);
        topRect.sizeDelta = new Vector2(0, 60);
        topRect.anchoredPosition = Vector2.zero;
        
        // Add horizontal layout to top panel
        HorizontalLayoutGroup topLayout = topPanel.AddComponent<HorizontalLayoutGroup>();
        topLayout.padding = new RectOffset(20, 20, 10, 10);
        topLayout.spacing = 20;
        topLayout.childAlignment = TextAnchor.MiddleLeft;
        
        // Create stat texts for top panel
        TextMeshProUGUI scoreText = CreateText("ScoreText", "Score: 0", topPanel.transform);
        TextMeshProUGUI multiplierText = CreateText("MultiplierText", "Mult: +1x", topPanel.transform);
        TextMeshProUGUI moneyText = CreateText("MoneyText", "100 $", topPanel.transform);
        
        // Add flexible space
        GameObject spacer = new GameObject("Spacer");
        spacer.transform.SetParent(topPanel.transform, false);
        LayoutElement spacerLayout = spacer.AddComponent<LayoutElement>();
        spacerLayout.flexibleWidth = 1;
        
        // Wave information
        TextMeshProUGUI waveText = CreateText("WaveText", "Vague: 1", topPanel.transform);
        TextMeshProUGUI enemiesText = CreateText("EnemiesText", "Enemies: 20", topPanel.transform);
        
        // Create tower selection panel (left side)
        GameObject towerPanel = CreatePanel("TowerSelectionPanel", canvasObj.transform);
        RectTransform towerRect = towerPanel.GetComponent<RectTransform>();
        towerRect.anchorMin = new Vector2(0, 0);
        towerRect.anchorMax = new Vector2(0, 1);
        towerRect.pivot = new Vector2(0, 0.5f);
        towerRect.sizeDelta = new Vector2(250, -60); // Account for top panel
        towerRect.anchoredPosition = new Vector2(0, -30);
        
        // Panel title
        TextMeshProUGUI panelTitle = CreateText("PanelTitle", "Available Towers", towerPanel.transform);
        RectTransform titleRect = panelTitle.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 40);
        titleRect.anchoredPosition = new Vector2(0, -5);
        panelTitle.alignment = TextAlignmentOptions.Center;
        panelTitle.fontSize = 20;
        
        // Create tower buttons container
        GameObject buttonsContainer = CreatePanel("TowerButtonsContainer", towerPanel.transform);
        RectTransform containerRect = buttonsContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(-20, -50);
        containerRect.anchoredPosition = new Vector2(0, -25);
        
        // Container has vertical layout
        VerticalLayoutGroup containerLayout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
        containerLayout.padding = new RectOffset(10, 10, 10, 10);
        containerLayout.spacing = 10;
        containerLayout.childAlignment = TextAnchor.UpperCenter;
        containerLayout.childControlWidth = true;
        containerLayout.childControlHeight = false;
        
        // Create sample tower buttons
        GameObject[] towerButtons = new GameObject[3]; // Adjust number as needed
        
        for (int i = 0; i < towerButtons.Length; i++)
        {
            towerButtons[i] = CreateTowerButton("TowerButton_" + i, buttonsContainer.transform, "Tower " + (i+1), 100 + (i*50));
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
        
        Debug.Log("Tower Defense UI created successfully!");
    }
    
    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.8f);
        
        return panel;
    }
    
    private static TextMeshProUGUI CreateText(string name, string text, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 18;
        tmpText.color = Color.white;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 40);
        
        return tmpText;
    }
    
    private static GameObject CreateTowerButton(string name, Transform parent, string towerName, int cost)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 80);
        
        Image image = button.AddComponent<Image>();
        image.color = Color.white;
        
        Button buttonComp = button.AddComponent<Button>();
        buttonComp.targetGraphic = image;
        
        // Create button layout
        HorizontalLayoutGroup layout = button.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(5, 5, 5, 5);
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        
        // Create tower icon
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(button.transform, false);
        
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = new Color(0.3f, 0.5f, 0.8f); // Placeholder color
        
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(70, 70);
        
        LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
        iconLayout.minWidth = 70;
        iconLayout.minHeight = 70;
        
        // Create info container
        GameObject infoObj = new GameObject("Info");
        infoObj.transform.SetParent(button.transform, false);
        
        RectTransform infoRect = infoObj.GetComponent<RectTransform>();
        
        VerticalLayoutGroup infoLayout = infoObj.AddComponent<VerticalLayoutGroup>();
        infoLayout.childAlignment = TextAnchor.MiddleLeft;
        
        LayoutElement infoElem = infoObj.AddComponent<LayoutElement>();
        infoElem.flexibleWidth = 1;
        
        // Tower name
        TextMeshProUGUI nameText = CreateText("NameText", towerName, infoObj.transform);
        nameText.alignment = TextAlignmentOptions.Left;
        
        // Tower cost
        TextMeshProUGUI costText = CreateText("CostText", cost + " $", infoObj.transform);
        costText.alignment = TextAlignmentOptions.Left;
        costText.color = new Color(0.8f, 0.8f, 0.2f);
        costText.fontSize = 16;
        
        return button;
    }
#endif
}