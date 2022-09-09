using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public List<LevelUI> LevelUIs;
    List<LevelData> Levels;

    void Start()
    {
        
    }

    public void SetData()
    {
        foreach (LevelUI _LevelUI in LevelUIs)
        {
            _LevelUI.LevelSelect = this;
            _LevelUI.Deselect();
        }
        
        Data.instance.SelectedLevel = SelectedLevel.None;
        Levels = Data.instance.Levels;
        for (int i = 0; i < Levels.Count; i++)
        {
            LevelUIs[i].SetStatus((Levels[i].isComplete==1||Levels[i].isPreviouslyComplete)?1:Levels[i].isComplete, Levels[i].attemptNumber);
            // LevelUIs[i].SetStatus(0, Levels[i].attemptNumber);
        }

        LinearFix();

        bool allComplete = true;
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].isComplete == 0)
            {
                allComplete = false;
            }
        }
        if (allComplete)
        {
            MenuUI.instance.ShowGameOver();
        }
    }

    public void DeselectLevels()
    {
        foreach (LevelUI _LevelUI in LevelUIs)
        {
            _LevelUI.Deselect();
        }
    }

    void LinearFix()
    {
        bool lockRest = false;
        for (int i = 0; i < Levels.Count; i++)
        {
            if (!lockRest)
            {
                lockRest = Levels[i].isComplete == 0;
            }
            else
            {
                LevelUIs[i].SetLocked(); 
            }
        }
    }

    public void SelectClicked()
    {
        if (Data.instance.SelectedLevel == SelectedLevel.None)
        {
            return;
        }
        gameObject.SetActive(false);
        MenuUI.instance.LevelIntroPanel.gameObject.SetActive(true);
    }
}
