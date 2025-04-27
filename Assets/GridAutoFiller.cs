using UnityEngine;

public class GridAutoFiller : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject placeholderPrefab;  // Placeholder object
    public Transform gridParent;           // TheGrid parent

    private void Start()
    {
        if (placeholderPrefab == null || gridParent == null)
        {
            Debug.LogWarning("GridAutoFiller is missing references!");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is missing!");
            return;
        }

        FillGrid();
    }

    private void FillGrid()
    {
        int skinCount = GameManager.Instance.m_Skins.Count;

        for (int i = 0; i < skinCount; i++)
        {
            Instantiate(placeholderPrefab, gridParent);
        }
    }
}