using System;

public class UserData
{
    public int IdUser { get; set; }
    public int IdGame { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public int IdOrganization { get; set; }

    public int IdFestival { get; set; }
    public string FestivalName { get; set; }
    public int LifeNo { get; set; }

    public string RawMaterial;
    public int FestivalRawMatPresented;
    public int FestivalRawMatSelected;
    public int FestivalScore;
    public int FestivalSuccess;
    public string SecondaryFestivalName1;
    public string SecondaryFestivalType1;
    public int SecondaryRawMatPresented1;
    public int SecondaryRawMatSelected1;
    public int SecondaryScore1;
    public int SecondarySuccess1;
    public string SecondaryFestivalName2;
    public string SecondaryFestivalType2;
    public int SecondaryRawMatPresented2;
    public int SecondaryRawMatSelected2;
    public int SecondaryScore2;
    public int SecondarySuccess2;
    public string SecondaryFestivalName3;
    public string SecondaryFestivalType3;
    public int SecondaryRawMatPresented3;
    public int SecondaryRawMatSelected3;
    public int SecondaryScore3;
    public int SecondarySuccess3;
    public int TimeTaken;
    public int FestivalExcessAmountCollected; // excess primary tiles collected
    public int LevelFestivalAmount;
    public string LevelName; // same as EndLevelName
    public int StartIdLevel;
    public string StartLevelName;
    public int EndIdLevel;
    public string EndLevelName;
    public int FestivalLevelScore;
    public int LevelStartScore;
    public int LevelEndScore;
    public int LevelCumulativeScore;
}

[Serializable]
public class FestivalData
{
    public string Name;
    public TileType FestivalType, FestivalProgress, FestivalStartProgress;
    public int Wheat, Grape, Chocolate, Milk;
    public int MaxWheat, MaxGrape, MaxChocolate, MaxMilk;

    public int primaryMaterialScore, secondaryMaterialScorePositive, secondaryMaterialScoreNegative, timeRequiredSeconds, rawMaterialExactqty, rawMaterialMinQtyPast, rawMaterialMinQtyFuture, idFestival;
    public int idLevel, levelClearenceAmount;
    public string levelName, levelClearenceCurrency, festivalName, rawMaterial;

    public int TimeTaken, WheatPresented, WheatSelected, GrapePresented, GrapeSelected, ChocolatePresented, ChocolateSelected, MilkPresented, MilkSelected;
    public int FestivalScore, FestivalSuccess, StartIdLevel, EndIdLevel, LevelStartScore, LevelEndScore, LevelCumulativeScore;
    public string StartLevelName, EndLevelName;

    // helpers
    public bool WheatSecAquired, GrapeSecAquired, ChocolateSecAquired, MilkSecAquired;
    public int ExcessWheat, ExcessGrape, ExcessChocolate, ExcessMilk;
}

[Serializable]
public class GamePlayLog
{
    public int IdUser { get; set; }
    public string GameName { get; set; }
}

[Serializable]
public class QuestionResponses
{
    public int idResponse, idResponseGroup, reponseOptionNo;
    public string responseOptionName, responseOptionDescription, additionInformation;
}

[Serializable]
public class Responses
{
    public int IdUser, IdQuestion, IdResponse, IdResponseGroup, ReponseOptionNo;
}

[Serializable]
public class MaterialLog
{
    public int IdUser { get; set; }
    public int LifeNo { get; set; }
    public string FestivalName { get; set; }
    public string RawMaterial { get; set; }
    public string RawMaterialType { get; set; }
    public int RawMaterialTileNo { get; set; }
    public string RawMaterialSelected { get; set; }
}

[Serializable]
public class GetGame3UserStatus
{
    public int endIdLevel, levelCumulativeScore;
    public string endLevelName, festivalName;
}