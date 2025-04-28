using UnityEngine;

public class FeatureController : MonoBehaviour
{
    public FeatureData featureData;

    // Add delegates for feature state changes
    public delegate void FeatureStateChanged(bool enabled);
    public event FeatureStateChanged OnPlayerCollisionChanged;
    public event FeatureStateChanged OnSkinSelectionChanged;
    public event FeatureStateChanged OnDailyRewardsChanged;
    public event FeatureStateChanged OnCustomFeatureChanged;

    private void Start()
    {
        if (featureData == null)
        {
            Debug.LogError("FeatureData not assigned!");
        }
    }

    // Add methods to change feature states that will trigger events
    public void SetPlayerCollision(bool enabled)
    {
        if (featureData != null)
        {
            featureData.PlayerCollision = enabled;
            OnPlayerCollisionChanged?.Invoke(enabled);
        }
    }

    public void SetSkinSelection(bool enabled)
    {
        if (featureData != null)
        {
            featureData.SkinSelectionScreen = enabled;
            OnSkinSelectionChanged?.Invoke(enabled);
        }
    }

    public void SetDailyRewards(bool enabled)
    {
        if (featureData != null)
        {
            featureData.DailyRewards = enabled;
            OnDailyRewardsChanged?.Invoke(enabled);
        }
    }

    public void SetCustomFeature(bool enabled)
    {
        if (featureData != null)
        {
            featureData.CustomFeature = enabled;
            OnCustomFeatureChanged?.Invoke(enabled);
        }
    }

    // Keep the existing Is*Enabled methods
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