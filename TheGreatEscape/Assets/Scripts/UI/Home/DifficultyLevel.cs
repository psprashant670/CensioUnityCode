using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyLevel : MonoBehaviour
{
    public Difficulty Difficulty;
    public Image SelectImage;
    public TextMeshProUGUI SelectText;
    public GameObject FailedOverlay, AttemptOverlay, CompleteOverlay;
    public TextMeshProUGUI AttemptsLeft_txt;


    public Sprite Green, Red;
    public bool isSelected, canBeSelected = false;
    public int attemptNo;

    public DifficultyPanel DifficultyPanel { get; set; }

    public void SelectDifficulty()
    {
        if (canBeSelected)
        {
            DifficultyPanel.isAttemptOverlayActive = AttemptOverlay.activeSelf;

            DifficultyPanel.DeselectAll();
            SetSelected(true);
            Data.instance.SelectedDifficulty = Difficulty;
        }
    }

    public void SetSelected(bool _state)
    {
        isSelected = _state;
        SelectImage.sprite = isSelected ? Green : Red;
        SelectText.color = isSelected ? Color.black : Color.white;
    }

    public bool Evaluate()
    {
        canBeSelected = false;
        attemptNo = Data.instance.Attempts[(int)Difficulty];

        // level completed
        if (Data.instance.GetCurrentLevelFor(Difficulty).isComplete==1)
        {
            CompleteOverlay.SetActive(true);
            FailedOverlay.SetActive(false);
            AttemptOverlay.SetActive(false);
            SelectImage.gameObject.SetActive(false);
            return false;
        }

        // // if all levels incomplete and no attempts left = failed all
        // bool attemptPending = false;
        // for (int i = 0; i < 4; i++)
        // {
        //     attemptPending = Data.instance.GetCurrentLevelFor((Difficulty)i).isComplete==1;
        //     if (attemptPending)
        //     {
        //         break;
        //     }
        // }
        // bool l1 = Data.instance.Attempts[0] >= Data.instance.Levels[0].attemptsAllowed && (Data.instance.Coins < Data.instance.Levels[0].againPlayCoins);
        // bool l2 = Data.instance.Attempts[1] >= Data.instance.Levels[1].attemptsAllowed && (Data.instance.Coins < Data.instance.Levels[1].againPlayCoins);
        // bool l3 = Data.instance.Attempts[2] >= Data.instance.Levels[2].attemptsAllowed && (Data.instance.Coins < Data.instance.Levels[2].againPlayCoins);
        // bool l4 = Data.instance.Attempts[3] >= Data.instance.Levels[3].attemptsAllowed && (Data.instance.Coins < Data.instance.Levels[3].againPlayCoins);
        // if (!attemptPending && l1 && l2 && l3 && l4)
        // {
        //     FailedOverlay.SetActive(true);
        //     AttemptOverlay.SetActive(false);
        //     CompleteOverlay.SetActive(false);
        //     SelectImage.gameObject.SetActive(false);
        //     return false;
        // }

        // if all levels incomplete and no attempts left = failed all
        bool moveAvailable = false;
        for (int i = 0; i < 4; i++)
        {
            bool l1 = Data.instance.Attempts[i] >= Data.instance.Levels[i].attemptsAllowed && (Data.instance.Coins < Data.instance.Levels[i].againPlayCoins);
            moveAvailable = Data.instance.GetCurrentLevelFor((Difficulty)i).isComplete==0 && !l1;
            if (moveAvailable)
            {
                break;
            }
        }
        if (!moveAvailable)
        {
            FailedOverlay.SetActive(true);
            AttemptOverlay.SetActive(false);
            CompleteOverlay.SetActive(false);
            SelectImage.gameObject.SetActive(false);
            return false;
        }

        // first attempt
        if (attemptNo == 0)
        {
            FailedOverlay.SetActive(false);
            AttemptOverlay.SetActive(false);
            CompleteOverlay.SetActive(false);
            SelectImage.gameObject.SetActive(true);
            canBeSelected = true;
            return true;
        }
        else if (attemptNo < Data.instance.Levels[(int)Difficulty].attemptsAllowed || Data.instance.GetCurrentLevelFor(Difficulty).isComplete==0) // possibility to try again if attempt or coins
        {
            FailedOverlay.SetActive(false);
            AttemptOverlay.SetActive(true);
            SelectImage.gameObject.SetActive(true);
            CompleteOverlay.SetActive(false);
            AttemptsLeft_txt.SetText("Attempt Left "+(Data.instance.Levels[(int)Difficulty].attemptsAllowed - attemptNo));
            canBeSelected = true;
            return true;
        }

        // fallback for whatever reason
        return false;
    }

    public void SetLocked()
    {
        // Debug.Log("Locked "+Difficulty);
        FailedOverlay.SetActive(false);
        AttemptOverlay.SetActive(false);
        SelectImage.gameObject.SetActive(true);
        CompleteOverlay.SetActive(true);
        canBeSelected = false;
        SelectImage.gameObject.SetActive(false);
    }
}

[System.Serializable]
public enum Difficulty
{
    Level1,
    Level2,
    Level3,
    Level4,
    None
}