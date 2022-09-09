using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewFestivalPanel : MonoBehaviour
{
    public GameObject Bread, Wine, Chocolate, Cheese;
    public TextMeshProUGUI Time_txt;

    TileType nextFest;

    public void ShowPanel(TileType _festival, float _time)
    {
        nextFest = _festival;
        Bread.SetActive(false);
        Wine.SetActive(false);
        Chocolate.SetActive(false);
        Cheese.SetActive(false);
        Time_txt.SetText("You have "+Mathf.RoundToInt(_time)+" seconds to complete <br>the festival");
        switch (_festival)
        {
            case TileType.Wheat:
            Bread.SetActive(true);
            break;

            case TileType.Grape:
            Wine.SetActive(true);
            break;

            case TileType.Chocolate:
            Chocolate.SetActive(true);
            break;

            case TileType.Milk:
            Cheese.SetActive(true);
            break;

            default:
            break;
        }
        
        gameObject.SetActive(true);
    }

    public void OKClick()
    {
        LevelManager.instance.BeginFestival(nextFest);
        gameObject.SetActive(false);
    }

    public void ShowDashboard()
    {
        InGameUI.instance.DashboardPanel.ShowPanel(true);
    }
}
