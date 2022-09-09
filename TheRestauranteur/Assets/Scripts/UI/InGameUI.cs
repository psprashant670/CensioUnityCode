using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    public CongratzPanel CongratzPanel;
    public NewFestivalPanel NewFestivalPanel;
    public DashboardPanel DashboardPanel;

    public ProgressBar ProgressBar;
    public List<ProgressBasket> ProgressBaskets;

    public TextMeshProUGUI TimeLeft_txt, FestivalTitle_txt, Score_txt, LifeNo_txt;
    public SpriteRenderer BreadBg, WineBg, ChocolateBg, CheeseBg;

    List<GameObject> AllPanels;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        AllPanels = new List<GameObject>();
        AllPanels.Add(CongratzPanel.gameObject);
        AllPanels.Add(NewFestivalPanel.gameObject);
        AllPanels.Add(DashboardPanel.gameObject);
    }

    public void HideAllPanels()
    {
        foreach (GameObject _panel in AllPanels)
        {
            _panel.SetActive(false);
        }
    }

    public void UpdateTimeLeft(int _time)
    {
        if (_time<0)
        {
            _time = 0;
        }
        int minutes = Mathf.FloorToInt(_time / 60f);
        int seconds = Mathf.FloorToInt(_time - minutes * 60);
        TimeLeft_txt.SetText("= "+string.Format("{0:0}:{1:00}", minutes, seconds));
    }

    public void SetScore()
    {
        Score_txt.SetText("= "+Data.instance.Score);
    }

    public void SetFestivalTitle(TileType _type)
    {
        switch (_type)
        {
            case TileType.Wheat:
            FestivalTitle_txt.SetText("Bread Festival");
            LifeNo_txt.SetText("lives 1/4");
            break;

            case TileType.Grape:
            FestivalTitle_txt.SetText("Wine Festival");
            LifeNo_txt.SetText("lives 2/4");
            BreadBg.DOFade(0, 2f).OnComplete(()=>{
                BreadBg.gameObject.SetActive(false);
            });
            break;

            case TileType.Chocolate:
            FestivalTitle_txt.SetText("Chocolate Festival");
            LifeNo_txt.SetText("lives 3/4");
            WineBg.DOFade(0, 2f).OnComplete(()=>{
                WineBg.gameObject.SetActive(false);
            });
            break;

            case TileType.Milk:
            FestivalTitle_txt.SetText("Cheese Festival");
            LifeNo_txt.SetText("lives 4/4");
            ChocolateBg.DOFade(0, 2f).OnComplete(()=>{
                ChocolateBg.gameObject.SetActive(false);
            });
            break;

            default:
            break;
        }
    }
}
