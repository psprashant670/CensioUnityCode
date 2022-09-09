using System;
using System.Collections.Generic;

[Serializable]
public class Game4Userlog
{
    public int IdUser { get; set; }
    public int IdGame { get; set; }
    public int BonusPoint { get; set; }
    public int CountofWordsMade { get; set; }
    public int IdLevelMasterid { get; set; }
    public int TimeTaken { get; set; }
    public string WordSelected { get; set; }
    
}

[Serializable]
public class GamePlayLog
{
    public int IdUser { get; set; }
    public string GameName { get; set; }
}

[Serializable]
public class Game4WordsLog
{
    public int IdUser { get; set; }
    public int IdGame { get; set; }
    public int IdLevel { get; set; }
    public string wordmadedetail { get; set; }
    public string WordSelected { get; set; }
    
}

[Serializable]
public class UserPurchaseLog
{
    
    public int? IdUser { get; set; }
    public int? IdGame { get; set; } // const
    public int? IdLevel { get; set; }
    public int? Coins { get; set; }
    public int AttemptNo { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public int? IdOrganization { get; set; } // const?
}

[Serializable]
public class LevelData 
{
    public SelectedLevel SelectedLevel;
    public int idLevel;
    public string levelName, idLevelStatus;
    public int levelWordcount, levelTimerequired, levelBonuspts;
    public float behaviorScore;
    public List<string> CardWords = new List<string>(), UserWords = new List<string>();
    public string WordSelected;
    public int timeTaken, isComplete, idSubliminalMeasurement1, idBehavior, bonusGranted, userWordsGenerated;

    public LevelData(int _id)
    {
        SelectedLevel = (SelectedLevel)_id;
    }

}

[Serializable]
public class SubliminalMeasurement1Data
{
    public int idSubliminalMeasurement1;
    public int idGame;
    public int idBehavior;
    public string subliminalMeasurementName;
    public float behaviorScore;
    public int idOrganization;
    public string status;
    public int idCmsUser;
}

[System.Serializable]
public enum SelectedLevel
{
    None,
    Level1,
    Level2,
    Level3
}