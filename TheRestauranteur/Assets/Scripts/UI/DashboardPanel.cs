using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardPanel : MonoBehaviour
{
    public TextMeshProUGUI Username_txt, DollarsEarned_txt;
    public List<DashboardFestivalProgress> FestivalProgresses;

    void Start()
    {
        
    }

    public void ShowPanel(bool _state)
    {
        foreach (DashboardFestivalProgress _progress in FestivalProgresses)
        {
            _progress.SetProgress();
            _progress.Blocker.SetActive(true);
        }
        
        switch (Data.instance.LastFestivalCompleted)
        {
            case TileType.Wheat:
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Wheat).Blocker.SetActive(false);
            break;

            case TileType.Grape:
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Wheat).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Grape).Blocker.SetActive(false);
            break;

            case TileType.Chocolate:
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Wheat).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Grape).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Chocolate).Blocker.SetActive(false);
            break;

            case TileType.Milk:
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Wheat).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Grape).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Chocolate).Blocker.SetActive(false);
            FestivalProgresses.Find((fest)=> fest.FestivalType==TileType.Milk).Blocker.SetActive(false);
            break;
        }

        Username_txt.SetText(Data.instance.UserName);
        DollarsEarned_txt.SetText("Dollars earned: = <color=red>"+Data.instance.Score);
        gameObject.SetActive(_state);
        if (!_state && LevelManager.instance!=null && LevelManager.instance.isComplete)
        {
            GameManager.instance.LoadScene("Questionare");
        }
    }

    void OnEnable() 
    {
        Time.timeScale = 0;
    }

    void OnDisable() 
    {
        Time.timeScale = 1;
    }
}
