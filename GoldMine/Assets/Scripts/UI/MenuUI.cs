using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;
    public List<GameObject> AllHomePanels;
    public GameObject LoadingPanel, StartPanel, EndScreensPanel;
    public MenuErrorPanel MenuErrorPanel;
    public DashboardPanel DashboardPanel;
    public LevelSelect LevelSelectPanel;
    public TextMeshProUGUI Coins_txt;
    public Image LoadingFill;

    public Data Data;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        Data.GetUserCoins_Co(GameManager.instance);
        Data.GetGame1Dashboard_Co(GameManager.instance);
        
        HideAllPanels();
        MenuErrorPanel.gameObject.SetActive(false);
        if (GameManager.instance.MenuState == MenuState.Loading)
        {
            LoadingPanel.SetActive(true);
        }
        else if (GameManager.instance.MenuState == MenuState.Start)
        {
            StartPanel.SetActive(true);
        }
        else if (GameManager.instance.MenuState == MenuState.LevelSelect)
        {
            LevelSelectPanel.SetData();
            LevelSelectPanel.gameObject.SetActive(true);
        }
        
        if (!GameManager.isLoaded)
        {
            StartCoroutine(WaitForLoad());
        }
        else
        {
            Coins_txt.SetText(Data.TotalCoins.ToString());
        }
    }

    public void HideAllPanels()
    {
        foreach (GameObject _panel in AllHomePanels)
        {
            _panel.SetActive(false);
        }
    }

    public void SetCoins()
    {
        Coins_txt.SetText(Data.TotalCoins.ToString());
    }

    public void ShowError(string _msg)
    {
        MenuErrorPanel.ShowError(_msg);
    }

    public void SetLoading(float _per)
    {
        LoadingFill.fillAmount = _per;
    }

    public void ShowDashboard(bool _state)
    {
        DashboardPanel.gameObject.SetActive(_state);
    }

    public void ShowGameOver()
    {
        EndScreensPanel.SetActive(true);
    }

    public void PlayLevel()
    {
        Debug.Log(Data.SelectedLevel);
        switch (Data.SelectedLevel)
        {
            case Level.Level1:
            GameManager.instance.LoadScene("Level1");
            break;
            case Level.Level2:
            GameManager.instance.LoadScene("Level2");
            break;
            case Level.Level3:
            GameManager.instance.LoadScene("Level3");
            break;
            case Level.Level4:
            GameManager.instance.LoadScene("Level4");
            break;
            default:
            break;
        }
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitUntil( ()=> GameManager.isLoaded );
        foreach (GameObject _panel in AllHomePanels)
        {
            _panel.SetActive(false);
        }
        StartPanel.SetActive(true);
        Coins_txt.SetText(Data.TotalCoins.ToString());
    }
}
