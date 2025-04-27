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

    private void PopulateGrid()
    {
        if (brushPrefabs.Count == 0 || brushColorsData.Count == 0)
        {
            Debug.LogError("Brush prefabs or ColorData not assigned properly!");
            return;
        }

        int totalItems = brushColorsData.Count * 2; // 6 colors × 2 brushes

        for (int i = 0; i < totalItems; i++)
        {
            GameObject skinItemInstance = Instantiate(skinItemPrefab, gridParent);
            _instantiatedSkinItems.Add(skinItemInstance);

            Transform modelHolder = skinItemInstance.transform.Find("ModelHolder");
            if (modelHolder == null)
            {
                Debug.LogError("ModelHolder missing inside SkinItemPrefab!");
                continue;
            }

            // --- UPDATED LOGIC ---
            int group = (i / 3) % 2; // Switch brush every 3 fields
            int brushIndex = group;
            int colorIndex = i % brushColorsData.Count;
            // ----------------------

            GameObject brushInstance = Instantiate(brushPrefabs[brushIndex], modelHolder);
            brushInstance.transform.localPosition = Vector3.zero;
            brushInstance.transform.localRotation = Quaternion.identity;
            brushInstance.transform.localScale = Vector3.one * 30f;

            _instantiatedBrushes.Add(brushInstance);

            Brush brush = brushInstance.GetComponent<Brush>();
            Color baseColor = brushColorsData[colorIndex].m_Colors[0];

            if (brush != null)
            {
                foreach (Renderer renderer in brush.m_Renderers)
                {
                    if (renderer != null)
                        renderer.material.color = baseColor;
                }
            }

            SkinItemButton buttonScript = skinItemInstance.GetComponent<SkinItemButton>();
            if (buttonScript != null)
            {
                buttonScript.Setup(this, brushPrefabs[brushIndex], baseColor);
            }
        }
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

    private void AutoSelectFirstBrush()
    {
        if (brushPrefabs.Count > 0 && brushColorsData.Count > 0)
        {
            UpdateTopBrush(brushPrefabs[0], brushColorsData[0].m_Colors[0]);
        }
    }
}
