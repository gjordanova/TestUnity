using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class SkinItemButton : MonoBehaviour

{
    private SkinSelectorGrid skinSelectorGrid;
    private GameObject brushPrefab;
    private Color brushColor;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }

    public void Setup(SkinSelectorGrid gridManager, GameObject prefab, Color color)
    {
        skinSelectorGrid = gridManager;
        brushPrefab = prefab;
        brushColor = color;
    }

    private void OnClick()
    {
        if (skinSelectorGrid != null)
        {
            // Update the visual preview
            skinSelectorGrid.UpdateTopBrush(brushPrefab, brushColor);
            
            // Save the selection in GameManager
            GameManager.Instance.SetSelectedSkin(brushPrefab, brushColor);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }
}