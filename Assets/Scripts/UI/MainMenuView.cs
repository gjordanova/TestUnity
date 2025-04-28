using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View<MainMenuView>
{
    private const string m_BestScorePrefix = "BEST SCORE ";
    [Header("Ranks")]
    public string[] m_Ratings;
    public Text m_BestScoreText;
    public Image m_BestScoreBar;
    public GameObject m_BestScoreObject;
    public InputField m_InputField;
    public List<Image> m_ColoredImages;
    public List<Text> m_ColoredTexts;

    public GameObject m_BrushGroundLight;
    public GameObject m_BrushesPrefab;
    public int m_IdSkin = 0;
    public GameObject m_PointsPerRank;
    public RankingView m_RankingView;

    [Header("Brush Selection UI")]
    public Button m_BrushButton;         // Only the button!
    public GameObject m_SkinSelectionScreen; // The grid screen
    public GameObject m_BrushSelect; // The grid screen

    [Header("Feature Control")]
    public FeatureData m_FeatureData;    // ScriptableObject

    private StatsManager m_StatsManager;

    protected override void Awake()
    {
        base.Awake();

        m_StatsManager = StatsManager.Instance;
        m_IdSkin = m_StatsManager.FavoriteSkin;

        if (m_FeatureData != null)
        {
            bool skinSelectionEnabled = m_FeatureData.SkinSelectionScreen;

            if (m_BrushButton != null)
                m_BrushButton.gameObject.SetActive(skinSelectionEnabled);

            if (m_BrushSelect != null)
                m_BrushSelect.SetActive(!skinSelectionEnabled);
        }
    }

    private void Start()
    {
        if (m_BrushButton != null)
            m_BrushButton.onClick.AddListener(OnBrushButtonPressed);

        if (m_SkinSelectionScreen != null)
            m_SkinSelectionScreen.SetActive(false);

        if (m_FeatureData != null)
        {
            bool skinSelectionEnabled = m_FeatureData.SkinSelectionScreen;
            if (m_BrushButton != null)
                m_BrushButton.gameObject.SetActive(skinSelectionEnabled);
        }
    }
    public void OnBrushButtonPressed()
    {
        Transition(false); // Hide MainMenuView
        GameManager.Instance.ChangePhase(GamePhase.SKINSELECTION);
        m_SkinSelectionScreen.SetActive(true);
    }

    public void OnPlayButton()
    {
        if (m_GameManager.currentPhase == GamePhase.MAIN_MENU)
            m_GameManager.ChangePhase(GamePhase.LOADING);
    }

    protected override void OnGamePhaseChanged(GamePhase _GamePhase)
    {
        base.OnGamePhaseChanged(_GamePhase);

        switch (_GamePhase)
        {
            case GamePhase.MAIN_MENU:
                m_BrushGroundLight.SetActive(true);
                m_BrushesPrefab.SetActive(true);
                Transition(true);
                m_SkinSelectionScreen.SetActive(false);
                break;

            case GamePhase.LOADING:
                m_BrushGroundLight.SetActive(false);
                m_BrushesPrefab.SetActive(false);
                if (m_Visible)
                    Transition(false);
                break;

            case GamePhase.SKINSELECTION:
                m_BrushGroundLight.SetActive(false);
                m_BrushesPrefab.SetActive(false);
                if (m_Visible)
                    Transition(true);
                break;

            case GamePhase.DAILYREWARD:
                //m_BrushGroundLight.SetActive(false);
                m_BrushesPrefab.SetActive(false);
                //Transition(false);  // Always transition to false, regardless of current visibility
                break;
        }
    }

    public void SetTitleColor(Color _Color)
    {
        if (m_BrushesPrefab != null)
        {
            m_BrushesPrefab.SetActive(true);
            int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);
            m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(m_GameManager.m_Skins[favoriteSkin]);
        }

        string playerName = m_StatsManager.GetNickname();
        if (!string.IsNullOrEmpty(playerName))
            m_InputField.text = playerName;

        foreach (var img in m_ColoredImages)
            img.color = _Color;

        foreach (var txt in m_ColoredTexts)
            txt.color = _Color;

        m_RankingView.gameObject.SetActive(true);
        m_RankingView.RefreshNormal();
        m_BrushButton.GetComponent<Image>().color = _Color;
    }

    public void OnSetPlayerName(string _Name)
    {
        m_StatsManager.SetNickname(_Name);
    }

    public string GetRanking(int _Rank)
    {
        return m_Ratings[_Rank];
    }

    public int GetRankingCount()
    {
        return m_Ratings.Length;
    }

    public void LeftButtonBrush()
    {
        ChangeBrush(m_IdSkin - 1);
    }

    public void RightButtonBrush()
    {
        ChangeBrush(m_IdSkin + 1);
    }

    public void ChangeBrush(int _NewBrush)
    {
        _NewBrush = Mathf.Clamp(_NewBrush, 0, GameManager.Instance.m_Skins.Count);
        m_IdSkin = _NewBrush;

        if (m_IdSkin >= GameManager.Instance.m_Skins.Count)
            m_IdSkin = 0;

        GameManager.Instance.m_PlayerSkinID = m_IdSkin;

        int favoriteSkin = Mathf.Min(m_StatsManager.FavoriteSkin, m_GameManager.m_Skins.Count - 1);

        if (m_BrushesPrefab != null)
            m_BrushesPrefab.GetComponent<BrushMainMenu>().Set(GameManager.Instance.m_Skins[favoriteSkin]);

        m_StatsManager.FavoriteSkin = m_IdSkin;

        GameManager.Instance.SetColor(GameManager.Instance.ComputeCurrentPlayerColor(true, 0));
    }
}