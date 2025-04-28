using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;

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
        claimButton.interactable = false; // Prevent multiple clicks
        
        int rewardAmount = dailyRewards[currentDayIndex];

        int currentCurrency = PlayerPrefs.GetInt(TotalCurrencyKey, 0);
        currentCurrency += rewardAmount;
        PlayerPrefs.SetInt(TotalCurrencyKey, currentCurrency);

        PlayerPrefs.SetString(LastClaimDateKey, DateTime.Today.ToString());
        PlayerPrefs.SetInt(CurrentStreakKey, currentDayIndex + 1);

        PlayerPrefs.Save();
        Debug.Log($"Claimed {rewardAmount} coins!");

        UpdateCurrencyText();
        
        // Wait for 1 second
        yield return new WaitForSeconds(1f);
        
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