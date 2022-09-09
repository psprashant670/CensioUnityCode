using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPanel : MonoBehaviour
{
    public GameObject NotEnoughCoins, Error, Dashboard, GameOver;
    public List<GameObject> Panels;
    public TextMeshProUGUI errorMsg_txt;
    public TextMeshProUGUI username_txt, school_txt, coins_txt, medallions_txt;
    
    public void HideAll()
    {
        foreach (GameObject _panel in Panels)
        {
            _panel.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void ShowError(string _msg)
    {
        HideAll();
        gameObject.SetActive(true);
        Error.SetActive(true);
        errorMsg_txt.SetText(_msg);
    }

    public void ShowCoinsError()
    {
        HideAll();
        gameObject.SetActive(true);
        NotEnoughCoins.SetActive(true);
    }

    public void ShowDashboard()
    {
        HideAll();
        gameObject.SetActive(true);
        Dashboard.SetActive(true);
        username_txt.SetText(Data.instance.UserName);
        school_txt.SetText(Data.instance.SchoolName);
        coins_txt.SetText(Data.instance.Coins.ToString());
        medallions_txt.SetText(Data.instance.MedallionCount.ToString());
    }

    public void ShowGameOver()
    {
        HideAll();
        gameObject.SetActive(true);
        GameOver.SetActive(true);
    }
}
