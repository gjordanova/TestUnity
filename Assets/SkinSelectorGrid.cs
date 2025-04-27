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
    public GamePhase currentPhase { get; private set; }
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
        switch (_GamePhase)
        {
            case GamePhase.SKINSELECTION:
                Transition(true);
                break;
            // case GamePhase.MAIN_MENU:
            //     Transition(true);
            //     break;
        }
    }

    public void OnBackButtonPressed()
    {
        //ClearGrid(); // First destroy all instantiated brushes
        Transition(false); // Fade out the SkinSelectorGrid
        GameManager.Instance.ChangePhase(GamePhase.MAIN_MENU); // Return to MainMenu
    }

    private void PopulateGrid()
    {
        if (brushPrefabs.Count == 0 || brushColorsData.Count == 0)
        {
            Debug.LogError("Brush prefabs or ColorData not assigned properly!");
            return;
        }

        int totalItems = brushPrefabs.Count * brushColorsData.Count; // Dynamic

        for (int i = 0; i < totalItems; i++)
        {
            GameObject skinItemInstance = Instantiate(skinItemPrefab, gridParent);

            Transform modelHolder = skinItemInstance.transform.Find("ModelHolder");
            if (modelHolder == null)
            {
                Debug.LogError("ModelHolder missing inside SkinItemPrefab!");
                continue;
            }

            int brushIndex = i / brushColorsData.Count;  // 0–1
            int colorIndex = i % brushColorsData.Count;  // 0–5

            GameObject brushInstance = Instantiate(brushPrefabs[brushIndex], modelHolder);
            brushInstance.transform.localPosition = Vector3.zero;
            brushInstance.transform.localRotation = Quaternion.identity;
            brushInstance.transform.localScale = Vector3.one * 30f;

            _instantiatedBrushes.Add(brushInstance); // Keep track of brush instances

            Renderer rend = brushInstance.GetComponentInChildren<Renderer>();
            if (rend != null && brushColorsData[colorIndex] != null)
            {
                rend.material.color = brushColorsData[colorIndex].m_Colors[0];
            }

            SkinItemButton buttonScript = skinItemInstance.GetComponent<SkinItemButton>();
            if (buttonScript != null)
            {
                buttonScript.Setup(this, brushPrefabs[brushIndex], brushColorsData[colorIndex].m_Colors[0]);
            }
        }
    }

    private void ClearGrid()
    {
        // 1. Disable all brushes inside the grid
        foreach (Transform child in gridParent)
        {
            child.gameObject.SetActive(false);
        }

        // 2. Disable the top preview brush
        if (currentTopBrush != null)
        {
            currentTopBrush.SetActive(false);
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
        currentTopBrush.transform.localScale = Vector3.one * 50f; // Bigger size

        Renderer rend = currentTopBrush.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material.color = brushColor;
        }
    }

    private void AutoSelectFirstBrush()
    {
        if (brushPrefabs.Count > 0 && brushColorsData.Count > 0)
        {
            UpdateTopBrush(brushPrefabs[0], brushColorsData[0].m_Colors[0]);
        }
    }
}
