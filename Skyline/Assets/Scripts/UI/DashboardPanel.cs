using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardPanel : MonoBehaviour
{
    public TextMeshProUGUI Accuracy_txt, RewardCoins_txt;
    
    public void ShowPanel(bool _state)
    {
        UpdateData();
        gameObject.SetActive(_state);
        Time.timeScale = _state ? 0 : 1;
    }

    public void UpdateData()
    {
        Accuracy_txt.SetText(Data.instance.AccuracyLevel+"%");
        RewardCoins_txt.SetText(Data.instance.RewardCoins.ToString());
    }
}
