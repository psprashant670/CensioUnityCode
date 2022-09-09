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
    public int? TimetakenToComplete { get; set; }
    public int? MedallionsCollected { get; set; }
    public int AttemptNo { get; set; }
    public int? IsCompleted { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public int? IdOrganization { get; set; }
    public string ObstacleName { get; set; }
    public int? NoOfClicks { get; set; }
    public int? NoValidclicks { get; set; }

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
public class GamePlayLog
{
    public int IdUser { get; set; }
    public string GameName { get; set; }
}

[Serializable]
public class Game5ClickLog
{
    public string GameName { get; set; }
    public int IdUser { get; set; }
    public int IdLevel { get; set; }
    public int AttemptNo { get; set; }
    public string InteractableName { get; set; }
    public int SequenceNo { get; set; }
}

[Serializable]
public class LevelData 
{
    public Difficulty difficulty;
    public int idLevel;
    public string challengeName, challengeIntroMessage;
    public int againPlayCoins, attemptsAllowed, attemptTimer, idOrganization;
    public int timeTaken, isComplete, idSubliminalMeasurement1, idBehavior, coins, attemptNumber, NoOfClicks, NoValidclicks;
    public string ObstacleName;
    public float behaviorScore;
    public bool playerDied;

    public LevelData(int _id)
    {
        difficulty = (Difficulty)_id;
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
public class PreviousLevelData
{
    public int idLevel;
    public int idBehavior;
    public float behaviorScore;
    public int attemptNo;
    public int isCompleted;
    public int medallionsCollected;
}