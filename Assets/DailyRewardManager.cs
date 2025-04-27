using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DailyRewardManager : MonoBehaviour
{
    [Header("Reward Settings")]
    public int[] dailyRewards = { 100, 100, 150, 150, 200, 300, 500 };

    [Header("UI References")]
    public GameObject dailyRewardPanel;
    public Button claimButton;
    public Transform rewardListParent;
    public TextMeshProUGUI currencyText; // 🔥 NEW: Display total coins
    public Color highlightColor = Color.white;
    public Color normalColor = Color.gray;

    [Header("Feature Control")]
    public FeatureData featureData; 

    private Text[] dayTexts;
    private Text[] amountTexts;

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

        dayTexts = new Text[rewardListParent.childCount];
        amountTexts = new Text[rewardListParent.childCount];

        for (int i = 0; i < rewardListParent.childCount; i++)
        {
            Transform dayItem = rewardListParent.GetChild(i);
            dayTexts[i] = dayItem.Find("Day").GetComponent<Text>();
            amountTexts[i] = dayItem.Find("Amount").GetComponent<Text>();
        }

        UpdateCurrencyText(); // 🔥 show current coins immediately
        CheckDailyReward();
        claimButton.onClick.AddListener(ClaimReward);
    }

    private void CheckDailyReward()
    {
        if (!PlayerPrefs.HasKey(LastClaimDateKey))
        {
            currentDayIndex = 0;
            UpdateRewardList();
            ShowRewardPopup();
            return;
        }

        DateTime lastClaimDate = DateTime.Parse(PlayerPrefs.GetString(LastClaimDateKey));
        DateTime today = DateTime.Today;

        if (lastClaimDate == today)
        {
            dailyRewardPanel.SetActive(false); 
        }
        else if (lastClaimDate == today.AddDays(-1))
        {
            currentDayIndex = PlayerPrefs.GetInt(CurrentStreakKey, 0) + 1;
            if (currentDayIndex >= dailyRewards.Length)
                currentDayIndex = dailyRewards.Length - 1;
            UpdateRewardList();
            ShowRewardPopup();
        }
        else
        {
            currentDayIndex = 0;
            UpdateRewardList();
            ShowRewardPopup();
        }
    }

    private void UpdateRewardList()
    {
        for (int i = 0; i < dayTexts.Length; i++)
        {
            dayTexts[i].text = $"DAY {i + 1}";
            amountTexts[i].text = dailyRewards[i].ToString();

            bool isToday = (i == currentDayIndex);
            dayTexts[i].color = isToday ? highlightColor : normalColor;
            amountTexts[i].color = isToday ? highlightColor : normalColor;
        }
    }

    private void ShowRewardPopup()
    {
        dailyRewardPanel.SetActive(true);
    }

    private void ClaimReward()
    {
        int rewardAmount = dailyRewards[currentDayIndex];

        int currentCurrency = PlayerPrefs.GetInt(TotalCurrencyKey, 0);
        currentCurrency += rewardAmount;
        PlayerPrefs.SetInt(TotalCurrencyKey, currentCurrency);

        PlayerPrefs.SetString(LastClaimDateKey, DateTime.Today.ToString());
        PlayerPrefs.SetInt(CurrentStreakKey, currentDayIndex);

        PlayerPrefs.Save();

        UpdateCurrencyText(); // 🔥 Update displayed coins
        Debug.Log($"Claimed {rewardAmount} coins!");

        dailyRewardPanel.SetActive(false);
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
