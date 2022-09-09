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
    public Difficulty SelectedDifficulty;
    public List<LevelData> Levels;
    public Dictionary<int, int> Attempts;
    public Dictionary<int, bool> LevelComplete;

    public bool RedMedallion, BlueMedallion, GreenMedallion, YellowMedallion;
    public int Coins;
    public string AuthToken = "";
    public float Timer { get { return GetTimer(); } }
    public int MaxAttempts { get { return GetMaxAttempts(); } }
    public LevelData CurrentLevel { get { return GetCurrentLevel(); } }
    public string UserName { get; set; }
    public string SchoolName { get; set; }
    public int MedallionCount { get { return CountMedallions(); } }

    string Domain = "https://censiobeta.in/CensioApi/api/";
    string CensioDomain = "https://app-api.censioanalytics.com/";
    const string FetchUserEndpoint = "api/v1/users/fetch_user";
    const string GetLevelDataEndpoint = "GetGameChallengeByGameId";
    const string GetAllAttemptsEndpoint = "GetGameAttemptData";
    const string GetUserCoinsEndpoint = "GetTotalGame5Coins";
    const string GetSubliminalMeasurement1Endpoint = "GetSubliminal_Measurement1";
    const string GetGame5AttempNumberEndpoint = "GetGame5AttempNumber";
    const string PostUserAttempt = "Game5UserLog";
    const string PostUserPurchase = "GameUserAttemptPurchaseLog";
    const string Game5ClickLog = "Game5ClickLog";
    const string GetGameList = "GetGameList";
    const string GamePlayLog = "GamePlayLog";

    string gameName = "THE GREAT ESCAPE";
    const int idGame = 5, OrgID = 1;

    public int UID { get; set; }
    public int loadingCounter;
    List<AttemptData> AttemptData;
    List<SubliminalMeasurement1Data> SubliminalMeasurement1Data;
    public List<string> ClickLog { get; set; }
    public PreviousLevelData PreviousLevelData { get; set; }

    void OnEnable() 
    {
        instance = this;
    }

    public void Init()
    {
        RedMedallion = false;
        GreenMedallion = false;
        BlueMedallion = false;
        YellowMedallion = false;
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

        ClickLog = new List<string>();

        if (Attempts == null)
        {
            Attempts = new Dictionary<int, int>();
            Attempts.Add(0, 0);
            Attempts.Add(1, 0);
            Attempts.Add(2, 0);
            Attempts.Add(3, 0);
        }

        if (LevelComplete == null)
        {
            LevelComplete = new Dictionary<int, bool>();
            LevelComplete.Add(0, false);
            LevelComplete.Add(1, false);
            LevelComplete.Add(2, false);
            LevelComplete.Add(3, false);
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
            if (Levels[i].difficulty == SelectedDifficulty)
            {
                return Levels[i].attemptsAllowed;
            }
        }

        return 0;
    }

    int CountMedallions()
    {
        int count = 0;
        count += RedMedallion?1:0;
        count += GreenMedallion?1:0;
        count += BlueMedallion?1:0;
        count += YellowMedallion?1:0;
        return count;
    }

    float GetTimer()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].difficulty == SelectedDifficulty)
            {
                return Levels[i].attemptTimer;
            }
        }

        return 120f;
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
        Debug.Log(_msg);
        MenuUI.instance?.ShowError(_msg);
    }

    LevelData GetCurrentLevel()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].difficulty == SelectedDifficulty)
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

    public LevelData GetCurrentLevelFor(Difficulty _difficulty)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].difficulty == _difficulty)
            {
                return Levels[i];
            }
        }
        return null;
    }

    public void GetGameList_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameListCo());
    }

    public void GetAllDescriptions_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetAllDescriptionsCo());
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

    public void GetGame5API_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame5APICo());
    }

    public void GetSubliminalMeasurement1_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetSubliminalMeasurement1Co());
    }

    public void PostUserAttempt_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostUserAttemptCo());
    }

    public void PostGame5ClickLog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostGame5ClickLogCo());
    }

    public void PostUserPurchase_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostUserPurchaseCo());
    }

    public void GamePlayLog_Co(MonoBehaviour caller, string _name)
    {
        caller.StartCoroutine(GamePlayLogCo(_name));
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                GamePlayLog_Co(GameManager.instance, gameName);
                UpdateLoadingCounter();
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetLevelDataEndpoint);
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
                Levels[i].againPlayCoins = (int)temp["againPlayCoins"];
                Levels[i].attemptsAllowed = (int)temp["attemptsAllowed"];
                Levels[i].attemptTimer = (int)temp["attemptTimer"];
                Levels[i].idOrganization = (int)temp["idOrganization"];
            }
            UpdateLoadingCounter();
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
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
                AttemptData _data = new AttemptData();
                _data.idAttempt = (int)temp["idAttempt"];
                _data.idLevel = (int)temp["idLevel"];
                _data.attemptNo = (int)temp["attemptNo"];
                _data.idGame = (int)temp["idGame"];
                _data.goldCoins = (int)temp["goldCoins"];
                // _data.idBehavior = (int)temp["idBehavior"];
                _data.game5Behaviors = (string)temp["game5Behaviors"];
                _data.behaviorScore = (float)temp["behaviorScore"];
                _data.timeInSecond = (int)temp["timeInSecond"];
                AttemptData.Add(_data);
            }
            UpdateLoadingCounter();
            yield break;
        } 
    }

    IEnumerator FetchUserCo()
    {
        //TESTING ONLY
        // UserName = "Fake User";
        // SchoolName = "Fake School";
        // GetUserCoins_Co(GameManager.instance);
        // GetGameList_Co(GameManager.instance);
        // GetGame5API_Co(GameManager.instance);
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
            // Debug.Log(webRequest.error);
            ShowAPIError(webRequest.error);
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
                yield break;
            }
            // Debug.Log(json);

            JObject result = new JObject();
            result = JObject.Parse(json);
            UID = (int)result.SelectToken("data").SelectToken("user").SelectToken("id");
            UserName = (string)result.SelectToken("data").SelectToken("user").SelectToken("username");
            SchoolName = (string)result.SelectToken("data").SelectToken("user").SelectToken("school_name");
            GetUserCoins_Co(GameManager.instance);
            GetGameList_Co(GameManager.instance);
            GetGame5API_Co(GameManager.instance);
            UpdateLoadingCounter();
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
            Coins = 0;
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetAllAttemptsEndpoint);
                yield break;
            }
            // Debug.Log(json);
            if (json.Equals("No Data Available"))
            {
                Coins = 0;
            }
            else
            {
                bool parse = int.TryParse(json, out int coins);
                Coins = parse? coins : 0;
                MenuUI.instance?.SetCoins();
            }

            yield break;
        } 
    }

    IEnumerator GetGame5APICo()
    {
        // Debug.Log(Domain+GetGame5AttempNumberEndpoint+"?idGame="+idGame+"&UID="+UID);
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame5AttempNumberEndpoint+"?idGame="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            // ShowAPIError(webRequest.error);
            // Debug.Log("error, stop sending text instead of json.. "+webRequest.error);
            PreviousLevelData = null;
            UpdateLoadingCounter();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetGame5AttempNumberEndpoint);
                yield break;
            }

            PreviousLevelData = new PreviousLevelData();
            JObject result = new JObject();
            result = JObject.Parse(json);
            PreviousLevelData.idLevel = (int)result["idLevel"];
            PreviousLevelData.idBehavior = (int)result["idBehavior"];
            PreviousLevelData.behaviorScore = (int)result["behaviorScore"];
            PreviousLevelData.attemptNo = (int)result["attemptNo"];
            PreviousLevelData.isCompleted = (int)result["isCompleted"];
            PreviousLevelData.medallionsCollected = (int)result["medallionsCollected"];
            switch (PreviousLevelData.medallionsCollected)
            {
                case 1:
                Data.instance.BlueMedallion = true;
                break;

                case 2:
                Data.instance.BlueMedallion = true;
                Data.instance.YellowMedallion = true;
                break;

                case 3:
                Data.instance.BlueMedallion = true;
                Data.instance.YellowMedallion = true;
                Data.instance.GreenMedallion = true;
                break;

                default:
                break;
            }
            UpdateLoadingCounter();
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("No Data Sent in API "+GetSubliminalMeasurement1Endpoint);
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
        req.TimetakenToComplete = Current.timeTaken;
        req.MedallionsCollected = CountMedallions();
        req.IsCompleted = Current.isComplete;
        req.UpdatedDateTime = System.DateTime.Now;
        req.IdOrganization = Current.idOrganization;
        req.AttemptNo = Current.attemptNumber;
        req.ObstacleName = Current.playerDied?Current.ObstacleName:"";
        req.NoOfClicks = Current.NoOfClicks;
        req.NoValidclicks = Current.NoValidclicks;
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("PostUserAttemptCo = "+json);
            yield break;
        } 
    }
    
    IEnumerator PostGame5ClickLogCo()
    {
        //TODO: Parameterize
        List<Game5ClickLog> reqArr = new List<Game5ClickLog>();
        LevelData Current = CurrentLevel;
        for (int i = 0; i < ClickLog.Count; i++)
        {
            Game5ClickLog req = new Game5ClickLog();
            req.GameName = gameName;
            req.IdUser = UID;
            req.IdLevel = Current.idLevel;
            req.AttemptNo = Current.attemptNumber;
            req.InteractableName = ClickLog[i];
            req.SequenceNo = i;
            reqArr.Add(req);
        }
        Debug.Log(JsonConvert.SerializeObject(reqArr));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+Game5ClickLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(reqArr));
        webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log(webRequest.error);
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("PostUserPurchaseCo = "+json);
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
        req.AttemptNo = Attempts[(int)Data.instance.SelectedDifficulty]+1;
        req.UpdatedDateTime = System.DateTime.Now;
        req.IdOrganization = Current.idOrganization;
        // Debug.Log(JsonConvert.SerializeObject(req));
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
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                yield break;
            }
            // Debug.Log(json);
            // Debug.Log("PostUserPurchaseCo = "+json);
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

            yield break;
        } 
    }
}