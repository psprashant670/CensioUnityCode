using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu]
public class Data : ScriptableObject
{
    public static Data instance;

    public bool IsCustomBuild = false, isStagingDomain = false;
    public List<FestivalData> Festivals;

    [Multiline(10)]
    public string CurrentBoard;
    public List<BoardData> Boards;
    public int Score;
    public int primaryScore = 1000, secondaryScore = 250;
    public int UID { get; set; }
    public int loadingCounter;
    public string AuthToken = "";
    public string UserName { get; set; }
    public string SchoolName { get; set; }

    //  string Domain = "https://censiobeta.in/CensioApi/api/";
    string Domain = "https://censiobeta.in/CensioGame3Api/api/";
    string CensioDomain = "https://app-api.censioanalytics.com/";
    const string FetchUserEndpoint = "api/v1/users/fetch_user";
    const string GetGameList = "GetGameList";
    const string GetGame3Instructions = "GetGame3Instructions";
    const string GetGame3Reset = "GetGame3Reset";
    const string GetGame3Festival = "GetGame3Festival";
    const string GetGame3Level = "GetGame3Level";
    const string GetGame3Question = "GetGame3Question";
    const string GetGame3Response = "GetGame3Response";
    const string GetGame3UserStatus = "GetGame3UserStatus";
    const string PostGame3Log = "Game3Log";
    const string PostGamePlayLog = "GamePlayLog";
    const string PostGame3RawMaterialLog = "Game3RawMaterialLog";
    const string PostGame3ResponseLog = "Game3ResponseLog";

    const int idGame = 3, OrgID = 1;
    
    public string gameName = "TheRestauranteur";
    public string Question { get; set; }
    public List<QuestionResponses> Responses { get; set; }
    public List<string> Instructions { get; set; }
    public GetGame3UserStatus SaveData { get; set; }
    public List<MaterialLog> MaterialLogs { get; set; }
    public TileType LastFestivalCompleted;
    [HideInInspector]
    public FestivalData WheatFest, GrapeFest, ChocolateFest, MilkFest;

    void OnEnable() 
    {
        instance = this;
    }

    public void Init()
    {
        Score = 0;
        UID = 0;

        if (isStagingDomain)
        {
            Domain = "https://censiostaging.in/CensioGame3Api/api/";
            CensioDomain = "https://app-api.staging.censioanalytics.com/";
        }
        else
        {
            Domain = "https://censiobeta.in/CensioGame3Api/api/";
            CensioDomain = "https://app-api.censioanalytics.com/";
        }

        LastFestivalCompleted = TileType.Empty;
        SaveData = null;
        MaterialLogs = new List<MaterialLog>();
        foreach (FestivalData _fest in Festivals)
        {
            _fest.Wheat = 0;
            _fest.Grape = 0;
            _fest.Chocolate = 0;
            _fest.Milk = 0;
            _fest.FestivalProgress = TileType.Empty;
            _fest.WheatPresented = 0;
            _fest.GrapePresented = 0;
            _fest.ChocolatePresented = 0;
            _fest.MilkPresented = 0;
            _fest.WheatSelected = 0;
            _fest.GrapeSelected = 0;
            _fest.ChocolateSelected = 0;
            _fest.MilkSelected = 0;
            _fest.TimeTaken = 0;
            _fest.WheatSecAquired = false;
            _fest.GrapeSecAquired = false;
            _fest.ChocolateSecAquired = false;
            _fest.MilkSecAquired = false;
        }

        WheatFest = Festivals.Find((fest)=> fest.FestivalType==TileType.Wheat);
        GrapeFest = Festivals.Find((fest)=> fest.FestivalType==TileType.Grape);
        ChocolateFest = Festivals.Find((fest)=> fest.FestivalType==TileType.Chocolate);
        MilkFest = Festivals.Find((fest)=> fest.FestivalType==TileType.Milk);
    }

    public BoardData GetBoardData(TileType _type)
    {
        // return Boards.Find((board) => board.Festival.Equals("milk"));
        BoardData Board = Boards.Find((board) => board.Festival == _type);
        return Board;
    }

    void UpdateLoadingCounter()
    {
        if (GameManager.isLoaded)
        {
            return;
        }
        
        loadingCounter--;
        // float _per = Mathf.Abs(((float)loadingCounter - (float)GameManager.instance.LoadingCount)/(float)GameManager.instance.LoadingCount);
        // MenuUI.instance?.SetLoading(Mathf.Clamp01(_per));
        MenuUI.instance?.SetLoading();
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

    public void FetchUser_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(FetchUserCo());
    }

    public void GetGameList_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGameListCo());
    }

    public void GetGame3Instructions_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3InstructionsCo());
    }

    public void GetGame3Festival_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3FestivalCo());
    }

    public void GetGame3Level_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3LevelCo());
    }

    public void GetGame3Question_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3QuestionCo());
    }

    public void GetGame3Response_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3ResponseCo());
    }

    public void GetGame3UserStatus_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(GetGame3UserStatusCo());
    }

    public void PostGamePlayLog_Co(MonoBehaviour caller, string _name)
    {
        caller.StartCoroutine(PostGamePlayLogCo(_name));
    }

    public void PostGame3Log_Co(MonoBehaviour caller, FestivalData _festivalData)
    {
        caller.StartCoroutine(PostGame3LogCo(_festivalData));
    }

    public void PostGame3RawMaterialLog_Co(MonoBehaviour caller)
    {
        caller.StartCoroutine(PostGame3RawMaterialLogCo());
    }

    public void PostGame3ResponseLog_Co(MonoBehaviour caller, List<Responses> _responses)
    {
        caller.StartCoroutine(PostGame3ResponseLogCo(_responses));
    }

    IEnumerator FetchUserCo()
    {
        //TESTING ONLY
        // UserName = "Fake User";
        // SchoolName = "Fake School";
        // GetGameList_Co(GameManager.instance);
        // GetGame3UserStatus_Co(GameManager.instance);
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
            Debug.Log("UID = "+UID);
            GetGameList_Co(GameManager.instance);
            GetGame3UserStatus_Co(GameManager.instance);
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
        // Debug.Log(Domain+GetGameList+"?OrgID="+OrgID);

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
                }
            }
            PostGamePlayLog_Co(GameManager.instance, gameName);
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3InstructionsCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3Instructions+"?OrgID="+OrgID+"&GameId="+idGame, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            ShowAPIError("GetGame3Instructions api error");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("GetGame3Instructions api error");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            Instructions = new List<string>();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                string instructionText = (string)temp["instructionText"];
                Instructions.Add(instructionText);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3FestivalCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3Festival+"?OrgID="+OrgID+"&GameId="+idGame, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            ShowAPIError("API Error GetGame3Festival");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("API Error GetGame3Festival");
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
                FestivalData festivalData = Festivals.Find((fest)=> fest.FestivalType == (TileType)((int)temp["idFestival"]));
                festivalData.rawMaterialExactqty = (int)temp["rawMaterialExactqty"];
                festivalData.rawMaterialMinQtyPast = (int)temp["rawMaterialMinQtyPast"];
                festivalData.rawMaterialMinQtyFuture = (int)temp["rawMaterialMinQtyFuture"];
                festivalData.primaryMaterialScore = (int)temp["primaryMaterialScore"];
                festivalData.secondaryMaterialScorePositive = (int)temp["secondaryMaterialScorePositive"];
                festivalData.secondaryMaterialScoreNegative = (int)temp["secondaryMaterialScoreNegative"];
                festivalData.timeRequiredSeconds = (int)temp["timeRequiredSeconds"];
                festivalData.idFestival = (int)temp["idFestival"];
                festivalData.festivalName = (string)temp["festivalName"];
                festivalData.rawMaterial = (string)temp["rawMaterial"];
                primaryScore = festivalData.primaryMaterialScore;
                secondaryScore = festivalData.secondaryMaterialScorePositive;
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3LevelCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3Level+"?OrgID="+OrgID+"&GameId="+idGame, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            ShowAPIError("API Error GetGame3Level");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("API Error GetGame3Level");
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
                FestivalData festivalData = Festivals.Find((fest)=> fest.FestivalType == (TileType)((int)temp["idLevel"]));
                festivalData.idLevel = (int)temp["idLevel"];
                festivalData.levelClearenceAmount = (int)temp["levelClearenceAmount"];
                festivalData.levelClearenceCurrency = (string)temp["levelClearenceCurrency"];
                festivalData.levelName = (string)temp["levelName"];
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3QuestionCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3Question+"?OrgID="+OrgID+"&GameId="+idGame, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            ShowAPIError("API Error GetGame3Question");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("API Error GetGame3Question");
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
                if ((int)temp["idQuestion"]==1)
                {
                    Question = (string)temp["idQuestionText"];
                }
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3ResponseCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3Response+"?Ques=1", "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGameList");
            ShowAPIError("API Error GetGame3Response");
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                ShowAPIError("API Error GetGame3Response");
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            Responses = new List<QuestionResponses>();
            for (int i = 0; i < result.Count; i++)
            {
                temp = (JObject)result[i];
                QuestionResponses resp = new QuestionResponses();
                resp.idResponse = (int)temp["idResponse"];
                resp.idResponseGroup = (int)temp["idResponseGroup"];
                resp.reponseOptionNo = (int)temp["reponseOptionNo"];
                resp.responseOptionName = (string)temp["responseOptionName"];
                resp.responseOptionDescription = (string)temp["responseOptionDescription"];
                resp.additionInformation = (string)temp["additionInformation"];
                Responses.Add(resp);
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator GetGame3UserStatusCo()
    {
        UnityWebRequest webRequest = new UnityWebRequest( Domain+GetGame3UserStatus+"?OrgID="+OrgID+"&GameId="+idGame+"&UID="+UID, "GET");
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Access-Control-Allow-Origin", "*");
        // Debug.Log(Domain+GetGame3UserStatus+"?OrgID="+OrgID+"&GameId="+idGame+"&UID="+UID);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError) 
        {
            Debug.Log("error GetGame3UserStatusCo");
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        }
        else 
        {
            string json = webRequest.downloadHandler.text;
            if (string.IsNullOrEmpty(json))
            {
                UpdateLoadingCounter();
                webRequest.Dispose();
                yield break;
            }
            // Debug.Log(json);
            JArray result = new JArray();
            result = JArray.Parse(json);
            JObject temp = new JObject();
            JArray UserLogs = new JArray();
            JArray LevelLogs = new JArray();
            for (int i = 0; i < result.Count; i++)
            {
                if (((JObject)result[i]).ContainsKey("idGame3UserLog"))
                {
                    UserLogs.Add(result[i]);
                }
                else
                {
                    LevelLogs.Add(result[i]);
                }
            }
            // level data
            if (LevelLogs.Count>0)
            {
                SaveData = new GetGame3UserStatus();
                temp = (JObject)LevelLogs[LevelLogs.Count-1];
                SaveData.endIdLevel = (int)temp["endIdLevel"];
                SaveData.endLevelName = (string)temp["endLevelName"];
                SaveData.festivalName = (string)temp["festivalName"];
                SaveData.levelCumulativeScore = (int)temp["levelCumulativeScore"];
                Score = SaveData.levelCumulativeScore;
                Festivals.Find((fest)=> fest.festivalName.Equals(SaveData.festivalName)).FestivalProgress = (TileType)SaveData.endIdLevel;
                LastFestivalCompleted = Festivals.Find((fest)=> fest.festivalName.Equals(SaveData.festivalName)).FestivalType;
                if (SaveData.festivalName.Equals(MilkFest.festivalName))
                {
                    MenuUI.instance?.ShowGameOver();
                }

                for (int i = 0; i < LevelLogs.Count; i++)
                {
                    temp = (JObject)LevelLogs[i];
                    Festivals.Find((fest)=> fest.festivalName.Equals((string)temp["festivalName"])).FestivalProgress = (TileType)(int)temp["endIdLevel"];
                }
            }
            // tile data
            for (int i = 0; i < UserLogs.Count; i++)
            {
                // Festivals.Find((fest)=> fest.rawMaterial.Equals((string)UserLogs[i]["rawMaterial"]));
                switch ((int)UserLogs[i]["idFestival"])
                {
                    case 1:
                    WheatFest.Wheat = (int)UserLogs[i]["levelFestivalAmount"];
                    WheatFest.Grape = (int)UserLogs[i]["secondaryRawMatSelected1"];
                    WheatFest.Chocolate = (int)UserLogs[i]["secondaryRawMatSelected2"];
                    WheatFest.Milk = (int)UserLogs[i]["secondaryRawMatSelected3"];
                    break;

                    case 2:
                    GrapeFest.Wheat = (int)UserLogs[i]["secondaryRawMatSelected1"];
                    GrapeFest.Grape = (int)UserLogs[i]["levelFestivalAmount"];
                    GrapeFest.Chocolate = (int)UserLogs[i]["secondaryRawMatSelected2"];
                    GrapeFest.Milk = (int)UserLogs[i]["secondaryRawMatSelected3"];
                    break;

                    case 3:
                    ChocolateFest.Wheat = (int)UserLogs[i]["secondaryRawMatSelected1"];
                    ChocolateFest.Grape = (int)UserLogs[i]["secondaryRawMatSelected2"];
                    ChocolateFest.Chocolate = (int)UserLogs[i]["levelFestivalAmount"];
                    ChocolateFest.Milk = (int)UserLogs[i]["secondaryRawMatSelected3"];
                    break;

                    case 4:
                    MilkFest.Wheat = (int)UserLogs[i]["secondaryRawMatSelected1"];
                    MilkFest.Grape = (int)UserLogs[i]["secondaryRawMatSelected2"];
                    MilkFest.Chocolate = (int)UserLogs[i]["secondaryRawMatSelected3"];
                    MilkFest.Milk = (int)UserLogs[i]["levelFestivalAmount"];
                    break;

                    default:
                    break;
                }
            }
            UpdateLoadingCounter();
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostGame3LogCo(FestivalData festivalData)
    {
        UserData req = new UserData();
        req.IdUser = UID;
        req.IdGame = idGame;
        req.IdOrganization = OrgID;
        req.UpdatedDateTime = System.DateTime.Now;

        req.IdFestival = festivalData.idFestival;
        req.LifeNo = festivalData.idFestival;
        req.FestivalName = festivalData.festivalName;
        req.RawMaterial = festivalData.rawMaterial;
        GetMaterialFor(festivalData, out req.FestivalRawMatPresented, out req.FestivalRawMatSelected, out req.SecondaryRawMatPresented1, out req.SecondaryRawMatPresented2, out req.SecondaryRawMatPresented3, out req.SecondaryRawMatSelected1, out req.SecondaryRawMatSelected2, out req.SecondaryRawMatSelected3);
        req.FestivalScore = festivalData.FestivalScore;
        req.FestivalSuccess = festivalData.FestivalSuccess;
        GetSecondaryScoresNSuccessFor(festivalData, out req.SecondaryScore1, out req.SecondaryScore2, out req.SecondaryScore3, out req.SecondarySuccess1, out req.SecondarySuccess2, out req.SecondarySuccess3);
        GetSecondaryNameNTypeFor(festivalData, out req.SecondaryFestivalName1, out req.SecondaryFestivalName2, out req.SecondaryFestivalName3, out req.SecondaryFestivalType1, out req.SecondaryFestivalType2, out req.SecondaryFestivalType3);
        req.TimeTaken = festivalData.TimeTaken;
        GetExcessPrimaryFor(festivalData, out req.FestivalExcessAmountCollected, out req.LevelFestivalAmount);
        GetStartNEndLevelFor(festivalData, out req.StartIdLevel, out req.EndIdLevel, out req.StartLevelName, out req.EndLevelName);
        req.LevelName = req.EndLevelName;
        req.LevelStartScore = festivalData.LevelStartScore;
        req.LevelEndScore = festivalData.LevelEndScore;
        req.FestivalLevelScore = festivalData.LevelEndScore - festivalData.LevelStartScore;
        req.LevelCumulativeScore = festivalData.LevelCumulativeScore;

        // Debug.Log(JsonConvert.SerializeObject(req));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostGame3Log, "POST");
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
            // Debug.Log(webRequest.responseCode);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostGame3RawMaterialLogCo()
    {
        // Debug.Log(JsonConvert.SerializeObject(MaterialLogs));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostGame3RawMaterialLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(MaterialLogs));
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
            // Debug.Log(webRequest.responseCode);
            webRequest.Dispose();
            yield break;
        } 
    }

    IEnumerator PostGame3ResponseLogCo(List<Responses> _responses)
    {
        // Debug.Log(JsonConvert.SerializeObject(_responses));
        // yield break;

        UnityWebRequest webRequest = new UnityWebRequest( Domain+PostGame3ResponseLog, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(JsonConvert.SerializeObject(_responses));
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
            // Debug.Log(webRequest.result);
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


    void GetMaterialFor(FestivalData _festivalData, out int pP, out int pS, out int sP1, out int sP2, out int sP3, out int sS1, out int sS2, out int sS3)
    {
        pP = 0;
        pS = 0;
        sP1 = 0;
        sP2 = 0;
        sP3 = 0;
        sS1 = 0;
        sS2 = 0;
        sS3 = 0;

        if (_festivalData.FestivalType == TileType.Wheat)
        {
            pP = _festivalData.WheatPresented;
            pS = _festivalData.WheatSelected;
            sP1 = _festivalData.GrapePresented;
            sP2 = _festivalData.ChocolatePresented;
            sP3 = _festivalData.MilkPresented;
            sS1 = _festivalData.GrapeSelected;
            sS2 = _festivalData.ChocolateSelected;
            sS3 = _festivalData.MilkSelected;
        }
        else if (_festivalData.FestivalType == TileType.Grape)
        {
            pP = _festivalData.GrapePresented;
            pS = _festivalData.GrapeSelected;
            sP1 = _festivalData.WheatPresented;
            sP2 = _festivalData.ChocolatePresented;
            sP3 = _festivalData.MilkPresented;
            sS1 = _festivalData.WheatSelected;
            sS2 = _festivalData.ChocolateSelected;
            sS3 = _festivalData.MilkSelected;
        }
        else if (_festivalData.FestivalType == TileType.Chocolate)
        {
            pP = _festivalData.ChocolatePresented;
            pS = _festivalData.ChocolateSelected;
            sP1 = _festivalData.WheatPresented;
            sP2 = _festivalData.GrapePresented;
            sP3 = _festivalData.MilkPresented;
            sS1 = _festivalData.WheatSelected;
            sS2 = _festivalData.GrapeSelected;
            sS3 = _festivalData.MilkSelected;
        }
        else if (_festivalData.FestivalType == TileType.Milk)
        {
            pP = _festivalData.MilkPresented;
            pS = _festivalData.MilkSelected;
            sP1 = _festivalData.WheatPresented;
            sP2 = _festivalData.GrapePresented;
            sP3 = _festivalData.ChocolatePresented;
            sS1 = _festivalData.WheatSelected;
            sS2 = _festivalData.GrapeSelected;
            sS3 = _festivalData.ChocolateSelected;
        }
    }

    void GetSecondaryScoresNSuccessFor(FestivalData _festivalData, out int s1, out int s2, out int s3, out int ss1, out int ss2, out int ss3)
    {
        s1 = 0;
        s2 = 0;
        s3 = 0;
        ss1 = 0;
        ss2 = 0;
        ss3 = 0;

        if (_festivalData.FestivalType == TileType.Wheat)
        {
            s1 = _festivalData.GrapeSecAquired? 250 : 0;
            s2 = _festivalData.ChocolateSecAquired? 250 : 0;
            s3 = _festivalData.MilkSecAquired? 250 : 0;
            ss1 = _festivalData.GrapeSecAquired? 1 : 0;
            ss2 = _festivalData.ChocolateSecAquired? 1 : 0;
            ss3 = _festivalData.MilkSecAquired? 1 : 0;
        }
        else if (_festivalData.FestivalType == TileType.Grape)
        {
            s1 = _festivalData.WheatSecAquired? 250 : 0;
            s2 = _festivalData.ChocolateSecAquired? 250 : 0;
            s3 = _festivalData.MilkSecAquired? 250 : 0;
            ss1 = _festivalData.WheatSecAquired? 1 : 0;
            ss2 = _festivalData.ChocolateSecAquired? 1 : 0;
            ss3 = _festivalData.MilkSecAquired? 1 : 0;
        }
        else if (_festivalData.FestivalType == TileType.Chocolate)
        {
            s1 = _festivalData.WheatSecAquired? 250 : 0;
            s2 = _festivalData.GrapeSecAquired? 250 : 0;
            s3 = _festivalData.MilkSecAquired? 250 : 0;
            ss1 = _festivalData.WheatSecAquired? 1 : 0;
            ss2 = _festivalData.GrapeSecAquired? 1 : 0;
            ss3 = _festivalData.MilkSecAquired? 1 : 0;
        }
        else if (_festivalData.FestivalType == TileType.Milk)
        {
            s1 = _festivalData.WheatSecAquired? 250 : 0;
            s2 = _festivalData.GrapeSecAquired? 250 : 0;
            s3 = _festivalData.ChocolateSecAquired? 250 : 0;
            ss1 = _festivalData.WheatSecAquired? 1 : 0;
            ss2 = _festivalData.GrapeSecAquired? 1 : 0;
            ss3 = _festivalData.ChocolateSecAquired? 1 : 0;
        }
    }

    void GetSecondaryNameNTypeFor(FestivalData _festivalData, out string sN1, out string sN2, out string sN3, out string sT1, out string sT2, out string sT3)
    {
        sN1 = "";
        sN2 = "";
        sN3 = "";
        sT1 = "";
        sT2 = "";
        sT3 = "";

        if (_festivalData.FestivalType == TileType.Wheat)
        {
            sN1 = GrapeFest.festivalName;
            sN2 = ChocolateFest.festivalName;
            sN3 = MilkFest.festivalName;
            sT1 = "F";
            sT2 = "F";
            sT3 = "F";
        }
        else if (_festivalData.FestivalType == TileType.Grape)
        {
            sN1 = WheatFest.festivalName;
            sN2 = ChocolateFest.festivalName;
            sN3 = MilkFest.festivalName;
            sT1 = "P";
            sT2 = "F";
            sT3 = "F";
        }
        else if (_festivalData.FestivalType == TileType.Chocolate)
        {
            sN1 = WheatFest.festivalName;
            sN2 = GrapeFest.festivalName;
            sN3 = MilkFest.festivalName;
            sT1 = "P";
            sT2 = "P";
            sT3 = "F";
        }
        else if (_festivalData.FestivalType == TileType.Milk)
        {
            sN1 = WheatFest.festivalName;
            sN2 = GrapeFest.festivalName;
            sN3 = ChocolateFest.festivalName;
            sT1 = "P";
            sT2 = "P";
            sT3 = "P";
        }
    }

    void GetExcessPrimaryFor(FestivalData _festivalData, out int excessAmount, out int amount)
    {
        excessAmount = 0;
        amount = 0;

        if (_festivalData.FestivalType == TileType.Wheat)
        {
            excessAmount = _festivalData.ExcessWheat;
            amount = _festivalData.Wheat;
        }
        else if (_festivalData.FestivalType == TileType.Grape)
        {
            excessAmount = _festivalData.ExcessGrape;
            amount = _festivalData.Grape;
        }
        else if (_festivalData.FestivalType == TileType.Chocolate)
        {
            excessAmount = _festivalData.ExcessChocolate;
            amount = _festivalData.Chocolate;
        }
        else if (_festivalData.FestivalType == TileType.Milk)
        {
            excessAmount = _festivalData.ExcessMilk;
            amount = _festivalData.Milk;
        }
    }

    void GetStartNEndLevelFor(FestivalData _festivalData, out int StartIdLevel, out int EndIdLevel, out string StartLevelName, out string EndLevelName)
    {
        StartIdLevel = 0;
        EndIdLevel = 0;
        StartLevelName = "Base";
        EndLevelName = "Base";
        EndIdLevel = (int)_festivalData.FestivalProgress;

        if (_festivalData.FestivalType == TileType.Wheat)
        {
            StartIdLevel = (int)0;
        }
        else if (_festivalData.FestivalType == TileType.Grape)
        {
            StartIdLevel = (int)WheatFest.FestivalProgress;
        }
        else if (_festivalData.FestivalType == TileType.Chocolate)
        {
            StartIdLevel = (int)GrapeFest.FestivalProgress;
        }
        else if (_festivalData.FestivalType == TileType.Milk)
        {
            StartIdLevel = (int)ChocolateFest.FestivalProgress;
        }

        switch (StartIdLevel)
        {
            case 1:
            StartLevelName = WheatFest.levelName;
            break;
            case 2:
            StartLevelName = GrapeFest.levelName;
            break;
            case 3:
            StartLevelName = ChocolateFest.levelName;
            break;
            case 4:
            StartLevelName = MilkFest.levelName;
            break;
            default:
            StartLevelName = "Base";
            break;
        }

        switch (EndIdLevel)
        {
            case 1:
            EndLevelName = WheatFest.levelName;
            break;
            case 2:
            EndLevelName = GrapeFest.levelName;
            break;
            case 3:
            EndLevelName = ChocolateFest.levelName;
            break;
            case 4:
            EndLevelName = MilkFest.levelName;
            break;
            default:
            EndLevelName = "Base";
            break;
        }
    }
}
