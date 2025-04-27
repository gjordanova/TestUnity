using UnityEngine;
using UnityEngine.UI;

public class SkinItemButton : MonoBehaviour
{
    private SkinSelectorGrid skinSelectorGrid;
    private GameObject brushPrefab;
    private Color brushColor;

    public void Setup(SkinSelectorGrid gridManager, GameObject prefab, Color color)
    {
        skinSelectorGrid = gridManager;
        brushPrefab = prefab;
        brushColor = color;

        // Add click event to Button
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (skinSelectorGrid != null)
        {
            skinSelectorGrid.UpdateTopBrush(brushPrefab, brushColor);
        }
    }
}