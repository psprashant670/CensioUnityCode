using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;

    public GameObject LoadingPanel, StartPanel, EndScreensPanel, LevelSelectPanel;
    public ErrorPanel ErrorPanel;
    public List<GameObject> AllHomePanels;
    public Data Data;
    public TextMeshProUGUI Coins_txt;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        foreach (GameObject _panel in AllHomePanels)
        {
            _panel.SetActive(false);
        }

        if (GameManager.instance.MenuState == MenuState.Loading)
        {
            LoadingPanel.SetActive(true);
        }
        else if (GameManager.instance.MenuState == MenuState.Start)
        {
            StartPanel.SetActive(true);
        }
        else if (GameManager.instance.MenuState == MenuState.NotEnoughMedallions)
        {
            LevelComplete(false);
            LevelSelectPanel.SetActive(true);
        }
        else
        {
            EndScreensPanel.SetActive(true);
        }
        
        if (!GameManager.isLoaded)
        {
            StartCoroutine(WaitForLoad());
        }
        else
        {
            Coins_txt.SetText(Data.instance.Coins.ToString());
        }
    }

    public void SetCoins()
    {
        Coins_txt.SetText(Data.instance.Coins.ToString());
    }

    public void ShowError(string _msg)
    {
        ErrorPanel.ShowError(_msg);
    }

    public void HomeClick()
    {
        if (GameManager.isLoaded)
        {
            GameManager.instance.MenuState = MenuState.Start;
            GameManager.instance.LoadScene("Menu");
        }
    }

    public void PlayLevel()
    {
        switch (Data.SelectedDifficulty)
        {
            case Difficulty.Level1:
            GameManager.instance.LoadScene("Level1");
            break;
            case Difficulty.Level2:
            GameManager.instance.LoadScene("Level2");
            break;
            case Difficulty.Level3:
            GameManager.instance.LoadScene("Level3");
            break;
            case Difficulty.Level4:
            GameManager.instance.LoadScene("Level4");
            break;
            default:
            break;
        }
    }

    public void TryAgain()
    {
        // Debug.Log(Data.Attempts[(int)Data.SelectedDifficulty]);
        LevelData level = Data.CurrentLevel;
        int attemptNo = Data.instance.Attempts[(int)level.difficulty];
        level.attemptNumber = Mathf.Clamp(attemptNo, 1, level.attemptsAllowed);
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        level.idBehavior = measurement1Data.idBehavior;
        level.behaviorScore = Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore;
        level.coins = 0;

        GameManager.instance.PostUserAttempt();
        
        // set previous level data
        Data.PreviousLevelData.idLevel = level.idLevel;
        Data.PreviousLevelData.isCompleted = level.isComplete;
        Data.PreviousLevelData.idBehavior = level.idBehavior;
        Data.PreviousLevelData.behaviorScore = level.behaviorScore;
        Data.PreviousLevelData.attemptNo = level.attemptNumber;
        Data.PreviousLevelData.medallionsCollected = Data.MedallionCount;

        if (Data.SelectedDifficulty!=Difficulty.None && Data.Attempts[(int)Data.SelectedDifficulty] < Data.MaxAttempts)
        {
            PlayLevel();
        }
    }

    public void MoveOn()
    {
        LevelData level = Data.CurrentLevel;
        int attemptNo = Data.instance.Attempts[(int)level.difficulty];
        level.attemptNumber = Mathf.Clamp(attemptNo, 1, level.attemptsAllowed);
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        level.behaviorScore = measurement1Data.behaviorScore;
        level.idBehavior = measurement1Data.idBehavior;
        level.coins = 0;
        
        GameManager.instance.PostUserAttempt();

        // set previous level data
        Data.PreviousLevelData.idLevel = level.idLevel;
        Data.PreviousLevelData.isCompleted = level.isComplete;
        Data.PreviousLevelData.idBehavior = level.idBehavior;
        Data.PreviousLevelData.behaviorScore = level.behaviorScore;
        Data.PreviousLevelData.attemptNo = level.attemptNumber;
        Data.PreviousLevelData.medallionsCollected = Data.MedallionCount;
    }

    public void LevelComplete(bool _successfully)
    {
        LevelData level = Data.CurrentLevel;
        int attemptNo = Data.instance.Attempts[(int)level.difficulty];
        level.attemptNumber = Mathf.Clamp(attemptNo, 1, level.attemptsAllowed);
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        if (!_successfully && GameManager.instance.MenuState == MenuState.GaveUp)
        {
            measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        }
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        level.idBehavior = GameManager.instance.MenuState!=MenuState.GaveUp ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).idBehavior : measurement1Data.idBehavior;
        level.behaviorScore = GameManager.instance.MenuState!=MenuState.GaveUp ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore : measurement1Data.behaviorScore; // TODO : second part is an issue, returns 1
        level.coins = _successfully ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).goldCoins : 0;
        // Debug.Log("behaviourScore "+level.behaviorScore);
        
        GameManager.instance.PostUserAttempt();

        // set previous level data
        Data.PreviousLevelData.idLevel = level.idLevel;
        Data.PreviousLevelData.isCompleted = level.isComplete;
        Data.PreviousLevelData.idBehavior = level.idBehavior;
        Data.PreviousLevelData.behaviorScore = level.behaviorScore;
        Data.PreviousLevelData.attemptNo = level.attemptNumber;
        Data.PreviousLevelData.medallionsCollected = Data.MedallionCount;

        if (GameManager.instance.MenuState == MenuState.NotEnoughMedallions)
        {
            Data.PreviousLevelData.idLevel++;
        }
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitUntil( ()=> GameManager.isLoaded );
        foreach (GameObject _panel in AllHomePanels)
        {
            _panel.SetActive(false);
        }
        StartPanel.SetActive(true);
        Coins_txt.SetText(Data.instance.Coins.ToString());
    }
}
