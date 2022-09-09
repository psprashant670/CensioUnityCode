using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratzPanel : MonoBehaviour
{
    public GameObject Cart, Truck, Cafe, FineDine;
    public int unlockedLevel = 0, lastShown = 0;

    public void ShowPanel()
    {
        // Debug.Log(this);
        Cart.SetActive(false);
        Truck.SetActive(false);
        Cafe.SetActive(false);
        FineDine.SetActive(false);

        if (unlockedLevel==1 && lastShown!=unlockedLevel)
        {
            lastShown = unlockedLevel;
            Cart.SetActive(true);
        }
        else if (unlockedLevel==2 && lastShown!=unlockedLevel)
        {
            lastShown = unlockedLevel;
            Truck.SetActive(true);
        }
        else if (unlockedLevel==3 && lastShown!=unlockedLevel)
        {
            lastShown = unlockedLevel;
            Cafe.SetActive(true);
        }
        else if (unlockedLevel==4 && lastShown!=unlockedLevel)
        {
            lastShown = unlockedLevel;
            FineDine.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
            OKClick();
            return;
        }

        // switch (LevelManager.instance.CurrentFestival)
        // {
        //     case TileType.Wheat:
        //     Cart.SetActive(true);
        //     break;

        //     case TileType.Grape:
        //     Truck.SetActive(true);
        //     break;

        //     case TileType.Chocolate:
        //     Cafe.SetActive(true);
        //     break;

        //     case TileType.Milk:
        //     FineDine.SetActive(true);
        //     break;
        // }

        gameObject.SetActive(true);
    }

    public void OKClick()
    {
        gameObject.SetActive(false);
        switch (LevelManager.instance.CurrentFestival)
        {
            case TileType.Wheat:
            // LevelManager.instance.BeginFestival(TileType.Grape);
            InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Grape, Data.instance.GrapeFest.timeRequiredSeconds);
            break;

            case TileType.Grape:
            // LevelManager.instance.BeginFestival(TileType.Chocolate);
            InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Chocolate, Data.instance.ChocolateFest.timeRequiredSeconds);
            break;

            case TileType.Chocolate:
            // LevelManager.instance.BeginFestival(TileType.Milk);
            InGameUI.instance.NewFestivalPanel.ShowPanel(TileType.Milk, Data.instance.MilkFest.timeRequiredSeconds);
            break;

            case TileType.Milk:
            LevelManager.instance.FinishFestivals();
            break;
        }
    }
}
