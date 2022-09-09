using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    public GameObject WinPanel, TimeOverPanel, TryAgainPanel, PuzzlePanel, ConfirmHomePanel, HUDPanel;
    public TimerUI TimerUI;
    public MapPanel MapPanel;
    public FeedbackUI FeedbackUI;
    public bool isPanelOpen = false;
    
    public TextMeshProUGUI TotalCoins_txt, AttemptNo_txt, Timer_txt;

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
        
    }

    public void HomeClick()
    {
        Time.timeScale = 0;
        isPanelOpen = true;
        ConfirmHomePanel.SetActive(true);
    }

    public void ConfirmShowHome(bool _state)
    {
        if (_state)
        {
            Time.timeScale = 1f;
            isPanelOpen = false;
            LevelManager.instance.HideGrids();
            ConfirmHomePanel.SetActive(false);
            LevelManager.instance.MoveOn();
        }
        else
        {
            Time.timeScale = 1f;
            isPanelOpen = false;
            ConfirmHomePanel.SetActive(false);
        }
    }

    public void SetCoins()
    {
        TotalCoins_txt.SetText(Data.instance.TotalCoins.ToString());
    }

    public void SetTime(float _time)
    {
        _time = Mathf.Clamp(_time, 0, _time);
        int minutes = Mathf.FloorToInt(_time / 60F);
        int seconds = Mathf.FloorToInt(_time - minutes * 60);
        Timer_txt.SetText(string.Format("{0:0}:{1:00}", minutes, seconds));
    }

    public void SetAttemptNo(int _attemptNo)
    {
        AttemptNo_txt.SetText("Attempt : "+_attemptNo+"/"+Data.instance.CurrentLevel.attemptsAllowed);
    }

    public void StartTimer()
    {
        LevelManager.instance.hasBegun = true;
        AttemptNo_txt.gameObject.SetActive(true);
        Timer_txt.gameObject.SetActive(true);
        TimerUI.gameObject.SetActive(true);
    }

    public void ShowPuzzlePanel(bool _state)
    {
        PuzzlePanel.SetActive(_state);
    }

    public void ShowWinPanel()
    {
        WinPanel.SetActive(true);
    }

    public void ShowTimeOverPanel()
    {
        TimeOverPanel.SetActive(true);
    }

    public void ShowMapPanel()
    {
        ShowPuzzlePanel(false);
        MapPanel.ShopMapPanel(true);
        for (int i = 0; i < Data.instance.Levels.Count; i++)
        {
            MapPanel.ShowMap(Data.instance.Levels[i].isComplete==1?(((int)Data.instance.Levels[i].level)+1):0);
        }
        MapPanel.SetMessage(Data.instance.CurrentLevel.bottomCompleteMessage);
        LevelManager.instance.HideGrids();
    }

    public void ShowTryAgainPanel()
    {
        if (LevelManager.instance.GridManager2 != null)
        {
            LevelManager.instance.GridManager2.gameObject.SetActive(false);
        }

        if (Data.instance.CurrentLevel.attemptNumber < Data.instance.CurrentLevel.attemptsAllowed)
        {
            //attempt exist
            TryAgainPanel.SetActive(true);
            Debug.Log("attempt exist");
        }
        else
        {
            Debug.Log("no attempt");
            LevelManager.instance.SetLevelFailed();
            LevelManager.instance.LevelComplete(false);
        }
    }

    public void HideTryAgainPanel()
    {
        TryAgainPanel.SetActive(false);
    }

    public void CheckFeedback()
    {
        LevelManager.instance.LevelComplete(Data.instance.CurrentLevel.isComplete==1);
    }

    public void GameCompleteClick()
    {
        // LevelManager.instance.
    }

    public void TryAgainClick()
    {
        LevelManager.instance.TryAgain();
    }

    public void MoveOnClick()
    {
        LevelManager.instance.MoveOn();
    }
}
