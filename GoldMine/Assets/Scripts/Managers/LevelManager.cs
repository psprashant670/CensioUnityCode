using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Level CurrentLevel;
    public GridManager GridManager;
    public GridManager2 GridManager2;

    public bool hasBegun { get; set; }
    [SerializeField]
    float TimeLeft = 0f, MaxTime;
    [HideInInspector]
    public int AttemptNo, Coins;
    Data Data;

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
        hasBegun = false;
        Data = Data.instance;

        LoadLevelData();

        InGameUI.instance.TimerUI.SetFill(TimeLeft/MaxTime);
        InGameUI.instance.SetAttemptNo(AttemptNo);
        InGameUI.instance.SetTime(TimeLeft);
        InGameUI.instance.SetCoins();

        InGameUI.instance.AttemptNo_txt.gameObject.SetActive(false);
        InGameUI.instance.Timer_txt.gameObject.SetActive(false);
        InGameUI.instance.TimerUI.gameObject.SetActive(false);
    }

    void FixedUpdate() 
    {
        if (hasBegun)
        {
            UpdateTimer();
        }
    }

    public void LoadLevelData()
    {
        
        #if UNITY_EDITOR
        Data.SelectedLevel = CurrentLevel;
        #endif

        TimeLeft = Data.Timer;
        MaxTime = Data.Timer; 
        Data.CurrentLevel.attemptNumber++;
        AttemptNo = Data.CurrentLevel.attemptNumber;
        Data.CurrentLevel.correctMoves = 0;
        Data.CurrentLevel.noOfMoves = 0;
        Data.CurrentLevel.rewardCoins = 0;
        Data.CurrentLevel.coins = 0;
    }

    public void SetLevelCompleted()
    {
        Data.CurrentLevel.isComplete = 1;
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Debug.Log(Data.CurrentLevel.timeTaken);
        AttemptData attemptData = Data.GetGameAttemptDataValue(Data.CurrentLevel.idLevel, Data.CurrentLevel.attemptNumber);
        if (Data.CurrentLevel.isComplete == 1)
        {
            int goldCoins = attemptData.goldCoins;
            Data.Coins += goldCoins;
            Data.CurrentLevel.rewardCoins = Data.CurrentLevel.timeTaken < attemptData.challengeCompletedTime2 ? attemptData.rewardCoinsTime2 : 0;
            if (Data.CurrentLevel.timeTaken < attemptData.challengeCompletedTime1)
            {
                Data.CurrentLevel.rewardCoins = attemptData.rewardCoinsTime1;
            }
            Data.RewardCoins += Data.CurrentLevel.rewardCoins;
        }

        Data.TotalCoins = Data.Coins+Data.RewardCoins;
        InGameUI.instance?.SetCoins();
    }

    public void SetLevelFailed()
    {
        Data.CurrentLevel.isComplete = -1;
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
    }

    public void TryAgain()
    {
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Data.CurrentLevel.isComplete = 0;

        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        // level.idBehavior = measurement1Data.idBehavior;
        level.idBehavior = 0;
        level.behaviorScore = Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore;
        level.coins = 0;

        GameManager.instance.PostUserAttempt();
        if (Data.SelectedLevel!=Level.None && level.attemptNumber < level.attemptsAllowed)
        {
            GameManager.instance.ReloadScene();
        }
    }

    public void MoveOn()
    {
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Data.CurrentLevel.isComplete = -1;

        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        level.behaviorScore = measurement1Data.behaviorScore;
        // level.idBehavior = measurement1Data.idBehavior;
        level.idBehavior = 0;
        level.coins = 0;
        
        if (LevelManager.instance.CurrentLevel == Level.Level1 || LevelManager.instance.CurrentLevel == Level.Level3)
        {
            GameManager.instance.PostUserAttempt();
            GameManager.instance.MenuState = MenuState.LevelSelect;
            GameManager.instance.LoadScene("Menu");
        }
        else
        {
            GameManager.instance.PostUserAttempt();
            GameManager.instance.MenuState = MenuState.LevelSelect;
            InGameUI.instance.FeedbackUI.UncheckAll();
            InGameUI.instance.HideTryAgainPanel();
            InGameUI.instance.FeedbackUI.ShowPanel(true);
        }
    }

    public void LevelComplete(bool _successfully)
    {
        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        if (!_successfully && GameManager.instance.MenuState == MenuState.GaveUp)
        {
            measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        }

        level.idSubliminalMeasurement1 = Data.CurrentLevel.isComplete==1||!_successfully ? 0 : measurement1Data.idSubliminalMeasurement1;
        level.idBehavior = Data.CurrentLevel.isComplete==1||!_successfully ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).idBehavior : measurement1Data.idBehavior;
        level.behaviorScore = GameManager.instance.MenuState!=MenuState.GaveUp ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore : measurement1Data.behaviorScore; // TODO : second part is an issue, returns 1
        level.coins = _successfully ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).goldCoins : 0;
        // Debug.Log("behaviourScore "+level.behaviorScore);
        
        if (LevelManager.instance.CurrentLevel == Level.Level1 || LevelManager.instance.CurrentLevel == Level.Level3)
        {
            GameManager.instance.PostUserAttempt();
            GameManager.instance.MenuState = MenuState.LevelSelect;
            GameManager.instance.LoadScene("Menu");
        }
        else
        {
            GameManager.instance.PostUserAttempt();
            GameManager.instance.MenuState = MenuState.LevelSelect;
            InGameUI.instance.FeedbackUI.UncheckAll();
            InGameUI.instance.FeedbackUI.ShowPanel(true);
        }
    }

    public void HideGrids()
    {
        if (GridManager!=null)
        {
            GridManager.gameObject.SetActive(false);
        }
        if (GridManager2!=null)
        {
            GridManager2.gameObject.SetActive(false);
        }
    }

    // --------------------------------- Inputs ---------------------------------
    public void Begin()
    {
        if (!hasBegun)
        {
            hasBegun = true;
        }
    }

    public void OnTouch(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
        }
    }

    // --------------------------------- UI ---------------------------------
    void UpdateTimer()
    {
        TimeLeft -= Time.deltaTime;
        InGameUI.instance.TimerUI.SetFill(TimeLeft/MaxTime);
        InGameUI.instance.SetTime(TimeLeft);

        if (TimeLeft < 0f)
        {
            // SetLevelFailed();
            TimeLeft = 0;
            Debug.Log("Failed");
            if (GridManager != null && (CurrentLevel == Level.Level3 || CurrentLevel == Level.Level4))
            {
                GridManager.LogTiles();
            }
            InGameUI.instance.ShowTimeOverPanel();
            GameManager.instance.MenuState = MenuState.TimeRanOut;
            hasBegun = false;
        }
    }
    
    private void OnEnable() 
    {
        InputManager.MouseClick += OnTouch;
    }

    private void OnDisable() 
    {
        InputManager.MouseClick -= OnTouch;
    }
}
