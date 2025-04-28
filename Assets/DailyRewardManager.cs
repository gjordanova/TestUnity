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
    public Color highlightColor = Color.yellow; 
    public Color normalColor = Color.gray;

    [Header("Feature Control")]
    public FeatureData featureData;

    [Header("Animation Settings")]
    public float coinAnimDuration = 1.2f;      
    public Transform coinsVisualPrefab;
    public Transform coinsTarget;
    public int numberOfCoins = 5;

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
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(ClaimReward);
        
        CheckDailyReward();
    }

    public void CheckDailyReward()
    {
        DateTime today = DateTime.Today;

        if (PlayerPrefs.HasKey(LastClaimDateKey))
        {
            DateTime lastClaimDate = DateTime.Parse(PlayerPrefs.GetString(LastClaimDateKey));

            if (lastClaimDate == today)
            {
                Debug.Log("test 1");
                dailyRewardPanel.SetActive(false);
                GameManager.Instance.ChangePhase(GamePhase.MAIN_MENU);
                claimButton.interactable = false; 
                return;
            }
            else if (lastClaimDate == today.AddDays(-1))
            {
                Debug.Log("test 2");
                currentDayIndex = PlayerPrefs.GetInt(CurrentStreakKey, 0);
                currentDayIndex++;
                if (currentDayIndex >= dailyRewards.Length)
                    currentDayIndex = dailyRewards.Length - 1;

            }
            else
            {
                Debug.Log("test 3");
                currentDayIndex = 0;
            }
        }
        else
        {
            GameManager.Instance.ChangePhase(GamePhase.DAILYREWARD);
            Debug.Log("test 4");
            currentDayIndex = 0;
        }

        UpdateRewardList();
        ShowRewardPopup();
    }

    private void UpdateRewardList()
    {
        if (dayTexts == null || amountTexts == null || dayTexts.Length == 0 || amountTexts.Length == 0)
        {
            Debug.LogError("Daily reward text arrays are not properly initialized!");
            return;
        }

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
        claimButton.interactable = true; 
    }

    private IEnumerator ClaimRewardRoutine()
    {
        claimButton.interactable = false;
        int rewardAmount = dailyRewards[currentDayIndex];
        Vector3 buttonPosition = claimButton.transform.position;
        
       
        for (int i = 0; i < numberOfCoins; i++)
        {
            Transform coinsVisual = Instantiate(coinsVisualPrefab, buttonPosition, Quaternion.identity, transform);
            
          
            float randomX = UnityEngine.Random.Range(-20f, 20f);
            float randomY = UnityEngine.Random.Range(-10f, 10f);
            Vector3 randomOffset = new Vector3(randomX, randomY, 0);
            
        
            float startDelay = i * 0.15f;
            
            Sequence coinSequence = DOTween.Sequence();
            
           
            coinSequence.Append(coinsVisual.DOMove(buttonPosition + randomOffset, 0.2f)
                .SetEase(Ease.OutQuad)
                .SetDelay(startDelay));
            
          
            coinSequence.Append(coinsVisual.DOMove(coinsTarget.position, coinAnimDuration)
                .SetEase(Ease.InSine));
            
         
            coinSequence.Join(coinsVisual.DOScale(0.3f, coinAnimDuration * 0.3f)
                .SetDelay(coinAnimDuration * 0.7f));
            
         
            coinSequence.OnComplete(() => Destroy(coinsVisual.gameObject));
        }
        
        yield return new WaitForSeconds(coinAnimDuration + 1f);
        
      
        int currentCurrency = PlayerPrefs.GetInt(TotalCurrencyKey, 0);
        currentCurrency += rewardAmount;
        PlayerPrefs.SetInt(TotalCurrencyKey, currentCurrency);

     
        if (currencyText != null)
        {
            currencyText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
        
        UpdateCurrencyText();
        PlayerPrefs.SetString(LastClaimDateKey, DateTime.Today.ToString());
        PlayerPrefs.SetInt(CurrentStreakKey, currentDayIndex + 1);
        PlayerPrefs.Save();
        
        Debug.Log($"Claimed {rewardAmount} coins!");
        
    
        yield return new WaitForSeconds(0.7f);
        
        dailyRewardPanel.SetActive(false);
        GameManager.Instance.ChangePhase(GamePhase.MAIN_MENU);
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