using System.Collections.Generic;
using UnityEngine;

public class LevelSelectPanel : MonoBehaviour
{
    public List<Level> Levels;
    public Data Data;
    
    void Start()
    {
        SetLevels();
    }

    public void SetLevels()
    {
        for (int i = 0; i < 3; i++)
        {
            if ((int)Levels[i].SelectedLevel <= Data.LastLevelID)
            {
               //star
               Levels[i].SetState(0);
            }
            else if ((int)Levels[i].SelectedLevel == (Data.LastLevelID+1))
            {
                //unlocked
               Levels[i].SetState(1);
            }
            else
            {
                //locked
               Levels[i].SetState(2);
            }
        }

        if (Data.LastLevelID >= 3)
        {
            MenuUI.instance.MenuPopupPanel.ShowGameOver(true);
        }
    }
}
