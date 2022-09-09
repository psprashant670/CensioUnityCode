using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;

    public List<GameObject> AllPanels;

    public GameObject LoadingPanel, StartPanel, GameOverPanel;
    public ErrorPanel ErrorPanel;

    private void Awake() 
    {
        instance = this;
    }

    void Start()
    {
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
            // LevelSelectPanel.SetData();
            // LevelSelectPanel.gameObject.SetActive(true);
        }
        
        if (!GameManager.isLoaded)
        {
            StartCoroutine(WaitForLoad());
        }
        else
        {
            StartPanel.SetActive(true);
            LoadingPanel.SetActive(false);
            // Coins_txt.SetText(Data.TotalCoins.ToString());
        }
    }

    public void SetLoading()
    {

    }

    public void Begin()
    {
        GameManager.instance.LoadScene("Game");
    }

    public void ShowError(string _msg)
    {
        ErrorPanel.ShowError(_msg);
    }

    public void ShowGameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void HideAllPanels()
    {
        foreach (GameObject _panel in AllPanels)
        {
            _panel.SetActive(false);
        }
    }

    IEnumerator WaitForLoad()
    {
        yield return new WaitUntil( ()=> GameManager.isLoaded );
        foreach (GameObject _panel in AllPanels)
        {
            _panel.SetActive(false);
        }
        StartPanel.SetActive(true);
        LoadingPanel.SetActive(false);
        // Coins_txt.SetText(Data.TotalCoins.ToString());
    }
}
