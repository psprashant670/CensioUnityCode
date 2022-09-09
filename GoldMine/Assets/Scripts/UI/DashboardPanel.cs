using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardPanel : MonoBehaviour
{
    public TextMeshProUGUI Username_txt, SchoolName_txt, Coins_txt, RewardCoins_txt;

    void OnEnable() 
    {
        Data.instance.GetGame1Dashboard_Co(GameManager.instance);
        Username_txt.SetText(Data.instance.UserName);
        SchoolName_txt.SetText(Data.instance.SchoolName);
        Coins_txt.SetText(Data.instance.Coins.ToString());
        RewardCoins_txt.SetText(Data.instance.RewardCoins.ToString());
    }

    public void SetData()
    {
        Username_txt.SetText(Data.instance.UserName);
        SchoolName_txt.SetText(Data.instance.SchoolName);
        Coins_txt.SetText(Data.instance.Coins.ToString());
        RewardCoins_txt.SetText(Data.instance.RewardCoins.ToString());
    }
}
