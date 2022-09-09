using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public static MenuUI instance;
    public List<GameObject> AllHomePanels;
    public GameObject LoadingPanel, PlayPanel, LevelSelectPanel, DescriptionPanel, PopupPanel;
    public MenuPopupPanel MenuPopupPanel;
    public Data Data;
    public TextMeshProUGUI Description_txt;
    public ErrorPanel ErrorPanel;

    void Awake() 
    {
        instance = this;
    }

    void Start()
    {
        HideAllPanels();
        LoadingPanel.SetActive(true);
        StartCoroutine(WaitTillLoaded());
        ErrorPanel.gameObject.SetActive(false);
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

    }

    public void ShowError(string _msg)
    {
        Debug.Log(_msg);
        ErrorPanel.ShowError(_msg);
    }

    public void SetDescription(string _desc)
    {
        Description_txt.SetText(_desc);
    }

    public void BeginLevel()
    {
        HideAllPanels();
        LoadingPanel.SetActive(true);
        GameManager.instance.Data.GetGame4LevelInfo_Co(GameManager.instance, (int)GameManager.instance.Data.SelectedLevel);
    }

    IEnumerator WaitTillLoaded()
    {
        yield return new WaitUntil(()=> GameManager.isLoaded );
        HideAllPanels();
        if (GameManager.instance.MenuState == MenuState.Start)
        {
            PlayPanel.SetActive(true);
        }
        else
        {
            LevelSelectPanel.SetActive(true);

            if (GameManager.instance.MenuState == MenuState.MoveOn)
            {
                MenuPopupPanel.ShowVictory(true);
            }
            else if (GameManager.instance.MenuState == MenuState.TimeUp)
            {
                MenuPopupPanel.ShowTimeUp(true);
            }
            else if (GameManager.instance.MenuState == MenuState.GameOver)
            {
                MenuPopupPanel.ShowGameOver(true);
            }
        }
    }

}
