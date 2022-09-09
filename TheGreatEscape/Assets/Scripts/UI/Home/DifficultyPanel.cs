using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyPanel : MonoBehaviour
{
    public List<DifficultyLevel> DifficultyLevels;
    public GameObject NextButton, TutorialScreen;
    public TextMeshProUGUI TutorialScreenTitle_txt, TutorialScreenDesc_txt;

    public bool isAttemptOverlayActive = false;
    bool canProgress = false;

    void Start()
    {
        foreach (DifficultyLevel _level in DifficultyLevels)
        {
            _level.DifficultyPanel = this;
            _level.SetSelected(false);
        }

        UpdateAllStates();
    }

    public void UpdateAllStates()
    {
        canProgress = false;

        foreach (DifficultyLevel _level in DifficultyLevels)
        {
            bool isPossible = _level.Evaluate();
            if (!canProgress)
            {
                canProgress = isPossible;
                _level.SelectDifficulty();
            }
        }

        bool canLinearProgress = LinearFlowFix();
        
        // changed to canProgress to canLinearProgress bool instead
        NextButton.SetActive(canProgress && canLinearProgress);
        if (!canLinearProgress)
        {
            Data.instance.SelectedDifficulty = Difficulty.None;
            MenuUI.instance.ErrorPanel.ShowGameOver();
        }
    }

    public bool LinearFlowFix()
    {
        PreviousLevelData previousLevelData = Data.instance.PreviousLevelData;
        Difficulty selected = Difficulty.None;
        // Debug.Log("Previous Attempts = "+(previousLevelData==null?"null":JsonUtility.ToJson(previousLevelData).ToString()));
        if (previousLevelData == null)
        {
            selected = Difficulty.Level1;
            Data.instance.PreviousLevelData = new PreviousLevelData();
        }
        else
        {
            // //incase last level complete
            // if (previousLevelData.idLevel==16 && (previousLevelData.isCompleted==1 || previousLevelData.behaviorScore == -1 || previousLevelData.attemptNo == 4))
            // {
            //     Debug.Log("lvl16");
            //     selected = Difficulty.None;
            // }
            // Debug.Log(previousLevelData.behaviorScore);
            // Debug.Log(previousLevelData.idLevel);
            if (previousLevelData.isCompleted==1 || previousLevelData.behaviorScore == -1 || previousLevelData.attemptNo == 4)
            {
                previousLevelData.idLevel++;
            }
            else
            {
                //update attempts
                int attemptIndex = 0;
                switch (previousLevelData.idLevel)
                {
                    case 13:
                    attemptIndex = 0;
                    break;
                    case 14:
                    attemptIndex = 1;
                    break;
                    case 15:
                    attemptIndex = 2;
                    break;
                    case 16:
                    attemptIndex = 3;
                    break;
                    default:
                    attemptIndex = -1;
                    break;
                }
                if (attemptIndex!=-1)
                {
                    Data.instance.Attempts[attemptIndex] = previousLevelData.attemptNo;
                }
                // Debug.Log(Data.instance.CurrentLevel.attemptNumber);
            }
            // Debug.Log(previousLevelData.idLevel);
            previousLevelData.idLevel = Mathf.Clamp(previousLevelData.idLevel, 13, 17);
            switch (previousLevelData.idLevel)
            {
                case 13:
                selected = Difficulty.Level1;
                break;
                case 14:
                selected = Difficulty.Level2;
                break;
                case 15:
                selected = Difficulty.Level3;
                break;
                case 16:
                selected = Difficulty.Level4;
                break;
                case 17:
                selected = Difficulty.None;
                break;
                default:
                break;
            }
        }

        // Debug.Log(selected);

        for (int i = 0; i < DifficultyLevels.Count; i++)
        {
            if (DifficultyLevels[i].Difficulty != selected)
            {
                if (DifficultyLevels[i].Difficulty<selected)
                {
                    DifficultyLevels[i].SetLocked();
                }
                else
                {
                    DifficultyLevels[i].SelectImage.gameObject.SetActive(false);
                    DifficultyLevels[i].canBeSelected = false;
                }
            }
            else
            {
                DifficultyLevels[i].SelectDifficulty();
            }
        }

        bool isPossible = selected!=Difficulty.None;
        return isPossible;
    }

    public void DeselectAll()
    {
        foreach (DifficultyLevel _level in DifficultyLevels)
        {
            _level.SetSelected(false);
        }
    }

    public void ShowNextTutorial()
    {
        bool isCoinsPurchased = false;

        DifficultyLevel difficulty = DifficultyLevels.Find((lvl)=> lvl.Difficulty == Data.instance.SelectedDifficulty);
        // Debug.Log(difficulty.attemptNo);//0
        // Debug.Log(Data.instance.Levels[(int)Data.instance.SelectedDifficulty].againPlayCoins);//0<80 // coins
        // Debug.Log(Data.instance.Levels[(int)Data.instance.SelectedDifficulty].attemptsAllowed);//0<4 // attempts

        if (difficulty.AttemptOverlay.activeSelf && (Data.instance.Coins<Data.instance.Levels[(int)Data.instance.SelectedDifficulty].againPlayCoins) && (difficulty.attemptNo>=Data.instance.Levels[(int)Data.instance.SelectedDifficulty].attemptsAllowed))
        {
            MenuUI.instance.ErrorPanel.ShowCoinsError();
            return;
        }
        else if (Data.instance.Coins>=Data.instance.Levels[(int)Data.instance.SelectedDifficulty].againPlayCoins && (difficulty.attemptNo>=Data.instance.Levels[(int)Data.instance.SelectedDifficulty].attemptsAllowed))
        {
            // Debug.Log("coins removed");
            Data.instance.Coins -= Data.instance.Levels[(int)Data.instance.SelectedDifficulty].againPlayCoins;
            MenuUI.instance.SetCoins();
            Data.instance.Levels[(int)Data.instance.SelectedDifficulty].attemptsAllowed++;
            isCoinsPurchased = true;
        }

        TutorialScreen.SetActive(true);
        gameObject.SetActive(false);
        LevelData selectedLevel = Data.instance.CurrentLevel;
        if (selectedLevel != null)
        {
            TutorialScreenTitle_txt.SetText(selectedLevel.challengeName);
            TutorialScreenDesc_txt.SetText(selectedLevel.challengeIntroMessage);
        }

        if (isCoinsPurchased)
        {
            GameManager.instance.PostUserPurchase();
        }
    }
}
