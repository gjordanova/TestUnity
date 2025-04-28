using UnityEngine;
using UnityEngine.UI;

public class DebugMenuController : MonoBehaviour
{
    [Header("Debug Menu UI Elements")]
    public GameObject debugMenuPanel;
    public Toggle playerCollisionToggle;
    public Toggle skinSelectionToggle;
    public Toggle dailyRewardsToggle;
    public Toggle customFeatureToggle;
    public Button closeButton;

    [Header("References")]
    public MainMenuView mainMenuView;
    public SkinSelectorGrid skinSelector;
    public DailyRewardManager dailyRewardManager;

    [Header("Tap Detection Settings")]
    public float tapTimeThreshold = 1f;
    public float tapAreaSize = 100f;

    private FeatureController featureController;
    private int tapCount = 0;
    private float lastTapTime;

    private void Start()
    {
        // Get reference to FeatureController
        featureController = FindObjectOfType<FeatureController>();
        
        if (featureController == null)
        {
            Debug.LogError("FeatureController not found in the scene!");
            return;
        }

        // Initialize toggles with current feature states
        InitializeToggles();

        // Add listeners to toggle events
        playerCollisionToggle.onValueChanged.AddListener(OnPlayerCollisionToggled);
        skinSelectionToggle.onValueChanged.AddListener(OnSkinSelectionToggled);
        dailyRewardsToggle.onValueChanged.AddListener(OnDailyRewardsToggled);
        customFeatureToggle.onValueChanged.AddListener(OnCustomFeatureToggled);
        closeButton.onClick.AddListener(CloseDebugMenu);

        // Hide debug menu by default
        debugMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 tapPosition = Input.mousePosition;
            
            if (IsInUpperRightCorner(tapPosition))
            {
                float currentTime = Time.time;

                if (currentTime - lastTapTime < tapTimeThreshold)
                {
                    tapCount++;
                    
                    if (tapCount == 3)
                    {
                        OpenDebugMenu();
                        tapCount = 0;
                    }
                }
                else
                {
                    tapCount = 1;
                }

                lastTapTime = currentTime;
            }
        }
    }

    private bool IsInUpperRightCorner(Vector2 position)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        return position.x > screenWidth - tapAreaSize &&
               position.y > screenHeight - tapAreaSize;
    }

    private void InitializeToggles()
    {
        if (featureController.featureData != null)
        {
            playerCollisionToggle.isOn = featureController.featureData.PlayerCollision;
            skinSelectionToggle.isOn = featureController.featureData.SkinSelectionScreen;
            dailyRewardsToggle.isOn = featureController.featureData.DailyRewards;
            customFeatureToggle.isOn = featureController.featureData.CustomFeature;
        }
    }

    private void OnPlayerCollisionToggled(bool enabled)
    {
        if (featureController.featureData != null)
        {
            featureController.featureData.PlayerCollision = enabled;
            
            // Force game state update if in game
            if (GameManager.Instance.currentPhase == GamePhase.GAME)
            {
                GameManager.Instance.ClearGame();
                GameManager.Instance.ChangePhase(GamePhase.LOADING);
            }
        }
    }

    private void OnSkinSelectionToggled(bool enabled)
    {
        if (featureController.featureData != null)
        {
            featureController.featureData.SkinSelectionScreen = enabled;
        
            // Update MainMenuView UI elements
            if (mainMenuView != null)
            {
                if (mainMenuView.m_BrushButton != null)
                    mainMenuView.m_BrushButton.gameObject.SetActive(enabled);
            
                if (mainMenuView.m_BrushSelect != null)
                    mainMenuView.m_BrushSelect.SetActive(!enabled);
            
                if (mainMenuView.m_SkinSelectionScreen != null)
                    mainMenuView.m_SkinSelectionScreen.SetActive(false);
            }

            // Update SkinSelector if it exists
            if (skinSelector != null)
            {
                // Since we can't call PopulateGrid directly, we'll disable and enable the object
                // which will trigger its Start() method that includes PopulateGrid
                skinSelector.gameObject.SetActive(false);
                if (enabled)
                {
                    skinSelector.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnDailyRewardsToggled(bool enabled)
    {
        if (featureController.featureData != null)
        {
            featureController.featureData.DailyRewards = enabled;
        
            if (dailyRewardManager != null)
            {
                dailyRewardManager.gameObject.SetActive(enabled);
                if (enabled)
                {
                    // Force check daily rewards immediately
                    dailyRewardManager.CheckDailyReward();
                    GameManager.Instance.ChangePhase(GamePhase.DAILYREWARD);
                }
                else if (GameManager.Instance.currentPhase == GamePhase.DAILYREWARD)
                {
                    GameManager.Instance.ChangePhase(GamePhase.MAIN_MENU);
                }
            }
        }
    }

    private void OnCustomFeatureToggled(bool enabled)
    {
        if (featureController.featureData != null)
        {
            featureController.featureData.CustomFeature = enabled;
            // Add custom feature implementation here
        }
    }

    public void OpenDebugMenu()
    {
        debugMenuPanel.SetActive(true);
        InitializeToggles(); // Refresh toggle states when opening
    }

    public void CloseDebugMenu()
    {
        debugMenuPanel.SetActive(false);
    }

    public void ResetAllFeatures()
    {
        if (featureController.featureData != null)
        {
            // Disable all features
            featureController.featureData.PlayerCollision = false;
            featureController.featureData.SkinSelectionScreen = false;
            featureController.featureData.DailyRewards = false;
            featureController.featureData.CustomFeature = false;

            // Update UI
            InitializeToggles();

            // Trigger all feature updates
            OnPlayerCollisionToggled(false);
            OnSkinSelectionToggled(false);
            OnDailyRewardsToggled(false);
            OnCustomFeatureToggled(false);

            // If in game, restart to apply changes
            if (GameManager.Instance.currentPhase == GamePhase.GAME)
            {
                GameManager.Instance.ClearGame();
                GameManager.Instance.ChangePhase(GamePhase.LOADING);
            }
        }
    }
}