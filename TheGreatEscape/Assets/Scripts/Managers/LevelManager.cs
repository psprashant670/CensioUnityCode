using UnityEngine;
using UnityEngine.InputSystem;


public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public PrinceController PrinceController;
    public bool hasKey = false;
    public bool hasBegun { get; set; }
    public Medallion MedallionTypeFound = Medallion.None;

    float TimeLeft = 0f, MaxTime;
    [HideInInspector]
    public int AttemptNo, Coins;
    LayerMask InteractableLayer;

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
        hasKey = false;
        hasBegun = false;
        MedallionTypeFound = Medallion.None;
        InteractableLayer = LayerMask.GetMask("Interact");

        LoadLevelData();

        InGameUI.instance.SetAttemptNo(AttemptNo);
        // InGameUI.SetCoins(Coins);
    }

    public void LoadLevelData()
    {
        TimeLeft = Data.instance.Timer;
        MaxTime = Data.instance.Timer; 
        AttemptNo = Data.instance.Attempts[(int)Data.instance.SelectedDifficulty]+1;
        Data.instance.Attempts[(int)Data.instance.SelectedDifficulty] = AttemptNo;
        Data.instance.CurrentLevel.attemptNumber = AttemptNo;
        Data.instance.CurrentLevel.NoOfClicks = 0;
        Data.instance.CurrentLevel.NoValidclicks = 0;
        Data.instance.CurrentLevel.ObstacleName = "";
        Data.instance.CurrentLevel.playerDied = false;
        Data.instance.ClickLog.Clear();
        // Coins = Data.instance.Coins;
    }

    public void SetLevelResult()
    {
        CheckMedallion();
        Data.instance.LevelComplete[(int)Data.instance.SelectedDifficulty] = true;

        // Data.instance.CurrentLevel.isComplete = (MedallionTypeFound != Medallion.None && hasKey) ? 1 : 0;
        Data.instance.CurrentLevel.isComplete = hasKey ? 1 : 0; // temp linear fix
        Data.instance.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);

        if (Data.instance.CurrentLevel.isComplete == 1)
        {
            int goldCoins = Data.instance.GetGameAttemptDataValue(Data.instance.CurrentLevel.idLevel, Data.instance.CurrentLevel.attemptNumber).goldCoins;
            Data.instance.Coins += goldCoins;
            // Debug.Log("Coins added "+goldCoins);
        }

        // GameManager.instance.PostUserAttempt();
    }

    public void SetLevelFailedResult()
    {
        Data.instance.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
        Data.instance.CurrentLevel.isComplete = 0;
        // GameManager.instance.PostUserAttempt();
    }

    public void CheckMedallion()
    {
        switch (MedallionTypeFound)
        {
            case Medallion.Red:
            Data.instance.RedMedallion = true;
            break;

            case Medallion.Green:
            Data.instance.GreenMedallion = true;
            break;

            case Medallion.Blue:
            Data.instance.BlueMedallion = true;
            break;

            case Medallion.Yellow:
            Data.instance.YellowMedallion = true;
            break;

            default:
            break;
        }
    }

    void FixedUpdate() 
    {
        UpdateTimer();
    }

    // --------------------------------- Inputs ---------------------------------
    public void Begin()
    {
        if (!hasBegun)
        {
            hasBegun = true;
            PrinceController.SetState(PlayerState.Run);
        }
    }

    public void OnTouch(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            Data.instance.CurrentLevel.NoOfClicks++;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()), Vector2.zero, 100f, InteractableLayer);
            if(hit.collider != null)
            {
                hit.collider.GetComponent<TriggerInteractable>()?.Trigger();
            }
        }
    }

    // --------------------------------- UI ---------------------------------
    void UpdateTimer()
    {
        TimeLeft -= Time.deltaTime;
        InGameUI.instance.Hourglass.SetHourGlass(TimeLeft/MaxTime);
        if (TimeLeft < 0.1f)
        {
            SetLevelFailedResult();
            GameManager.instance.MenuState = MenuState.TimeRanOut;
            GameManager.instance.LoadScene("Menu");
        }
    }

    void OnEnable() 
    {
        InputManager.MouseClick += OnTouch;
    }

    void OnDisable() 
    {
        InputManager.MouseClick -= OnTouch;
    }
}
