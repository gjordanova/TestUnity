using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkinSelectorGrid : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject skinItemPrefab;          // Prefab of the grid item
    public Transform gridParent;               // Where to instantiate (TheGrid)
    public Transform topPreviewParent;         // Where the big brush is shown (Top Preview area)
    public List<GameObject> brushPrefabs;       // Two brush prefabs (Brush + Roller)
    public List<ColorData> brushColorsData;     // 6 colors from scriptable objects

    [Header("Feature Control")]
    public FeatureData featureData;

    private GameObject currentTopBrush;

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

    private void PopulateGrid()
    {
        if (brushPrefabs.Count < 2 || brushColorsData.Count < 6)
        {
            Debug.LogError("Brush prefabs or ColorData not assigned properly!");
            return;
        }

        for (int i = 0; i < 12; i++)
        {
            GameObject skinItemInstance = Instantiate(skinItemPrefab, gridParent);

            Transform modelHolder = skinItemInstance.transform.Find("ModelHolder");

            if (modelHolder == null)
            {
                Debug.LogError("ModelHolder missing inside SkinItemPrefab!");
                return;
            }

            int brushIndex = (i < 6) ? 0 : 1;
            int colorIndex = i % 6;

            GameObject brushInstance = Instantiate(brushPrefabs[brushIndex], modelHolder);
            brushInstance.transform.localPosition = Vector3.zero;
            brushInstance.transform.localRotation = Quaternion.identity;
            brushInstance.transform.localScale = Vector3.one * 30f;

            Renderer rend = brushInstance.GetComponentInChildren<Renderer>();
            if (rend != null && brushColorsData[colorIndex] != null)
            {
                rend.material.color = brushColorsData[colorIndex].m_Colors[0];
            }

            // Setup button click
            SkinItemButton buttonScript = skinItemInstance.GetComponent<SkinItemButton>();
            if (buttonScript != null)
            {
                buttonScript.Setup(this, brushPrefabs[brushIndex], brushColorsData[colorIndex].m_Colors[0]);
            }
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
