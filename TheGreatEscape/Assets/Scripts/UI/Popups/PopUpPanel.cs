using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpPanel : MonoBehaviour
{
    public GameObject NotEnoughMedallions, HomeConfirm, Victory, Dashboard;

    public List<GameObject> AllPanels;

    public TextMeshProUGUI username_txt, school_txt, coins_txt, medallions_txt;

    void Start()
    {

    }

    public void HideAllPanels()
    {
        for (int i = 0; i < AllPanels.Count; i++)
        {
            AllPanels[i].SetActive(false);
        }
    }

    public void ShowConfirmHome(bool _state)
    {
        HideAllPanels();
        HomeConfirm.SetActive(_state);
        gameObject.SetActive(_state);
        Time.timeScale = _state? 0:1;
    }

    public void ShowMedallion(bool _state)
    {
        HideAllPanels();
        NotEnoughMedallions.SetActive(_state);
        gameObject.SetActive(_state);
        Time.timeScale = _state? 0:1;
    }

    public void HomeClick()
    {
        ShowConfirmHome(false);
        LevelManager.instance.SetLevelFailedResult();
        GameManager.instance.MenuState = MenuState.GaveUp;
        GameManager.instance.LoadScene("Menu");
    }

    public void ShowVictoryPanel(bool _state)
    {
        HideAllPanels();
        Victory.SetActive(_state);
        gameObject.SetActive(_state);
        Time.timeScale = _state? 0:1;
    }

    public void VictoryConfirm()
    {
        ShowVictoryPanel(false);
        GameManager.instance.LoadScene("Menu");
    }

    public void ShowDashboard(bool _state)
    {
        HideAllPanels();
        gameObject.SetActive(_state);
        Dashboard.SetActive(_state);
        username_txt.SetText(Data.instance.UserName);
        school_txt.SetText(Data.instance.SchoolName);
        coins_txt.SetText(Data.instance.Coins.ToString());
        medallions_txt.SetText(Data.instance.MedallionCount.ToString());
        Time.timeScale = _state? 0:1;
    }
}
