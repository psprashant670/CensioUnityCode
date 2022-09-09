using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<Card> Cards;
    public int WordsRequired = 1;
    public GameObject Sparkle;
    public TextMeshPro Bonus_txt;
    public List<string> Words { get { return Data.instance.CurrentLevel.UserWords; } }
    public bool levelHasBegun { get; set; }

    [SerializeField]
    float TimeLeft = 0f, MaxTime;
    [HideInInspector]
    public int AttemptNo, Coins;
    bool wordsTargetAchieved = false;
    LayerMask InteractableLayer;
    Data Data;
    InGameUI InGameUI;

    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        Words.Clear();
        levelHasBegun = false;
        Sparkle.SetActive(false);
        wordsTargetAchieved = false;
        InGameUI = InGameUI.instance;
        Data = GameManager.instance.Data;
        InteractableLayer = LayerMask.GetMask("Interact");

        LoadLevelData();

        // InGameUI.instance.SetAttemptNo(AttemptNo);
    }

    void FixedUpdate() 
    {
        UpdateTimer();
    }

    public void LoadLevelData()
    {
        MaxTime = Data.CurrentLevel.levelTimerequired; 
        TimeLeft = MaxTime;
        WordsRequired = Data.CurrentLevel.levelWordcount;
        InGameUI.UpdateTimeLeft((int)MaxTime);
        for (int i = 0; i < Cards.Count; i++)
        {
            Cards[i].SetWord(Data.CurrentLevel.CardWords[i]);
        }
    }

    public void OnTouch(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue()), Vector2.zero, 100f, InteractableLayer);
            if(hit.collider != null)
            {
                hit.collider.GetComponent<Card>()?.TryPlay();
            }
        }
    }

    public void DisableCards()
    {
        foreach (Card _card in Cards)
        {
            _card.SetUsed();
        }
    }

    public void AddWord(string _word)
    {
        Words.Add(_word);
        // if target of words achieved
        if (Words.Count >= WordsRequired && !wordsTargetAchieved)
        {
            Sparkle.SetActive(true);
            wordsTargetAchieved = true;
            InGameUI.instance.ShowPopup();
            Bonus_txt.SetText("+ "+Data.CurrentLevel.levelBonuspts);
            Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
            Data.CurrentLevel.bonusGranted = Data.CurrentLevel.levelBonuspts;
        }
    }

    public void SubmitData()
    {
        Data.CurrentLevel.userWordsGenerated = Words.Count;
        Data.LastLevelID = Data.CurrentLevel.idLevel;
        GameManager.instance.PostUserAttempt();
    }

    // --------------------------------- UI ---------------------------------
    void UpdateTimer()
    {
        if (levelHasBegun)
        {
            TimeLeft -= Time.deltaTime;
            // InGameUI.instance.Hourglass.SetHourGlass(TimeLeft/MaxTime);
            InGameUI.UpdateTimeLeft((int)TimeLeft);
            if (TimeLeft < -0.1f)
            {
                if (!wordsTargetAchieved)
                {
                    Data.CurrentLevel.timeTaken = (int)(MaxTime - TimeLeft);
                }
                SubmitData();
                GameManager.instance.MenuState = MenuState.TimeUp;
                GameManager.instance.LoadScene("Menu");
            }
        }
    }

    void OnEnable() 
    {
        InputManager.MouseClick += OnTouch;
    }

    void OnDisable() 
    {
        InputManager.MouseClick -= OnTouch;
    }
}
