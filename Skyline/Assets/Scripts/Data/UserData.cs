using System;
using System.Collections.Generic;

[Serializable]
public class GamePlayLog
{
    public int IdUser { get; set; }
    public string GameName { get; set; }
}

[Serializable]
public class UserData
{
    public int IdUser { get; set; }
    public int IdGame { get; set; }
    public int IdLevel { get; set; }
    public int IdSubliminalMeasurement1 { get; set; }
    public int IdSubliminalMeasurement2 { get; set; }
    public int IdBehavior { get; set; }
    public float BehaviorScore { get; set; }
    public int AccuracyLevel { get; set; }
    public int RewardCoins { get; set; }
    public int TimetakenToComplete { get; set; }
    public int AttemptNo { get; set; }
    public int IsCompleted { get; set; }
    public DateTime UpdatedDateTime { get; set; }
    public int IdOrganization { get; set; }
    public int BlocksPresented { get; set; }
    public int BlocksClicked { get; set; }
    public int CorrectBlocks { get; set; }
    public int SuccessfulBlocks { get; set; }
}

[Serializable]
public class Game2BlockAccuracyLog
{
    public string GameName { get; set; }
    public int IdUser;
    public int IdLevel;
    public int AttemptNo;
    public int BlockNo;
    public int AccuracyLevel;
    public int IsCorrect;

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
    public string levelName, challengeIntroMessage;
    public int levelTimerequired, levelStatus, attemptsAllowed, attemptTimer;
    public float behaviorScore;
    public int timeTaken, isComplete, idSubliminalMeasurement1, idBehavior, accuracy, attemptNumber, blocksPresented, blocksClicked, correctBlocks, successfulBlocks, rewardCoins;
    public bool isPreviouslyComplete;

    public LevelData(int _id)
    {
        SelectedLevel = (SelectedLevel)_id;
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
    // public string game5Behaviors;
    public float behaviorScore;
    // public int timeInSecond;
    // public int challengeCompletedTime1;
    public int rewardCoinsTime1;
    // public int challengeCompletedTime2;
    // public int rewardCoinsTime2;
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

[System.Serializable]
public enum SelectedLevel
{
    None,
    Level1,
    Level2,
    Level3,
    Level4
}