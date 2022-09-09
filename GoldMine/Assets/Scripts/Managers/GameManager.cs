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
    public int TestingUID, LoadingCount;
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
        #if UNITY_EDITOR
            Data.UID = TestingUID;
            Data.loadingCounter = LoadingCount;
            Data.FetchUser_Co(this);
            Data.GetAllDescriptions_Co(this);
            Data.GetAllAttempts_Co(this);
            Data.GetSubliminalMeasurement1_Co(this);
            Data.GetSubliminalMeasurement2_Co(this, 2);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("VERSION "+Version);
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

            // Data.loadingCounter = 5;
            // Data.FetchUser_Co(this);
            // Data.GetAllDescriptions_Co(this);
            // Data.GetAllAttempts_Co(this);
            // Data.GetSubliminalMeasurement1_Co(this);
            StartCoroutine(WaitUntillReady(_url));
        #endif
    }

    public void PostUserAttempt()
    {
        Data.PostUserAttempt_Co(this);
        if (Data.SelectedLevel==Level.Level3 || Data.SelectedLevel==Level.Level4)
        {
            Data.instance.PostGame1CorrectTilesLog_Co(this);
        }
    }

    public void PostUserPurchase()
    {
        Data.PostUserPurchase_Co(this);
    }

    public void LoadScene(string _name)
    {
        SceneManager.LoadScene(_name);
    }

    public void LoadSceneWithDelay(string _name, float _time)
    {
        StartCoroutine(LoadSceneWithDelayCo(_time, _name));
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        Data.loadingCounter = LoadingCount;
        Data.FetchUser_Co(this);
        Data.GetAllDescriptions_Co(this);
        Data.GetAllAttempts_Co(this);
        Data.GetSubliminalMeasurement1_Co(this);
        Data.GetSubliminalMeasurement2_Co(this, 2);
    }
}

public enum MenuState
{
    Loading,
    Start,
    Success,
    TimeRanOut,
    Dead,
    Failed,
    GaveUp,
    LevelSelect
}