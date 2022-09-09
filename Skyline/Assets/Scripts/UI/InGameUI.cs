using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    public GameObject TouchPanel;
    public TextMeshProUGUI TotalCoins_txt, AttemptNo_txt;
    public Image AccuracyBarFill;
    public TimerUI TimerUI;
    public ResultsPanel ResultsPanel;
    public RequiredColorUI RequiredColorUI;
    public DashboardPanel DashboardPanel;
    public bool isPanelOpen = false;
    
    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        InGameUI.instance.SetAccuracyBar(0);
    }

    public void HomeClick()
    {
        Time.timeScale = 0;
        isPanelOpen = true;
        // ConfirmHomePanel.SetActive(true);
    }

    public void ConfirmShowHome(bool _state)
    {
        if (_state)
        {
            Time.timeScale = 1f;
            isPanelOpen = false;
            // ConfirmHomePanel.SetActive(false);
            // LevelManager.instance.MoveOn();
        }
        else
        {
            Time.timeScale = 1f;
            isPanelOpen = false;
            // ConfirmHomePanel.SetActive(false);
        }
    }

    public void TryAgainClicked()
    {
        LevelManager.instance.TryAgain();
    }

    public void MoveOnClicked()
    {
        LevelManager.instance.MoveOn();
    }

    public void OkClicked()
    {
        LevelManager.instance.LevelComplete(LevelManager.instance.CalculatePercentage()>=Data.instance.AccuracyRequired);
    }

    public void SetCoins()
    {
        TotalCoins_txt.SetText(Data.instance.RewardCoins.ToString());
    }

    public void SetAccuracyBar(float _per)
    {
        AccuracyBarFill.fillAmount = _per;
    }

    public void SetAttemptNo(int _attemptNo)
    {
        AttemptNo_txt.SetText(_attemptNo.ToString());
    }

    public void StartTimer()
    {
        LevelManager.instance.hasBegun = true;
        AttemptNo_txt.gameObject.SetActive(true);
        TouchPanel.SetActive(false);
    }
}
