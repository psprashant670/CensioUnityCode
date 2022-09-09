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
    public List<LevelData> Levels;
    public LevelData CurrentLevel { get { return GetCurrentLevel(); } }

    // public int Coins;
    public string AuthToken = "";
    public string UserName { get; set; }
    public string SchoolName { get; set; }

    string Domain = "https://censiobeta.in/CensioApi/api/";
    string CensioDomain = "https://app-api.censioanalytics.com/";
    const string FetchUserEndpoint = "api/v1/users/fetch_user";
    const string GetGameList = "GetGameList";
    const string GetSubliminalMeasurement1Endpoint = "GetSubliminal_Measurement1";
    const string GetUserGame2LevelStatus = "GetUserGame2LevelStatus"; // previous level and attempt data
    const string GetGame2Dashboard = "GetGame2Dashboard";
    const string GetGameGuideDialogueList = "GetGameGuideDialogueList"; // intro messages
    const string GetGameChallengeByGameId = "GetGameChallengeByGameId"; // level data
    const string GetGameAttemptData = "GetGameAttemptData"; // attempts data

    const string PostGamePlayLog = "GamePlayLog";
    const string PostUserAttempt = "Game2UserLog";
    const string PostGame2BlockAccuracyLog = "Game2BlockAccuracyLog";

    const string GetTotalGame1Coins = "GetTotalGame2Coins"; // unused, error in api, not needed

    const int idGame = 2, OrgID = 1;
    [HideInInspector]
    public string gameName = "SKYLINE";

    public int UID { get; set; }
    public int AccuracyRequired { get; set; }
    public int loadingCounter;
    public int RewardCoins, TotalCoins, AccuracyLevel;
    List<AttemptData> AttemptData;
    List<SubliminalMeasurement1Data> SubliminalMeasurement1Data;
    public List<string> IntroMessages;
    public List<Game2BlockAccuracyLog> Game2BlockAccuracyLogs;

    void OnEnable() 
    {
        instance = this;
    }

    public void Init()
    {
        UID = 0;
        
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
    }

    void UpdateLoadingCounter()
    {
        if (GameManager.isLoaded)
        {
            return;
        }
        
        loadingCounter--;
        float _per = Mathf.Abs(((float)loadingCounter - (float)GameManager.instance.LoadingCount)/(float)GameManager.instance.LoadingCount);
        MenuUI.instance?.SetLoading(Mathf.Clamp01(_per));
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

    LevelData GetCurrentLevel()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].SelectedLevel == SelectedLevel)
            {
                return Levels[i];
            }
        }
        return null;
    }

    public AttemptData GetGameAttemptDataValue(int _idLevel, int _attemptNo)
    {
        return AttemptData.Find( data => (data.idLevel == _idLevel && data.attemptNo == _attemptNo));
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

    public void GetGameGuideDialogueList_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameGuideDialogueListCo());
    }

    public void GetGameChallengeByGameId_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameChallengeByGameIdCo());
    }

    public void GetAllAttempts_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetAllAttemptsCo());
    }

    public void GetUserGame2LevelStatus_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetUserGame2LevelStatusCo());
    }

    public void GetGame2Dashboard_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame2DashboardCo());
    }

    public void GetSubliminalMeasurement1_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetSubliminalMeasurement1Co());
    }

    public void PostGamePlayLog_Co(MonoBehaviour caller, string _name)
    {
        caller.StartCoroutine(PostGamePlayLogCo(_name));
    }

    public void PostUserAttempt_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostUserAttemptCo());
    }

    public void PostGame2BlockAccuracyLog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostGame2BlockAccuracyLogCo());
    }

    IEnumerator FetchUserCo()
    {
        //TESTING ONLY
        // UserName = "Fake User";
        // SchoolName = "Fake School";
        // GetGameList_Co(GameManager.instance);
        // GetGame2Dashboard_Co(GameManager.instance);
        // GetUserGame2LevelStatus_Co(GameManager.instance);
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

            GetGameList_Co(GameManager.instance);
            GetGame2Dashboard_Co(GameManager.instance);
            GetUserGame2LevelStatus_Co(GameManager.instance);
            
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
            PostGamePlayLog_Co(GameManager.instance, gameName);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                PostGamePlayLog_Co(GameManager.instance, gameName);
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
                    AccuracyRequired = (int)temp["accuracyLimit"];
                }
            }
            PostGamePlayLog_Co(GameManager.instance, gameName);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }
    
    // get the intro
    IEnumerator GetGameGuideDialogueListCo()
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGameGuideDialogueList+"?IdGame="+idGame+"&OrgID="+OrgID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log("error GetGameGuideDialogueList");
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGameGuideDialogueList");
                UpdateLoadingCounter();
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            IntroMessages.Clear();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                if ((int?)temp["sequence"] != null)
                {
                    IntroMessages.Add((string)temp["guideDialogue"]);
                }
            }
            //TODO above
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGameChallengeByGameIdCo()
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGameChallengeByGameId+"?IdGame="+idGame+"&OrgID="+OrgID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            ShowAPIError(webRequest.error);
            Debug.Log("error GetGameGuideDialogueList");
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API GetGameGuideDialogueList");
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
                Levels[i].idLevel = (int)temp["idLevel"];
                Levels[i].attemptsAllowed = (int)temp["attemptsAllowed"];
                Levels[i].attemptTimer = (int)temp["attemptTimer"];
                Levels[i].challengeIntroMessage = (string)temp["challengeIntroMessage"];
                Levels[i].isPreviouslyComplete = false;
            }
            //TODO above
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetAllAttemptsCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGameAttemptData+"?idGame="+idGame+"&OrgID="+OrgID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
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
                ShowAPIError("No Data Sent in API "+GetGameAttemptData);
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);

            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            AttemptData = new List<AttemptData>();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                if ((int)temp["attemptNo"]>4)
                {
                    continue;
                }
                AttemptData _data = new AttemptData();
                _data.idAttempt = (int)temp["idAttempt"];
                _data.idLevel = (int)temp["idLevel"];
                _data.attemptNo = (int)temp["attemptNo"];
                _data.idGame = (int)temp["idGame"];
                _data.goldCoins = (int)temp["goldCoins"];
                _data.idBehavior = (int)temp["idBehavior"];
                // _data.game5Behaviors = (string)temp["game5Behaviors"];
                _data.behaviorScore = (float)temp["behaviorScore"];
                // _data.timeInSecond = (int)temp["timeInSecond"];
                // _data.challengeCompletedTime1 = (int)temp["challengeCompletedTime1"];
                _data.rewardCoinsTime1 = (int)temp["rewardCoinsTime1"];
                // _data.challengeCompletedTime2 = (int)temp["challengeCompletedTime2"];
                // _data.rewardCoinsTime2 = (int)temp["rewardCoinsTime2"];
                _data.failAttemptMessage = (string)temp["failAttemptMessage"];
                AttemptData.Add(_data);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetUserGame2LevelStatusCo()
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetUserGame2LevelStatus+"?IdGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            // ShowAPIError(webRequest.error);
            for (int i = 0; i < Levels.Count; i++)
            {
                Levels[i].isComplete = 0;
                Levels[i].attemptNumber = 0;
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                // ShowAPIError("No Data Sent in API GetGameGuideDialogueList");
                UpdateLoadingCounter();
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            for (int i = 0; i < Levels.Count; i++)
            {
                Levels[i].isComplete = 0;
                Levels[i].attemptNumber = 0;
            }
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                LevelData level = Levels.Find((lvl)=>lvl.idLevel == (int)temp["idLevel"]);
                level.isComplete = (int)temp["levelStatus"];
                level.attemptNumber = (int)temp["attempt_No"]-1;
            }

            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame2DashboardCo()
    {
        // Debug.Log(GetGame5API+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame2Dashboard+"?IdGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            // ShowAPIError(webRequest.error);
            for (int i = 0; i < Levels.Count; i++)
            {
                RewardCoins = 0;
                AccuracyLevel = 0;
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                // ShowAPIError("No Data Sent in API GetGameGuideDialogueList");
                for (int i = 0; i < Levels.Count; i++)
                {
                    RewardCoins = 0;
                    AccuracyLevel = 0;
                }
                UpdateLoadingCounter();
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JObject result = new JObject();
            result = JObject.Parse(json);
            Debug.Log("before "+RewardCoins);
            RewardCoins = (int)result["rewardCoins"];
            AccuracyLevel = (int)result["accuracy_Level"];
            Debug.Log("after "+RewardCoins);
            MenuUI.instance?.SetCoins();
            InGameUI.instance?.DashboardPanel?.UpdateData();
            UpdateLoadingCounter();
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

    IEnumerator PostGamePlayLogCo(string _name)
    {
        GamePlayLog req = new GamePlayLog();
        req.IdUser = UID;
        req.GameName = _name;
        // Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostGamePlayLog, "POST");
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

    IEnumerator PostUserAttemptCo()
    {
        UserData res = new UserData();

        res.IdUser = UID;
        res.IdGame = idGame;
        res.IdLevel = CurrentLevel.idLevel;
        res.IdSubliminalMeasurement1 = CurrentLevel.idSubliminalMeasurement1;
        res.IdSubliminalMeasurement2 = 0;
        res.IdBehavior = CurrentLevel.idBehavior;
        res.BehaviorScore = CurrentLevel.behaviorScore;
        res.AccuracyLevel = CurrentLevel.accuracy;
        res.RewardCoins = CurrentLevel.rewardCoins;
        res.TimetakenToComplete = CurrentLevel.timeTaken;
        res.AttemptNo = CurrentLevel.attemptNumber;
        res.IsCompleted = (int)Mathf.Clamp01(CurrentLevel.isComplete);
        res.UpdatedDateTime = DateTime.Now;
        res.IdOrganization = OrgID;
        res.BlocksPresented = CurrentLevel.blocksPresented;
        res.BlocksClicked = CurrentLevel.blocksClicked;
        res.CorrectBlocks = CurrentLevel.correctBlocks;
        res.SuccessfulBlocks = CurrentLevel.successfulBlocks;

        // Debug.Log(JsonConvert.SerializeObject(res));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostUserAttempt, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(res));
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
            GetGame2Dashboard_Co(GameManager.instance);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostGame2BlockAccuracyLogCo()
    {

        // Debug.Log(JsonConvert.SerializeObject(Game2BlockAccuracyLogs));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostGame2BlockAccuracyLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(Game2BlockAccuracyLogs));
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
}