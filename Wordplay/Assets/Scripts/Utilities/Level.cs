using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public SelectedLevel SelectedLevel;
    public Image Image;
    public Sprite Completed, Unlocked, Locked;

    void Start()
    {
        
    }

    public void SetState(int _state) //0, 1, 2
    {
        switch (_state)
        {
            case 0:
            Image.sprite = Completed;
            break;

            case 1:
            Image.sprite = Unlocked;
            break;

            case 2:
            Image.sprite = Locked;
            break;

            default:
            break;
        }
    }

    public void SelectLevel()
    {
        if (Image.sprite != Unlocked)
        {
            return;
        }
        ResetLevelData();
        MenuUI.instance.HideAllPanels();
        MenuUI.instance.LoadingPanel.SetActive(true);
        GameManager.instance.Data.SelectedLevel = SelectedLevel;
        GameManager.instance.Data.CurrentLevel.SelectedLevel = SelectedLevel;
        GameManager.instance.Data.GetGame4LevelInstructions_Co(GameManager.instance, (int)SelectedLevel);
    }

    void ResetLevelData()
    {
        GameManager.instance.Data.CurrentLevel.levelTimerequired = 0;
        GameManager.instance.Data.CurrentLevel.levelWordcount = 0;
        GameManager.instance.Data.CurrentLevel.timeTaken = 0;
        GameManager.instance.Data.CurrentLevel.bonusGranted = 0;
        GameManager.instance.Data.CurrentLevel.levelBonuspts = 0;
        GameManager.instance.Data.CurrentLevel.WordSelected = "";
    }
}
