using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System;

[CreateAssetMenu]
public class Data : ScriptableObject
{
    public static Data instance;

    public bool IsCustomBuild = false, isStagingDomain = false;
    public SelectedLevel SelectedLevel;
    public LevelData CurrentLevel;
    public int LastLevelID = 0;

    // public int Coins;
    public string AuthToken = "";
    public string UserName { get; set; }
    public string SchoolName { get; set; }

    string Domain = "https://censiobeta.in/CensioApi/api/";
    string CensioDomain = "https://app-api.censioanalytics.com/";
    const string FetchUserEndpoint = "api/v1/users/fetch_user";
    const string GetSubliminalMeasurement1Endpoint = "GetSubliminal_Measurement1";
    const string GetGame4UserLevel = "GetGame4UserLevel";
    const string GetGame4LevelInstructions = "GetGame4LevelInstructions";
    const string GetGame4LevelInfo = "GetGame4LevelInfo";
    const string GetGame4WordMaster = "GetGame4WordMaster";

    const string Game4Userlog = "Game4Userlog";
    const string Game4WordsLog = "Game4WordsLog";
    const string GamePlayLog = "GamePlayLog";
    const string GetGameList = "GetGameList";

    const int idGame = 4, OrgID = 1;
    string gameName = "WordPlay";

    public int UID { get; set; }
    public int loadingCounter;
    List<SubliminalMeasurement1Data> SubliminalMeasurement1Data;

    void OnEnable() 
    {
        instance = this;
    }

    public void Init()
    {
        UID = -1;
        LastLevelID = 0;

        if (isStagingDomain)
        {
            Domain = "https://censiostaging.in/CensioApi/api/";
            CensioDomain = "https://app-api.staging.censioanalytics.com/";
        }
        else
        {
            Domain = "https://censiobeta.in/CensioApi/api/";
            CensioDomain = "https://app-api.censioanalytics.com/";
        }
        
        if (CurrentLevel==null)
        {
            return;
        }
        if (CurrentLevel.UserWords.Count > 0)
        {
            CurrentLevel.UserWords.Clear();
        }
        if (CurrentLevel.CardWords.Count > 0)
        {
            CurrentLevel.CardWords.Clear();
        }
    }

    void UpdateLoadingCounter()
    {
        loadingCounter--;
        if (loadingCounter <= 0)
        {
            GameManager.isLoaded = true;
            GameManager.instance.MenuState = MenuState.Start;
        }
    }

    void ShowAPIError(string _msg)
    {
        MenuUI.instance?.ShowError(_msg);
    }

    public SubliminalMeasurement1Data GetSubliminalMeasurement1Data(string _name)
    {
        return SubliminalMeasurement1Data.Find( data => data.subliminalMeasurementName.Equals(_name) );
    }

    public void FetchUser_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(FetchUserCo());
    }

    public void GetGameList_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameListCo());
    }

    public void GetGame4UserLevel_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame4UserLevelCo());
    }

    public void GetGame4LevelInstructions_Co(MonoBehaviour caller, int _levelID)
    {
        caller.StartCoroutine(GetGame4LevelInstructionsCo(_levelID));
    }

    public void GetGame4LevelInfo_Co(MonoBehaviour caller, int _levelID)
    {
        caller.StartCoroutine(GetGame4LevelInfoCo(_levelID));
    }

    public void GetGame4WordMaster_Co(MonoBehaviour caller, int _levelID)
    {
        caller.StartCoroutine(GetGame4WordMasterCo(_levelID));
    }

    public void GetSubliminalMeasurement1_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetSubliminalMeasurement1Co());
    }

    public void Game4Userlog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(Game4UserlogCo());
    }
    
    public void Game4WordsLog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(Game4WordsLogCo());
    }

    public void GamePlayLog_Co(MonoBehaviour caller, string _name)
    {
        caller.StartCoroutine(GamePlayLogCo(_name));
    }

    public void VerifyWord_Co(MonoBehaviour caller, string _word, Action<bool, string> _callback)
    {
        caller.StartCoroutine(VerifyWordCo(_word, _callback));
    }

    IEnumerator FetchUserCo()
    {
        //TESTING ONLY
        // UserName = "Fake User";
        // SchoolName = "Fake School";
        // // GetUserCoins_Co(GameManager.instance);
        // GetGameList_Co(GameManager.instance);
        // GetGame4UserLevel_Co(GameManager.instance);
        // UpdateLoadingCounter();
        // yield break;
        // --------------------------------------------------------------------------------------------------

        UnityWebRequest webRequest = new UnityWebRequest( CensioDomain+FetchUserEndpoint, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", AuthToken);
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            ShowAPIError(webRequest.error);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API FetchUser");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);

            JObject result = new JObject();
            result = JObject.Parse(json);
            UID = (int)result.SelectToken("data").SelectToken("user").SelectToken("id");
            UserName = (string)result.SelectToken("data").SelectToken("user").SelectToken("username");
            SchoolName = (string)result.SelectToken("data").SelectToken("user").SelectToken("school_name");
            // GetUserCoins_Co(GameManager.instance);
            GetGameList_Co(GameManager.instance);
            GetGame4UserLevel_Co(GameManager.instance);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGameListCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGameList+"?OrgID="+OrgID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            GamePlayLog_Co(GameManager.instance, gameName);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                GamePlayLog_Co(GameManager.instance, gameName);
                UpdateLoadingCounter();
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                int _idGame = (int)temp["idGame"];
                if (_idGame == idGame)
                {
                    gameName = (string)temp["gameName"];
                }
            }
            GamePlayLog_Co(GameManager.instance, gameName);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    // gets the latest level 1 2 or 3
    IEnumerator GetGame4UserLevelCo()
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame4UserLevel+"?Gameid="+idGame+"&UserId="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            // ShowAPIError(webRequest.error);
            Debug.Log("error GetGame4UserLevelCo");
            LastLevelID = 0;
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGame4UserLevel");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            LastLevelID = 0;
            JObject temp = new JObject();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                int id = (int)temp["idLevelMasterid"];
                if (LastLevelID<id)
                {
                    LastLevelID = id;
                }
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }
    
    // get the description for level x
    IEnumerator GetGame4LevelInstructionsCo(int _levelID)
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame4LevelInstructions+"?Gameid="+idGame+"&Levelid="+_levelID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log("error GetGame4LevelInstructions");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGame4LevelInstructions");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);

            JObject temp = new JObject();
            string description = "", newLine = "<br><br>";
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                string _string = (string)temp["instructionDetail"];
                if (i==0)
                {
                    description += "<b>";
                    description += (_string+newLine);
                    description += "</b>";
                }
                else
                {
                    description += (_string+newLine);
                }
            }
            MenuUI.instance.SetDescription(description);
            MenuUI.instance.HideAllPanels();
            MenuUI.instance.DescriptionPanel.SetActive(true);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame4LevelInfoCo(int _levelID)
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame4LevelInfo+"?Gameid="+idGame+"&Levelid="+_levelID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log("error GetGame4LevelInfo");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGame4LevelInfo");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);

            JObject temp = new JObject();
            temp = (JObject)result[0];
            CurrentLevel.idLevel = (int)temp["idLevel"];
            CurrentLevel.levelName = (string)temp["levelName"];
            CurrentLevel.levelWordcount = (int)temp["levelWordcount"];
            CurrentLevel.levelTimerequired = (int)temp["levelTimerequired"];
            CurrentLevel.levelBonuspts = (int)temp["levelBonuspts"];
            GetGame4WordMaster_Co(GameManager.instance, _levelID);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame4WordMasterCo(int _levelID)
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame4WordMaster+"?Gameid="+idGame+"&Levelid="+_levelID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log("error GetGame4WordMaster");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGame4WordMaster");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);

            JObject temp = new JObject();
            CurrentLevel.CardWords.Clear();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                CurrentLevel.CardWords.Add((string)temp["idWordDetail"]);
            }
            GameManager.instance.BeginGameplay(SelectedLevel);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetSubliminalMeasurement1Co()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetSubliminalMeasurement1Endpoint+"?idGame="+idGame+"&OrgID="+OrgID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log(webRequest.error);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetSubliminalMeasurement1Endpoint);
                webRequest.Dispose();
                yield break;
            }

            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            SubliminalMeasurement1Data = new List<SubliminalMeasurement1Data>();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                SubliminalMeasurement1Data _data = new SubliminalMeasurement1Data();
                _data.idSubliminalMeasurement1 = (int)temp["idSubliminalMeasurement1"];
                _data.idGame = (int)temp["idGame"];
                _data.idBehavior = (int)temp["idBehavior"];
                _data.subliminalMeasurementName = (string)temp["subliminalMeasurementName"];
                _data.behaviorScore = (int)temp["behaviorScore"];
                _data.idOrganization = (int)temp["idOrganization"];
                _data.idOrganization = (int)temp["idOrganization"];
                _data.status = (string)temp["status"];
                SubliminalMeasurement1Data.Add(_data);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator Game4UserlogCo()
    {
        //TODO: Parameterize
        Game4Userlog req = new Game4Userlog();
        LevelData Current = CurrentLevel;
        req.IdUser = UID;
        req.IdGame = idGame;
        req.BonusPoint = CurrentLevel.bonusGranted;
        req.CountofWordsMade = CurrentLevel.userWordsGenerated;
        req.IdLevelMasterid = CurrentLevel.idLevel;
        req.TimeTaken = CurrentLevel.timeTaken;
        req.WordSelected = CurrentLevel.WordSelected;

        Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+Game4Userlog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(req));
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            Debug.Log(webRequest.downloadHandler.text);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("Game4UserlogCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator Game4WordsLogCo()
    {
        //TODO: Parameterize
        Game4WordsLog req = new Game4WordsLog();
        LevelData Current = CurrentLevel;
        req.IdUser = UID;
        req.IdGame = idGame;
        req.IdLevel = CurrentLevel.idLevel;
        req.WordSelected = CurrentLevel.WordSelected;
        req.wordmadedetail = "";
        for (int i = 0; i < CurrentLevel.UserWords.Count; i++)
        {
            if (i<CurrentLevel.UserWords.Count-1)
            {
                req.wordmadedetail += (CurrentLevel.UserWords[i]+",");
            }
            else
            {
                req.wordmadedetail += (CurrentLevel.UserWords[i]);
            }
        }
        Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+Game4WordsLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(req));
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            Debug.Log(webRequest.downloadHandler.text);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("Game4WordsLogCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GamePlayLogCo(string _name)
    {
        GamePlayLog req = new GamePlayLog();
        req.IdUser = UID;
        req.GameName = _name;
        // Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+GamePlayLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(req));
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            // string json = webRequest.downloadHandler.text;
            // if (string.IsNullOrEmpty(json))
            // {
            //     yield break;
            // }
            // Debug.Log(json);
            // Debug.Log("Game4WordsLogCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator VerifyWordCo(string _word, Action<bool, string> callback)
    {
        UnityWebRequest webRequest = new UnityWebRequest( "https://api.dictionaryapi.dev/api/v2/entries/en/"+_word, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
        webRequest.timeout = 5;
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("VerifyWord error");
            callback(true, _word);
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            if (webRequest.responseCode == 200)
            {
                callback(true, _word);
            }
            else if (webRequest.responseCode == 404)
            {
                callback(false, _word);
            }
            webRequest.Dispose();
            yield break;
        } 
    }
}