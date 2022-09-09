using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashboardFestivalProgress : MonoBehaviour
{
    public TileType FestivalType;
    public Transform Wheat, Grape, Chocolate, Milk;
    public Image ProgressImage;
    public Sprite Cart, Truck, Cafe, Diner;
    public GameObject Blocker;

    public void SetProgress()
    {
        FestivalData festivalData = Data.instance.Festivals.Find((fest) => fest.FestivalType == FestivalType);
        ProgressImage.gameObject.SetActive(true);
        if (festivalData.FestivalProgress==TileType.Wheat)
        {
            ProgressImage.sprite = Cart;
        }
        else if (festivalData.FestivalProgress==TileType.Grape)
        {
            ProgressImage.sprite = Truck;
        }
        else if (festivalData.FestivalProgress==TileType.Chocolate)
        {
            ProgressImage.sprite = Cafe;
        }
        else if (festivalData.FestivalProgress==TileType.Milk)
        {
            ProgressImage.sprite = Diner;
        }
        else
        {
            ProgressImage.gameObject.SetActive(false);
        }
        
        SetBar(Wheat, Mathf.Clamp(festivalData.Wheat, 0, festivalData.MaxWheat)/100f);
        SetBar(Grape, Mathf.Clamp(festivalData.Grape, 0, festivalData.MaxGrape)/100f);
        SetBar(Chocolate, Mathf.Clamp(festivalData.Chocolate, 0, festivalData.MaxChocolate)/100f);
        SetBar(Milk, Mathf.Clamp(festivalData.Milk, 0, festivalData.MaxMilk)/100f);
    }
    
    void SetBar(Transform _Fill, float _per)
    {
        foreach (Transform _child in _Fill)
        {
            _child.gameObject.SetActive(false);
        }

        _per = Mathf.Clamp01(_per);
        int amount = Mathf.RoundToInt(_Fill.childCount*_per);
        for (int i = 0; i < amount; i++)
        {
            _Fill.GetChild(i).gameObject.SetActive(true);
        }

        _Fill.GetChild(_Fill.childCount-1).gameObject.SetActive((_per >= 0.99f));
    }
}
