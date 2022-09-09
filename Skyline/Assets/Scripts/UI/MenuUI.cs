using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;

    public List<GameObject> AllHomePanels;
    public GameObject LoadingPanel, StartPanel, GameOverPanel;
    public MenuErrorPanel MenuErrorPanel;
    public DashboardPanel DashboardPanel;
    public IntroPanel IntroPanel;
    public LevelIntroPanel LevelIntroPanel;
    public LevelSelect LevelSelectPanel;
    public TextMeshProUGUI Coins_txt;
    public Image LoadingFill;

    public Data Data;

    private void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        // Data.GetUserCoins_Co(GameManager.instance);
        // Data.GetGame2Dashboard_Co(GameManager.instance);

        HideAllPanels();

        if (GameManager.instance.MenuState == MenuState.Loading)
        {
            LoadingPanel.SetActive(true);
            StartPanel.SetActive(false);
        }
        else if (GameManager.instance.MenuState == MenuState.Start)
        {
            StartPanel.SetActive(true);
            LoadingPanel.SetActive(false);
        }
        else if (GameManager.instance.MenuState == MenuState.LevelSelect)
        {
            StartPanel.SetActive(false);
            LoadingPanel.SetActive(false);
            LevelSelectPanel.SetData();
            LevelSelectPanel.gameObject.SetActive(true);
        }
        
        if (!GameManager.isLoaded)
        {
            StartCoroutine(WaitForLoad());
        }
        else
        {
            // Coins_txt.SetText(Data.TotalCoins.ToString());
        }
    }

    public void HideAllPanels()
    {
        foreach (GameObject _panel in AllHomePanels)
        {
            _panel.SetActive(false);
        }
    }

    public void ShowError(string _msg)
    {
        MenuErrorPanel.ShowError(_msg);
    }

    public void ShowGameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void SetCoins()
    {
        DashboardPanel.UpdateData();
    }

    public void SetLoading(float _per)
    {
        LoadingFill.fillAmount = _per;
    }

    public void PlayLevel()
    {
        // Debug.Log(Data.SelectedLevel);
        switch (Data.SelectedLevel)
        {
            case SelectedLevel.Level1:
            GameManager.instance.LoadScene("Level1");
            break;
            case SelectedLevel.Level2:
            GameManager.instance.LoadScene("Level2");
            break;
            case SelectedLevel.Level3:
            GameManager.instance.LoadScene("Level3");
            break;
            case SelectedLevel.Level4:
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
        LoadingPanel.SetActive(false);
        // Coins_txt.SetText(Data.TotalCoins.ToString());
    }
}
