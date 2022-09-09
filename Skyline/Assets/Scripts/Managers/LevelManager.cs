using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public SelectedLevel CurrentLevel;
    public Transform CamTarget;
    public LayerMask BuildingBlocksLayer;
    public VerticalParallax Parallax;
    public bool CanDrop = false;
    public bool hasBegun { get; set; }
    public bool hasAtleastOneBlock { get; set; }
    public BlockColor RequiredBlockColor;
    public List<Game2BlockAccuracyLog> Game2BlockAccuracyLog = new List<Game2BlockAccuracyLog>();

    [HideInInspector]
    public int AttemptNo, Coins;

    [SerializeField]
    float TimeLeft = 0f, MaxTime;
    Data Data;
    Camera Camera;
    CinemachineImpulseSource ImpulseSource;

    private void Awake() 
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
        hasAtleastOneBlock = false;
        Game2BlockAccuracyLog.Clear();

        Camera = Camera.main;
        ImpulseSource = GetComponent<CinemachineImpulseSource>();
        CanDrop = true;

        LoadLevelData();

        InGameUI.instance.TimerUI.SetFill(TimeLeft/MaxTime);
        InGameUI.instance.SetAttemptNo(AttemptNo);
        InGameUI.instance.SetCoins();

        // InGameUI.instance.AttemptNo_txt.gameObject.SetActive(false);
        // InGameUI.instance.TimerUI.gameObject.SetActive(false);
    }

    public void LoadLevelData()
    {
        
        #if UNITY_EDITOR
        Data.SelectedLevel = CurrentLevel;
        #endif
        
        TimeLeft = Data.CurrentLevel.attemptTimer;
        MaxTime = Data.CurrentLevel.attemptTimer;
        Data.CurrentLevel.attemptNumber++;
        AttemptNo = Data.CurrentLevel.attemptNumber;
        Data.CurrentLevel.accuracy = 0;
        Data.CurrentLevel.blocksPresented = 0;
        Data.CurrentLevel.blocksClicked = 0;
        Data.CurrentLevel.correctBlocks = 0;
        Data.CurrentLevel.successfulBlocks = 0;
    }

    void FixedUpdate() 
    {
        if (hasBegun)
        {
            UpdateTimer();
        }
    }

    public void SetCamTarget(float _height)
    {
        CamTarget.position = new Vector3(CamTarget.position.x, _height, CamTarget.position.z);
        Parallax?.ReCalculate();
    }

    public void OnLanded(BuildingBlock _block)
    {
        CanDrop = true;
        // ImpulseSource.GenerateImpulse();
    }

    public int CalculatePercentage() // int 0 - 100
    {
        float per, sum = 0;
        List<Game2BlockAccuracyLog> logs = LevelManager.instance.Game2BlockAccuracyLog.FindAll((log)=> log.IsCorrect == 1);
        for (int i = 0; i < logs.Count; i++)
        {
            sum += logs[i].AccuracyLevel;
        }
        per = Mathf.Clamp01(sum/(logs.Count)/100f);
        if (float.IsNaN(per))
        {
            per = 0;
        }

        Debug.Log(per);
        return Mathf.RoundToInt(per*100);
    }

    public void CalculateCoins(bool _success)
    {
        if (!_success)
        {
            Data.instance.CurrentLevel.rewardCoins = 0;
            return;
        }
        AttemptData attemptData = Data.GetGameAttemptDataValue(Data.CurrentLevel.idLevel, Data.CurrentLevel.attemptNumber);

        int rewardCoins = attemptData.rewardCoinsTime1;
        Data.instance.CurrentLevel.rewardCoins = rewardCoins;
        Data.instance.RewardCoins += rewardCoins;
        InGameUI.instance?.SetCoins();
    }

    public void TryAgain()
    {
        int accuracy = CalculatePercentage();
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Data.CurrentLevel.accuracy = accuracy;
        Data.CurrentLevel.isComplete = accuracy>=Data.instance.AccuracyRequired ? 1 : 0;
        CalculateCoins(accuracy>=Data.instance.AccuracyRequired);
        if (Data.CurrentLevel.isComplete==1)
        {
            Data.CurrentLevel.isPreviouslyComplete = true;
        }
        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        // level.idBehavior = measurement1Data.idBehavior;
        level.idBehavior = Data.CurrentLevel.isComplete==1 ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).idBehavior : 0;
        level.behaviorScore = Data.CurrentLevel.isComplete==1 ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore : 0;

        GameManager.instance.PostUserAttempt();
        if (Data.SelectedLevel!=SelectedLevel.None && level.attemptNumber < level.attemptsAllowed)
        {
            GameManager.instance.ReloadScene();
        }
    }

    public void MoveOn()
    {
        int accuracy = CalculatePercentage();
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Data.CurrentLevel.isComplete = accuracy>=Data.instance.AccuracyRequired ? 1 : -1;
        Data.CurrentLevel.accuracy = accuracy;
        CalculateCoins(accuracy>=Data.instance.AccuracyRequired);
        if (Data.CurrentLevel.isComplete==1)
        {
            Data.CurrentLevel.isPreviouslyComplete = true;
        }
        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        // Debug.Log("attemptNo ="+attemptNo);

        level.idSubliminalMeasurement1 = measurement1Data.idSubliminalMeasurement1;
        level.behaviorScore = measurement1Data.behaviorScore;
        // level.idBehavior = measurement1Data.idBehavior;
        level.idBehavior = 0;
        
        GameManager.instance.PostUserAttempt();
        GameManager.instance.MenuState = MenuState.LevelSelect;
        GameManager.instance.LoadScene("Menu");
    }


    public void LevelComplete(bool _successfully)
    {
        int accuracy = CalculatePercentage();
        Data.CurrentLevel.accuracy = accuracy;
        Data.CurrentLevel.isComplete = _successfully? 1:-1;
        Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        CalculateCoins(accuracy>=Data.instance.AccuracyRequired);
        if (Data.CurrentLevel.isComplete==1)
        {
            Data.CurrentLevel.isPreviouslyComplete = true;
        }
        LevelData level = Data.CurrentLevel;
        SubliminalMeasurement1Data measurement1Data = Data.GetSubliminalMeasurement1Data("Try Again");
        if (!_successfully && GameManager.instance.MenuState == MenuState.GaveUp)
        {
            measurement1Data = Data.GetSubliminalMeasurement1Data("Move On");
        }

        level.idSubliminalMeasurement1 = Data.CurrentLevel.isComplete==1||!_successfully ? 0 : measurement1Data.idSubliminalMeasurement1;
        level.idBehavior = 0;
        level.behaviorScore = Data.CurrentLevel.isComplete==1 ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).behaviorScore : 0;
        // level.rewardCoins = _successfully ? Data.GetGameAttemptDataValue(level.idLevel, level.attemptNumber).rewardCoinsTime1 : 0;
        // Debug.Log("behaviourScore "+level.behaviorScore);
        
        GameManager.instance.PostUserAttempt();
        GameManager.instance.MenuState = MenuState.LevelSelect;
        GameManager.instance.LoadScene("Menu");
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
        if (hasBegun && value.performed && CanDrop)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.ScreenToWorldPoint(Pointer.current.position.ReadValue()), Vector2.zero, 100f, BuildingBlocksLayer);
            
            if (hit.collider != null)
            {
                BuildingBlock block = hit.collider.GetComponent<BuildingBlock>();
                if (!block.isClicked && block.Color == RequiredBlockColor)
                {
                    CanDrop = false;
                    hasAtleastOneBlock = true;
                    block.SetRigid();
                }
            }
        }
    }

    // --------------------------------- UI ---------------------------------
    void UpdateTimer()
    {
        TimeLeft -= Time.deltaTime;
        InGameUI.instance.TimerUI.SetFill(TimeLeft/MaxTime);

        if (TimeLeft < 0f)
        {
            BuildingBlock[] remainingBlocks = GameObject.FindObjectsOfType<BuildingBlock>();
            foreach (BuildingBlock _block in remainingBlocks)
            {
                _block.AddRemainingLogs();
            }
            TimeLeft = 0;
            InGameUI.instance.ResultsPanel.ShowResult(true);
            hasBegun = false;
            Data.instance.Game2BlockAccuracyLogs = Game2BlockAccuracyLog;
        }
    }

    public void SetBlockColor(BlockColor _color)
    {
        // Debug.Log(_color);
        RequiredBlockColor = _color;
        InGameUI.instance.RequiredColorUI.SetColor(_color);
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

public enum BlockColor
{
    None,
    Grey,
    Green,
    Red,
    Orange,
    Cherry,
    Blue
}