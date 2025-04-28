using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;
using DG.Tweening;

public class DailyRewardManager : MonoBehaviour
{
    [Header("Reward Settings")]
    public int[] dailyRewards = { 100, 100, 150, 150, 200, 300, 500 };

    [Header("UI References")]
    public GameObject dailyRewardPanel;
    public Button claimButton;
    public Transform rewardListParent;
    public TextMeshProUGUI currencyText;
    public Color highlightColor = Color.yellow; // Changed to yellow for better visibility
    public Color normalColor = Color.gray;

    [Header("Feature Control")]
    public FeatureData featureData;

    [Header("Animation Settings")]
    public float coinAnimDuration = 0.7f;
    public float coinJumpPower = 20f;
    public Transform coinsVisualPrefab;  // Assign a coin image prefab in inspector
    public Transform coinsTarget;         // Where coins should fly to (usually currency UI)

    private TextMeshProUGUI[] dayTexts;
    private TextMeshProUGUI[] amountTexts;

    private int currentDayIndex = 0;
    private const string LastClaimDateKey = "LastClaimDate";
    private const string CurrentStreakKey = "CurrentStreak";
    private const string TotalCurrencyKey = "TotalCurrency";

    private void Start()
    {
        if (featureData == null)
        {
            Debug.LogError("FeatureData not assigned!");
            return;
        }

        if (!featureData.DailyRewards)
        {
            dailyRewardPanel.SetActive(false);
            return;
        }

        dayTexts = new TextMeshProUGUI[rewardListParent.childCount];
        amountTexts = new TextMeshProUGUI[rewardListParent.childCount];

        for (int i = 0; i < rewardListParent.childCount; i++)
        {
            Transform dayItem = rewardListParent.GetChild(i);
            dayTexts[i] = dayItem.Find("Day").GetComponent<TextMeshProUGUI>();
            amountTexts[i] = dayItem.Find("Amount").GetComponent<TextMeshProUGUI>();
        }

        UpdateCurrencyText();
        claimButton.onClick.RemoveAllListeners(); // ➡️ осигурај дека нема удвоено слушачи
        claimButton.onClick.AddListener(ClaimReward);

        CheckDailyReward();
    }

    private void CheckDailyReward()
    {
        DateTime today = DateTime.Today;

        if (PlayerPrefs.HasKey(LastClaimDateKey))
        {
            DateTime lastClaimDate = DateTime.Parse(PlayerPrefs.GetString(LastClaimDateKey));

            if (lastClaimDate == today)
            {
                dailyRewardPanel.SetActive(false);
                claimButton.interactable = false; // ➡️ не дозволуваш клик
                return;
            }
            else if (lastClaimDate == today.AddDays(-1))
            {
                currentDayIndex = PlayerPrefs.GetInt(CurrentStreakKey, 0);
                currentDayIndex++;
                if (currentDayIndex >= dailyRewards.Length)
                    currentDayIndex = dailyRewards.Length - 1;
            }
            else
            {
                currentDayIndex = 0;
            }
        }
        else
        {
            currentDayIndex = 0;
        }

        UpdateRewardList();
        ShowRewardPopup();
    }

    private void UpdateRewardList()
    {
        int totalDays = dayTexts.Length;

        for (int i = 0; i < totalDays; i++)
        {
            int reversedIndex = totalDays - 1 - i;
            dayTexts[i].text = $"DAY {reversedIndex + 1}";
            amountTexts[i].text = $"{dailyRewards[reversedIndex]} 🪙";

            bool isToday = (reversedIndex == currentDayIndex);
            dayTexts[i].color = isToday ? highlightColor : normalColor;
            amountTexts[i].color = isToday ? highlightColor : normalColor;
        }
    }

    private void ShowRewardPopup()
    {
        dailyRewardPanel.SetActive(true);
        claimButton.interactable = true; // ➡️ овозможи го копчето кога има право да земе
    }

    private IEnumerator ClaimRewardRoutine()
    {
        claimButton.interactable = false;
        
        int rewardAmount = dailyRewards[currentDayIndex];

        // Spawn and animate coins
        Transform rewardDay = rewardListParent.GetChild(rewardListParent.childCount - 1 - currentDayIndex);
        Transform coinsVisual = Instantiate(coinsVisualPrefab, rewardDay.position, Quaternion.identity, transform);
        
        // First jump up
        coinsVisual.DOJump(coinsVisual.position + Vector3.up * 1f, coinJumpPower, 1, 0.3f);
        
        // Wait for jump
        yield return new WaitForSeconds(1.0f);
        
        // Then fly to target with scale animation
        Sequence coinSequence = DOTween.Sequence();
        coinSequence.Append(coinsVisual.DOMove(coinsTarget.position, coinAnimDuration).SetEase(Ease.InBack));
        coinSequence.Join(coinsVisual.DOScale(0.5f, coinAnimDuration));
        
        yield return coinSequence.WaitForCompletion();
        
        // Destroy coin visual
        Destroy(coinsVisual.gameObject);
        
        // Update actual currency
        int currentCurrency = PlayerPrefs.GetInt(TotalCurrencyKey, 0);
        currentCurrency += rewardAmount;
        PlayerPrefs.SetInt(TotalCurrencyKey, currentCurrency);

        // Animate currency text
        if (currencyText != null)
        {
            currencyText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
        
        UpdateCurrencyText();
        PlayerPrefs.SetString(LastClaimDateKey, DateTime.Today.ToString());
        PlayerPrefs.SetInt(CurrentStreakKey, currentDayIndex + 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Claimed {rewardAmount} coins!");
        
        // Wait a moment after animation
        yield return new WaitForSeconds(0.7f);
        
        dailyRewardPanel.SetActive(false);
    }

    private void ClaimReward()
    {
        StartCoroutine(ClaimRewardRoutine());
    }

    private void UpdateCurrencyText()
    {
        if (currencyText != null)
        {
            int currentCurrency = PlayerPrefs.GetInt(TotalCurrencyKey, 0);
            currencyText.text = currentCurrency.ToString();
        }
    }
}