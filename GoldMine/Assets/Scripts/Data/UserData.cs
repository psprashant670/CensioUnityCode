using System;

[Serializable]
public class UserData
{
    public int? IdUser { get; set; }
    public int? IdGame { get; set; }
    public int? IdLevel { get; set; }
    public int? IdSubliminalMeasurement1 { get; set; }
    public int? IdSubliminalMeasurement2 { get; set; }
    public int? IdBehavior { get; set; }
    public float? BehaviorScore { get; set; }
    public int? GoldCoins { get; set; }
    public int? RewardCoins { get; set; }
    public int? TimetakenToComplete { get; set; }
    public int AttemptNo { get; set; }
    public int? IsCompleted { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public int? IdOrganization { get; set; }
    public int? NoOfMoves { get; set; }
    public int? CorrectMoves { get; set; }
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
    public Level level;
    public int idLevel;
    public string challengeName, challengeIntroMessage, bottomCompleteMessage, bottomFailMessage;
    public int againPlayCoins, attemptsAllowed, attemptTimer, idOrganization;
    public int timeTaken, isComplete, idSubliminalMeasurement1, idBehavior, coins, rewardCoins, attemptNumber, noOfMoves, correctMoves; // levelStatus 1=complete, -1=failed, 0=attempted
    public float behaviorScore;

    public LevelData(int _id)
    {
        level = (Level)_id;
    }

}

[Serializable]
public class AttemptData 
{
    public int idAttempt;
    public int idLevel;
    public int attemptNo;
    public int idGame;
    public int goldCoins;
    public int idBehavior;
    public string game5Behaviors;
    public float behaviorScore;
    public int timeInSecond;
    public int challengeCompletedTime1;
    public int rewardCoinsTime1;
    public int challengeCompletedTime2;
    public int rewardCoinsTime2;
    public string failAttemptMessage;
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

[Serializable]
public class SubliminalMeasurement2Data
{
    public int idSubliminalMeasurement2;
    public int idGame;
    public int idLevel;
    public int idBehavior;
    public string subliminalMeasurement2Name;
    public float behaviorScore;
    public int idOrganization;
    public string gameFeedbackMsg;
    public int idCmsUser;
}

[Serializable]
public class PreviousLevelData
{
    public int idLevel;
    public int idBehavior;
    public float behaviorScore;
    public int attemptNo;
    public int isCompleted;
    public int medallionsCollected;
}

[System.Serializable]
public enum Level
{
    Level1,
    Level2,
    Level3,
    Level4,
    None
}

[Serializable]
public class GamePlayLog
{
    public int IdUser { get; set; }
    public string GameName { get; set; }
}

[Serializable]
public class Game1CorrectTilesLog
{
    public string GameName { get; set; }
    public int IdUser { get; set; }
    public int IdLevel { get; set; }
    public int AttemptNo { get; set; }
    public int TileNo { get; set; }
    public int TilePosition { get; set; }
    public int Status { get; set; }
}