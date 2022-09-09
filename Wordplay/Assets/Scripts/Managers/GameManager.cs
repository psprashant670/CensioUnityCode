using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Web;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool isLoaded = false;
    public Data Data;
    public MenuState MenuState; 
    public int TestingUID, LoadingCounter;
    public string Version;
    string token;

    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            MenuState = MenuState.Loading;
            Data.Init();
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Application.targetFrameRate = -1;
        Debug.Log(Version);
        #if UNITY_EDITOR
            Data.UID = TestingUID;
            Data.loadingCounter = LoadingCounter;
            Data.FetchUser_Co(this);
            // Data.GetSubliminalMeasurement1_Co(this);
        #endif
    }

    public void LoadParameters(string _url)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            // Uri myUri = new Uri(_url);
            // // Debug.Log("LoadParameters "+_url);
            // token = "";
            // token = HttpUtility.ParseQueryString(myUri.Query).Get("token");

            // // testing only
            // // string UID = HttpUtility.ParseQueryString(myUri.Query).Get("UID");
            // // if (!string.IsNullOrEmpty(UID))
            // // {
            // //     Data.UID = int.Parse(UID);
            // // }

            // if(!string.IsNullOrEmpty(token))
            // {
            //     Data.AuthToken = token;
            // }

            // Data.loadingCounter = 4;
            // Data.FetchUser_Co(this);
            // Data.GetAllDescriptions_Co(this);
            // Data.GetAllAttempts_Co(this);
            // Data.GetSubliminalMeasurement1_Co(this);
            StartCoroutine(WaitUntillReady(_url));
        #endif
    }

    public void PostUserAttempt()
    {
        Data.Game4Userlog_Co(this);
        Data.Game4WordsLog_Co(this);
    }

    public void BeginGameplay(SelectedLevel _level)
    {
        switch (_level)
        {
            case SelectedLevel.Level1:
            LoadScene("Level1");
            break;

            case SelectedLevel.Level2:
            LoadScene("Level2");
            break;
            
            case SelectedLevel.Level3:
            LoadScene("Level3");
            break;
            
            default:
            break;
        }
    }

    public void LoadScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }

    public void LoadSceneWithDelay(string _name, float _time)
    {
        StartCoroutine(LoadSceneWithDelayCo(_time, _name));
    }

    IEnumerator LoadSceneWithDelayCo(float _time, string _name)
    {
        yield return new WaitForSeconds(_time);
        LoadScene(_name);
    }

    IEnumerator WaitUntillReady(string _url)
    {
        yield return new WaitUntil(()=> instance!=null && MenuUI.instance!=null);
        Uri myUri = new Uri(_url);
        // Debug.Log("LoadParameters "+_url);
        token = "";
        token = HttpUtility.ParseQueryString(myUri.Query).Get("token");

        // testing only
        // string UID = HttpUtility.ParseQueryString(myUri.Query).Get("UID");
        // if (!string.IsNullOrEmpty(UID))
        // {
        //     Data.UID = int.Parse(UID);
        // }

        if(!string.IsNullOrEmpty(token))
        {
            Data.AuthToken = token;
        }

        Data.loadingCounter = LoadingCounter;
        Data.FetchUser_Co(this);
        // Data.GetSubliminalMeasurement1_Co(this);
    }
}

public enum MenuState
{
    Loading,
    Start,
    MoveOn,
    TimeUp,
    GameOver
}