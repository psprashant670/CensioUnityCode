using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu]
public class Data : ScriptableObject
{
    public static Data instance;

    public bool IsCustomBuild = false, isStagingDomain = false;
    public Level SelectedLevel;
    public List<LevelData> Levels;

    public int Coins, RewardCoins, TotalCoins;
    public string AuthToken = "";
    public float Timer { get { return GetTimer(); } }
    public int MaxAttempts { get { return GetMaxAttempts(); } }
    public LevelData CurrentLevel { get { return GetCurrentLevel(); } }
    public string UserName { get; set; }
    public string SchoolName { get; set; }

    string Domain = "https://censiobeta.in/CensioApi/api/";
    string CensioDomain = "https://app-api.censioanalytics.com/";
    const string FetchUserEndpoint = "api/v1/users/fetch_user";
    const string GetLevelDataEndpoint = "GetGameChallengeByGameId";
    const string GetAllAttemptsEndpoint = "GetGameAttemptData";
    const string GetUserCoinsEndpoint = "GetTotalGame1Coins";
    const string GetSubliminalMeasurement1Endpoint = "GetSubliminal_Measurement1";
    const string GetSubliminal_Measurement2Endpoint = "GetSubliminal_Measurement2";
    const string GetGame1AttempNumberEndpoint = "GetGame1AttempNumber";
    const string PostUserAttempt = "Game1UserLog";
    const string PostUserPurchase = "GameUserAttemptPurchaseLog";

    const string GetGame1Dashboard = "GetGame1Dashboard";
    const string GetUserGame1LevelStatus = "GetUserGame1LevelStatus";
    const string GamePlayLog = "GamePlayLog";
    const string GetGameList = "GetGameList";
    const string Game1CorrectTilesLog = "Game1CorrectTilesLog";

    const int idGame = 1, OrgID = 1;

    public string gameName = "The Gold Mine Treasure Map";
    public int UID { get; set; }
    public int loadingCounter;
    List<AttemptData> AttemptData;
    List<SubliminalMeasurement1Data> SubliminalMeasurement1Data;
    public List<SubliminalMeasurement2Data> SubliminalMeasurement2Data;
    public PreviousLevelData PreviousLevelData { get; set; }
    public List<Game1CorrectTilesLog> TilesList;

    void OnEnable() 
    {
        instance = this;
    }

    public void Init()
    {
        Coins = 0;
        UID = -1;

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

        if (TilesList==null)
        {
            TilesList = new List<Game1CorrectTilesLog>();
        }

        if (Levels == null)
        {
            Levels = new List<LevelData>();
            for (int i = 0; i < 4; i++)
            {
                Levels.Add(new LevelData(i));
            }
        }

        foreach (LevelData _level in Levels)
        {
            _level.isComplete = 0;
            _level.timeTaken = 0;
        }
    }

    int GetMaxAttempts()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].level == SelectedLevel)
            {
                return Levels[i].attemptsAllowed;
            }
        }

        return 0;
    }

    float GetTimer()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].level == SelectedLevel)
            {
                return Levels[i].attemptTimer;
            }
        }

        return 120f;
    }

    void UpdateLoadingCounter()
    {
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
        Debug.Log(_msg);
        MenuUI.instance?.ShowError(_msg);
    }

    LevelData GetCurrentLevel()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].level == SelectedLevel)
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

    public LevelData GetCurrentLevelFor(Level _Level)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].level == _Level)
            {
                return Levels[i];
            }
        }
        return null;
    }

    public void GetAllDescriptions_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetAllDescriptionsCo());
    }

    public void GetUserGame1LevelStatus_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetUserGame1LevelStatusCo());
    }

    public void GetGame1Dashboard_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame1DashboardCo());
    }

    public void GetAllAttempts_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetAllAttemptsCo());
    }

    public void FetchUser_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(FetchUserCo());
    }

    public void GetUserCoins_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetUserCoinsCo());
    }

    public void GetSubliminalMeasurement1_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetSubliminalMeasurement1Co());
    }

    public void GetSubliminalMeasurement2_Co(MonoBehaviour caller, int _level)
    {
        caller.StartCoroutine(GetSubliminalMeasurement2Co(_level));
    }

    public void PostUserAttempt_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostUserAttemptCo());
    }

    public void PostUserFeedback_Co(MonoBehaviour caller, string _feedbackResult)
    {
        caller.StartCoroutine(PostUserFeedbackCo(_feedbackResult));
    }

    public void PostUserPurchase_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostUserPurchaseCo());
    }

    public void PostGame1CorrectTilesLog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostGame1CorrectTilesLogCo());
    }

    public void GamePlayLog_Co(MonoBehaviour caller, string _name)
    {
        caller.StartCoroutine(GamePlayLogCo(_name));
    }

    public void GetGameList_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameListCo());
    }
    
    IEnumerator FetchUserCo()
    {
        //TESTING ONLY
        // UserName = "Fake User";
        // SchoolName = "Fake School";
        // GetGameList_Co(GameManager.instance);
        // GetUserCoins_Co(GameManager.instance);
        // GetGame1Dashboard_Co(GameManager.instance);
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
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
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
            GetUserCoins_Co(GameManager.instance);
            GetGame1Dashboard_Co(GameManager.instance);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame1DashboardCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame1Dashboard+"?idGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
        
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            // ShowAPIError(webRequest.error);
            Coins = 0;
            RewardCoins = 0;
            MenuUI.instance?.SetCoins();
            MenuUI.instance?.DashboardPanel.SetData();
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                // ShowAPIError("No Data Sent in API "+GetGame1Dashboard);
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log(json);
            JObject result = new JObject();
            result = JObject.Parse(json);
            Coins = (int)result["goldCoins"];
            RewardCoins = (int)result["rewardCoins"];
            MenuUI.instance?.SetCoins();
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetUserGame1LevelStatusCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetUserGame1LevelStatus+"?idGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
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
                // ShowAPIError("No Data Sent in API "+GetUserGame1LevelStatus);
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

    IEnumerator GetAllDescriptionsCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetLevelDataEndpoint+"?idGame="+idGame+"&OrgID="+OrgID, "GET");
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
                ShowAPIError("No Data Sent in API "+GetLevelDataEndpoint);
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);

            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            for (int i = 0; i < Levels.Count; i++)
            {
                temp = (JObject)result[i];
                Levels[i].idLevel = (int)temp["idLevel"];
                Levels[i].challengeName = (string)temp["challengeName"];
                Levels[i].challengeIntroMessage = (string)temp["challengeIntroMessage"];
                Levels[i].bottomCompleteMessage = (string)temp["bottomCompleteMessage"];
                Levels[i].bottomFailMessage = (string)temp["bottomFailMessage"];
                Levels[i].againPlayCoins = (int)temp["againPlayCoins"];
                Levels[i].attemptsAllowed = (int)temp["attemptsAllowed"];
                Levels[i].attemptTimer = (int)temp["attemptTimer"];
                Levels[i].idOrganization = (int)temp["idOrganization"];
            }
            GetUserGame1LevelStatus_Co(GameManager.instance);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetAllAttemptsCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetAllAttemptsEndpoint+"?idGame="+idGame+"&OrgID="+OrgID, "GET");
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
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
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
                _data.game5Behaviors = (string)temp["game5Behaviors"];
                _data.behaviorScore = (float)temp["behaviorScore"];
                _data.timeInSecond = (int)temp["timeInSecond"];
                _data.challengeCompletedTime1 = (int)temp["challengeCompletedTime1"];
                _data.rewardCoinsTime1 = (int)temp["rewardCoinsTime1"];
                _data.challengeCompletedTime2 = (int)temp["challengeCompletedTime2"];
                _data.rewardCoinsTime2 = (int)temp["rewardCoinsTime2"];
                _data.failAttemptMessage = (string)temp["failAttemptMessage"];
                AttemptData.Add(_data);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetUserCoinsCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetUserCoinsEndpoint+"?idGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            // Debug.Log("get coins api..."+webRequest.error);
            // ShowAPIError(webRequest.error);
            TotalCoins = 0;
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            if (json.Equals("No Data Available"))
            {
                TotalCoins = 0;
            }
            else
            {
                bool parse = int.TryParse(json, out int coins);
                TotalCoins = parse? coins : 0;
                MenuUI.instance?.SetCoins();
            }
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

    IEnumerator GetSubliminalMeasurement2Co(int _level)
    {
        // _level should be 2 or 4
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetSubliminal_Measurement2Endpoint+"?idGame="+idGame+"&OrgID="+OrgID+"&IdLevel="+_level, "GET");
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
            SubliminalMeasurement2Data = new List<SubliminalMeasurement2Data>();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                SubliminalMeasurement2Data _data = new SubliminalMeasurement2Data();
                _data.idSubliminalMeasurement2 = (int)temp["idSubliminalMeasurement2"];
                _data.idGame = (int)temp["idGame"];
                _data.idLevel = (int)temp["idLevel"];
                _data.idBehavior = (int)temp["idBehavior"];
                _data.subliminalMeasurement2Name = (string)temp["subliminalMeasurement2Name"];
                _data.behaviorScore = (int)temp["behaviorScore"];
                _data.idOrganization = (int)temp["idOrganization"];
                _data.gameFeedbackMsg = (string)temp["gameFeedbackMsg"];
                SubliminalMeasurement2Data.Add(_data);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostUserAttemptCo()
    {
        //TODO: Parameterize
        UserData req = new UserData();
        LevelData Current = CurrentLevel;
        req.IdUser = UID;
        req.IdGame = idGame;
        req.IdLevel = Current.idLevel;
        req.IdSubliminalMeasurement1 = Current.idSubliminalMeasurement1;
        req.IdSubliminalMeasurement2 = null;
        req.IdBehavior = Current.idBehavior;
        req.BehaviorScore = Current.behaviorScore;
        req.GoldCoins = Current.coins;
        req.RewardCoins = Current.rewardCoins;
        req.TimetakenToComplete = Current.timeTaken;
        req.IsCompleted = (int)Mathf.Clamp01(Current.isComplete);
        req.UpdatedDateTime = System.DateTime.Now;
        req.IdOrganization = Current.idOrganization;
        req.AttemptNo = Current.attemptNumber;
        req.NoOfMoves = Current.noOfMoves;
        req.CorrectMoves = Current.correctMoves;
        Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostUserAttempt, "POST");
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
            // Debug.Log("PostUserAttemptCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostUserFeedbackCo(string _feedbackResult)
    {
        //TODO: Parameterize
        Debug.Log(Domain+PostUserAttempt);
        UserData req = new UserData();
        LevelData Current = CurrentLevel;
        req.IdUser = UID;
        req.IdGame = idGame;
        req.IdLevel = Current.idLevel;
        req.IdSubliminalMeasurement1 = 0;
        req.IdSubliminalMeasurement2 = int.Parse(_feedbackResult);
        req.IdBehavior = 0;
        req.BehaviorScore = 0;
        req.GoldCoins = 0;
        req.RewardCoins = 0;
        req.TimetakenToComplete = 0;
        req.IsCompleted = 0;
        req.UpdatedDateTime = System.DateTime.Now;
        req.IdOrganization = Current.idOrganization;
        req.AttemptNo = 0;
        req.NoOfMoves = 0;
        req.CorrectMoves = 0;
        Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostUserAttempt, "POST");
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
            // Debug.Log("PostUserAttemptCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostUserPurchaseCo()
    {
        //TODO: Parameterize
        UserPurchaseLog req = new UserPurchaseLog();
        LevelData Current = CurrentLevel;
        req.IdUser = UID;
        req.IdGame = idGame;
        req.IdLevel = Current.idLevel;
        req.Coins = Current.againPlayCoins;
        req.AttemptNo = Current.attemptNumber;
        req.UpdatedDateTime = System.DateTime.Now;
        req.IdOrganization = Current.idOrganization;
        Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostUserPurchase, "POST");
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
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("PostUserPurchaseCo = "+json);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostGame1CorrectTilesLogCo()
    {
        //TODO: Parameterize
        Debug.Log(Domain+Game1CorrectTilesLog);
        Debug.Log(JsonConvert.SerializeObject(TilesList));
        // yield break;
        

        UnityWebRequest webRequest = new UnityWebRequest( Domain+Game1CorrectTilesLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(TilesList));
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
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("PostUserPurchaseCo = "+json);
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
}