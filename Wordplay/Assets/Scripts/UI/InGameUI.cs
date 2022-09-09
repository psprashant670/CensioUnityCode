using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public GameObject InputField, EnterBtn, PopupPanel;
    public TMP_InputField Input;
    public TextMeshProUGUI EnteredText, TimeLeft_txt, Toast_txt;
    public Animator Toast;

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
        EnableInput(false);
        PopupPanel.SetActive(false);
        EnteredText.text = "";
    }

    public void ShowPopup()
    {
        Time.timeScale = 0;
        PopupPanel.SetActive(true);
    }

    public void EnableInput(bool _state)
    {
        InputField.SetActive(_state);
        EnterBtn.SetActive(_state);
    }

    public void UpdateInput()
    {
        string text = Input.text;
        text = text.Replace(" ","");
        Input.text = text;
    }

    public void UpdateTimeLeft(int _time)
    {
        int minutes = Mathf.FloorToInt(_time / 60f);
        int seconds = Mathf.FloorToInt(_time - minutes * 60);
        TimeLeft_txt.SetText("Time Left : "+string.Format("{0:0}:{1:00}", minutes, seconds));
    }

    public void EnterText()
    {
        string _word = Input.text;
        Input.text = "";
        Input.Select();
        Input.ActivateInputField();
        if (string.IsNullOrEmpty(_word))
        {
            return;
        }

        if (LevelManager.instance.Words.Contains(_word))
        {
            ShowWordError(_word+" has already been entered before!");
            return;
        }
        else if (_word.Length == 1)
        {
            ShowWordError("Enter at least 2 characters!");
            return;
        }
        else
        {
            GameManager.instance.Data.VerifyWord_Co(this, _word, AddValidWord);
        }
    }

    public void KeepPlaying()
    {
        Time.timeScale = 1;
        PopupPanel.SetActive(false);
    }

    public void MoveOn()
    {
        Time.timeScale = 1;
        PopupPanel.SetActive(false);
        LevelManager.instance.SubmitData();
        GameManager.instance.MenuState = MenuState.MoveOn;
        GameManager.instance.LoadScene("Menu");
    }

    void ShowWordError(string _msg)
    {
        Toast_txt.SetText(_msg);
        Toast.Play("Toast", 0);
    }

    void AddValidWord(bool _state, string _word)
    {
        if (_state)
        {
            LevelManager.instance.AddWord(_word);
            _word +="   ";
            EnteredText.text += _word;
        }
        else
        {
            ShowWordError(_word+" is not a valid word!");
        }
    }

    public void OnEnter(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            EnterText();
        }
    }

    void OnEnable() 
    {
        InputManager.Enter += OnEnter;
    }

    void OnDisable() 
    {
        InputManager.Enter -= OnEnter;
    }
}
