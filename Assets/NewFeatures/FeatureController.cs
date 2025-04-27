using UnityEngine;

public class FeatureController : MonoBehaviour
{
    public FeatureData featureData; // Drag and drop your FeatureSettings asset here

    private void Start()
    {
        if (featureData == null)
        {
            Debug.LogError("FeatureData not assigned!");
        }
    }

    public bool IsCollisionEnabled()
    {
        return featureData != null && featureData.PlayerCollision;
    }

    public bool IsSkinSelectionEnabled()
    {
        return featureData != null && featureData.SkinSelectionScreen;
    }

    public bool IsDailyRewardsEnabled()
    {
        return featureData != null && featureData.DailyRewards;
    }

    public bool IsCustomFeatureEnabled()
    {
        return featureData != null && featureData.CustomFeature;
    }
}