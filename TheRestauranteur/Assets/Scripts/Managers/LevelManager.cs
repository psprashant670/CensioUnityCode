using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public GridManager GridManager;
    public TileType CurrentFestival, CurrentLevelProgress;
    public float TimeLeft;
    public bool isReady = false, hasBegun = false, isComplete = false;

    public int Wheat, Grape, Chocolate, Milk;
    public int MaxWheat, MaxGrape, MaxChocolate, MaxMilk;
    public float MaxTime { get; set; }
    public FestivalData festivalData { get; set; }
    public Tile LastSelected { get; set; }

    Camera cam;
    Vector2 PointerWorldPos;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        isComplete = false;
        cam = Camera.main;
        MaxTime = 30f;
        TimeLeft = MaxTime;
        InGameUI.instance.UpdateTimeLeft((int)TimeLeft);

        if (Data.instance.SaveData == null)
        {
            InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Wheat, Data.instance.WheatFest.timeRequiredSeconds);
        }
        else
        {
            if (Data.instance.SaveData.festivalName == Data.instance.WheatFest.festivalName)
            {
                CurrentFestival = Data.instance.WheatFest.FestivalType;
                Data.instance.WheatFest.FestivalProgress = (TileType)Data.instance.SaveData.endIdLevel;
                InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Grape, Data.instance.GrapeFest.timeRequiredSeconds);
            }
            else if (Data.instance.SaveData.festivalName == Data.instance.GrapeFest.festivalName)
            {
                CurrentFestival = Data.instance.GrapeFest.FestivalType;
                Data.instance.GrapeFest.FestivalProgress = (TileType)Data.instance.SaveData.endIdLevel;
                InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Chocolate, Data.instance.ChocolateFest.timeRequiredSeconds);
            }
            else if (Data.instance.SaveData.festivalName == Data.instance.ChocolateFest.festivalName)
            {
                CurrentFestival = Data.instance.ChocolateFest.FestivalType;
                Data.instance.ChocolateFest.FestivalProgress = (TileType)Data.instance.SaveData.endIdLevel;
                InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Milk, Data.instance.MilkFest.timeRequiredSeconds);
            }
            else
            {
                CurrentFestival = Data.instance.WheatFest.FestivalType;
                InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Wheat, Data.instance.WheatFest.timeRequiredSeconds);
            }
        }
    }

    public void BeginFestival(TileType _festivaltype)
    {
        CurrentFestival = _festivaltype;
        festivalData = Data.instance.Festivals.Find((fest)=> fest.FestivalType == CurrentFestival);
        festivalData.FestivalSuccess = 0;
        festivalData.FestivalScore = 0;
        festivalData.LevelStartScore = Data.instance.Score;
        MaxWheat = festivalData.MaxWheat;
        MaxGrape = festivalData.MaxGrape;
        MaxChocolate = festivalData.MaxChocolate;
        MaxMilk = festivalData.MaxMilk;
        MaxTime = festivalData.timeRequiredSeconds;
        TimeLeft = MaxTime;
        InGameUI.instance.SetFestivalTitle(CurrentFestival);

        switch (CurrentFestival)
        {
            case TileType.Wheat:
            Wheat = festivalData.Wheat;
            Grape = festivalData.Grape;
            Chocolate = festivalData.Chocolate;
            Milk = festivalData.Milk;
            break;

            case TileType.Grape:
            Wheat = festivalData.Wheat;
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Wheat).ResetBasket(Mathf.Clamp(Wheat, 0, MaxWheat)/100f);
            break;

            case TileType.Chocolate:
            Wheat = festivalData.Wheat;
            Grape = festivalData.Grape;
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Wheat).ResetBasket(Mathf.Clamp(Wheat, 0, MaxWheat)/100f);
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Grape).ResetBasket(Mathf.Clamp(Grape, 0, MaxGrape)/100f);
            break;

            case TileType.Milk:
            Wheat = festivalData.Wheat;
            Grape = festivalData.Grape;
            Chocolate = festivalData.Chocolate;
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Wheat).ResetBasket(Mathf.Clamp(Wheat, 0, MaxWheat)/100f);
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Grape).ResetBasket(Mathf.Clamp(Grape, 0, MaxGrape)/100f);
            InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Chocolate).ResetBasket(Mathf.Clamp(Chocolate, 0, MaxChocolate)/100f);
            break;

            default:
            break;
        }
        GridManager.LoadBoard();
        // InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Wheat).ResetBasket(Mathf.Clamp(Wheat, 0, MaxWheat)/100f);
        // InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Grape).ResetBasket(Mathf.Clamp(Grape, 0, MaxGrape)/100f);
        // InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Chocolate).ResetBasket(Mathf.Clamp(Chocolate, 0, MaxChocolate)/100f);
        // InGameUI.instance.ProgressBaskets.Find((basket)=> basket.Type==TileType.Milk).ResetBasket(Mathf.Clamp(Milk, 0, MaxMilk)/100f);
        TimeLeft = MaxTime;
        hasBegun = true;
        CheckFestivalData();
    }

    public void CompleteFestival(bool _successfully)
    {
        hasBegun = false;
        Data.instance.LastFestivalCompleted = CurrentFestival;
        FestivalData festivalData = Data.instance.Festivals.Find((fest)=> fest.FestivalType == CurrentFestival);
        festivalData.Wheat = Mathf.Clamp(Wheat, 0, MaxWheat);
        festivalData.Grape = Mathf.Clamp(Grape, 0, MaxGrape);
        festivalData.Chocolate = Mathf.Clamp(Chocolate, 0, MaxChocolate);
        festivalData.Milk = Mathf.Clamp(Milk, 0, MaxMilk);
        festivalData.ExcessWheat = Mathf.Clamp(Wheat-MaxWheat, 0, 999);
        festivalData.ExcessGrape = Mathf.Clamp(Grape-MaxGrape, 0, 999);
        festivalData.ExcessChocolate = Mathf.Clamp(Chocolate-MaxChocolate, 0, 999);
        festivalData.ExcessMilk = Mathf.Clamp(Milk-MaxMilk, 0, 999);
        
        festivalData.FestivalProgress = CurrentLevelProgress;
        festivalData.LevelEndScore = Data.instance.Score;
        festivalData.LevelCumulativeScore = Data.instance.Score;
        festivalData.TimeTaken = (int)(MaxTime-TimeLeft);
        GridManager.ClearBoard();
        InGameUI.instance.SetScore();
        InGameUI.instance.ProgressBar.SetBarFill(Data.instance.Score);
        // InGameUI.instance.CongratzPanel.ShowPanel();
        if (_successfully)
        {
            InGameUI.instance.CongratzPanel.ShowPanel();
        }
        else
        {
            InGameUI.instance.CongratzPanel.OKClick();
        }

        Data.instance.PostGame3Log_Co(GameManager.instance, festivalData);
        Data.instance.PostGame3RawMaterialLog_Co(GameManager.instance);
        Data.instance.MaterialLogs.Clear();
    }

    public void CheckFestivalData()
    {
        if (!hasBegun)
        {
            return;
        }

        switch (CurrentFestival)
        {
            case TileType.Wheat:
            if (Wheat >= MaxWheat)
            {
                Data.instance.Score += Data.instance.primaryScore;
                festivalData.FestivalScore += Data.instance.primaryScore;
                festivalData.FestivalSuccess = 1;
                CompleteFestival(true);
            }
            break;

            case TileType.Grape:
            if (Grape >= MaxGrape)
            {
                Data.instance.Score += Data.instance.primaryScore;
                festivalData.FestivalScore += Data.instance.primaryScore;
                festivalData.FestivalSuccess = 1;
                CompleteFestival(true);
            }
            break;

            case TileType.Chocolate:
            if (Chocolate >= MaxChocolate)
            {
                Data.instance.Score += Data.instance.primaryScore;
                festivalData.FestivalScore += Data.instance.primaryScore;
                festivalData.FestivalSuccess = 1;
                CompleteFestival(true);
            }
            break;

            case TileType.Milk:
            if (Milk >= MaxMilk)
            {
                Data.instance.Score += Data.instance.primaryScore;
                festivalData.FestivalScore += Data.instance.primaryScore;
                festivalData.FestivalSuccess = 1;
                CompleteFestival(true);
            }
            break;

            default:
            break;
        }

        InGameUI.instance.ProgressBar.SetBarFill(Data.instance.Score);
    }

    public void FinishFestivals()
    {
        hasBegun = false;
        isComplete = true;
        InGameUI.instance.DashboardPanel.ShowPanel(true);
    }

    public void UpdateBaskets()
    {
        List<ProgressBasket> baskets = InGameUI.instance.ProgressBaskets;
        for (int i = 0; i < baskets.Count; i++)
        {
            switch (baskets[i].Type)
            {
                case TileType.Wheat:
                baskets[i].SetFill(Mathf.Clamp(Wheat, 0, MaxWheat)/100f);
                break;

                case TileType.Grape:
                baskets[i].SetFill(Mathf.Clamp(Grape, 0, MaxGrape)/100f);
                break;

                case TileType.Chocolate:
                baskets[i].SetFill(Mathf.Clamp(Chocolate, 0, MaxChocolate)/100f);
                break;

                case TileType.Milk:
                baskets[i].SetFill(Mathf.Clamp(Milk, 0, MaxMilk)/100f);
                break;
            }
        }
    }

    void FixedUpdate() 
    {
        if (hasBegun)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        TimeLeft -= Time.deltaTime;
        InGameUI.instance.UpdateTimeLeft((int)TimeLeft);

        if (TimeLeft < 0f && !GridManager.isSwapping)
        {
            TimeLeft = 0;
            hasBegun = false;
            UpdateBaskets();
            //TODO
            switch (CurrentFestival)
            {
                case TileType.Wheat:
                if (Wheat >= MaxWheat)
                {
                    Data.instance.Score += Data.instance.primaryScore;
                    festivalData.FestivalScore += Data.instance.primaryScore;
                    festivalData.FestivalSuccess = 1;
                }
                // CompleteFestival(Wheat >= MaxWheat);
                break;

                case TileType.Grape:
                if (Grape >= MaxGrape)
                {
                    Data.instance.Score += Data.instance.primaryScore;
                    festivalData.FestivalScore += Data.instance.primaryScore;
                    festivalData.FestivalSuccess = 1;
                }
                // CompleteFestival(Grape >= MaxGrape);
                break;

                case TileType.Chocolate:
                if (Chocolate >= MaxChocolate)
                {
                    Data.instance.Score += Data.instance.primaryScore;
                    festivalData.FestivalScore += Data.instance.primaryScore;
                    festivalData.FestivalSuccess = 1;
                }
                // CompleteFestival(Chocolate >= MaxChocolate);
                break;

                case TileType.Milk:
                if (Milk >= MaxMilk)
                {
                    Data.instance.Score += Data.instance.primaryScore;
                    festivalData.FestivalScore += Data.instance.primaryScore;
                    festivalData.FestivalSuccess = 1;
                }
                // CompleteFestival(Milk >= MaxMilk);
                break;
            }
            CompleteFestival(true);
            InGameUI.instance.ProgressBar.SetBarFill(Data.instance.Score);
        }
    }

    public void OnTouch(InputAction.CallbackContext value)
    {
        if (!isReady)
        {
            return;
        }

        if (value.performed)
        {
            PointerWorldPos = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            GridManager?.ClickStart(PointerWorldPos);
        }
        else if (value.canceled)
        {
            PointerWorldPos = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
            GridManager?.ClickEnd(PointerWorldPos);
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
