using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkinSelectorGrid : View<SkinSelectorGrid>
{
    [Header("Prefab Settings")]
    public GameObject skinItemPrefab;
    public Transform gridParent;
    public Transform topPreviewParent;
    public List<GameObject> brushPrefabs;
    public List<ColorData> brushColorsData;
    public Button backButton;

    [Header("Feature Control")]
    public FeatureData featureData;

    private GameObject currentTopBrush;
    private List<GameObject> _instantiatedBrushes = new List<GameObject>();
    private List<GameObject> _instantiatedSkinItems = new List<GameObject>();

    protected override void Awake()
    {
        
        base.Awake();
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonPressed);
        //InstantiateGridItems();
    }

    private void Start()
    {
        if (featureData == null || !featureData.SkinSelectionScreen)
        {
            gameObject.SetActive(false);
            return;
        }

        PopulateGrid();
        AutoSelectFirstBrush();
    }

    protected override void OnGamePhaseChanged(GamePhase _GamePhase)
    {
        if (_GamePhase == GamePhase.SKINSELECTION)
            Transition(true);
    }

    public void OnBackButtonPressed()
    {
        Transition(false);
        GameManager.Instance.ChangePhase(GamePhase.MAIN_MENU);
    }

    public void PopulateGrid()
    {
        // Initial validation
        if (brushPrefabs.Count == 0 || brushColorsData.Count == 0)
        {
            Debug.LogError("[SkinSelectorGrid] Brush prefabs or ColorData not assigned properly!");
            return;
        }

        // Debug available resources
        Debug.Log($"[SkinSelectorGrid] Initializing grid with {brushPrefabs.Count} brush prefabs and {brushColorsData.Count} color sets");
        for (int i = 0; i < brushPrefabs.Count; i++)
        {
            Debug.Log($"[SkinSelectorGrid] Available brush prefab {i}: {brushPrefabs[i].name}");
        }

        // Clear existing items if any
        ClearGrid();

        // Calculate total items (number of colors × number of brush types)
        int totalItems = brushColorsData.Count * brushPrefabs.Count;
        Debug.Log($"[SkinSelectorGrid] Creating {totalItems} total grid items");

        for (int i = 0; i < totalItems; i++)
        {
            // Create skin item instance
            GameObject skinItemInstance = Instantiate(skinItemPrefab, gridParent);
            _instantiatedSkinItems.Add(skinItemInstance);

            // Find model holder
            Transform modelHolder = skinItemInstance.transform.Find("ModelHolder");
            if (modelHolder == null)
            {
                Debug.LogError($"[SkinSelectorGrid] ModelHolder missing in SkinItemPrefab at index {i}!");
                continue;
            }

            // Calculate brush and color indices
            int brushIndex = (i / brushColorsData.Count) % brushPrefabs.Count;
            int colorIndex = i % brushColorsData.Count;

            Debug.Log($"[SkinSelectorGrid] Item {i}: Using brush {brushIndex} with color {colorIndex}");

            // Instantiate brush
            GameObject brushInstance = Instantiate(brushPrefabs[brushIndex], modelHolder);
            _instantiatedBrushes.Add(brushInstance);

            // Set transform values
            brushInstance.transform.localPosition = Vector3.zero;
            brushInstance.transform.localRotation = Quaternion.identity;
            brushInstance.transform.localScale = Vector3.one * 30f;

            // Get and apply color
            Color baseColor = brushColorsData[colorIndex].m_Colors[0];
            Brush brush = brushInstance.GetComponent<Brush>();
        
            if (brush != null)
            {
                foreach (Renderer renderer in brush.m_Renderers)
                {
                    if (renderer != null)
                    {
                        renderer.material.color = baseColor;
                        Debug.Log($"[SkinSelectorGrid] Applied color R:{baseColor.r}, G:{baseColor.g}, B:{baseColor.b} to brush {brushIndex}");
                    }
                    else
                    {
                        Debug.LogWarning($"[SkinSelectorGrid] Null renderer found in brush at index {i}");
                    }
                }
            }
            else
            {
                Debug.LogError($"[SkinSelectorGrid] Brush component missing on prefab at index {i}");
            }

            // Setup button
            SkinItemButton buttonScript = skinItemInstance.GetComponent<SkinItemButton>();
            if (buttonScript != null)
            {
                buttonScript.Setup(this, brushPrefabs[brushIndex], baseColor);
            }
            else
            {
                Debug.LogError($"[SkinSelectorGrid] SkinItemButton component missing on item at index {i}");
            }
        }

        Debug.Log("[SkinSelectorGrid] Grid population completed");
    }

    private void ClearGrid()
    {
        foreach (var brush in _instantiatedBrushes)
        {
            if (brush != null)
                brush.SetActive(false);
        }
        _instantiatedBrushes.Clear();

        foreach (var item in _instantiatedSkinItems)
        {
            if (item != null)
                item.SetActive(false);
        }
        _instantiatedSkinItems.Clear();

        if (currentTopBrush != null)
        {
            currentTopBrush.SetActive(false);
            currentTopBrush = null;
        }
    }

    public void UpdateTopBrush(GameObject brushPrefab, Color brushColor)
    {
        if (currentTopBrush != null)
        {
            Destroy(currentTopBrush);
        }

        currentTopBrush = Instantiate(brushPrefab, topPreviewParent);
        currentTopBrush.transform.localPosition = Vector3.zero;
        currentTopBrush.transform.localRotation = Quaternion.identity;
        currentTopBrush.transform.localScale = Vector3.one * 50f;

        // --- Correct coloring using Brush script ---
        Brush brush = currentTopBrush.GetComponent<Brush>();
        if (brush != null)
        {
            foreach (Renderer renderer in brush.m_Renderers)
            {
                if (renderer != null)
                    renderer.material.color = brushColor;
            }
        }
        // ------------------------------------------------
    }

    public void AutoSelectFirstBrush()
    {
        if (brushPrefabs.Count > 0 && brushColorsData.Count > 0)
        {
            UpdateTopBrush(brushPrefabs[0], brushColorsData[0].m_Colors[0]);
        }
    }

    public void InstantiateGridItems()
    {
        // Store current active state
        bool wasActive = gameObject.activeSelf;
        
        // Temporarily activate if needed
        if (!wasActive)
            gameObject.SetActive(true);
        
        // Your existing grid population logic here
        PopulateGrid();  // Or whatever your method is called
        AutoSelectFirstBrush();
        // Restore previous state
        if (!wasActive)
            gameObject.SetActive(false);
    }
}