using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public Level Level;
    public GameObject SelectionCircle, Blocker, AttemptsParent;
    public LevelSelect LevelSelect { get; set; }
    public TextMeshProUGUI LevelStatus_txt;
    Button Button;


    void Start()
    {
        
    }

    public void SelectLevel()
    {
        LevelSelect.DeselectLevels();
        SelectionCircle.SetActive(true);
        Data.instance.SelectedLevel = Level;
    }

    public void Deselect()
    {
        SelectionCircle.SetActive(false);
    }

    public void SetStatus(int _state, int _attemptNo)
    {
        Button = GetComponent<Button>();

        switch (_state)
        {
            case 0:
            LevelStatus_txt.SetText("");
            Blocker.SetActive(false);
            _attemptNo = Mathf.Clamp(_attemptNo, 0, 4);
            foreach (Transform _attempt in AttemptsParent.transform)
            {
                _attempt.gameObject.SetActive(true);
            }
            for (int i = 0; i < _attemptNo; i++)
            {
                AttemptsParent.transform.GetChild(i).gameObject.SetActive(false);
            }
            Button.enabled = true;
            if (_attemptNo>=4)
            {
                Blocker.SetActive(true);
                Button.enabled = false;
                LevelStatus_txt.SetText("Level Failed");
            }
            break;

            case 1:
            LevelStatus_txt.SetText("Level Complete");
            Blocker.SetActive(true);
            AttemptsParent.SetActive(false);
            Button.enabled = false;
            Deselect();
            break;

            case -1:
            LevelStatus_txt.SetText("Level Failed");
            Blocker.SetActive(true);
            AttemptsParent.SetActive(false);
            Button.enabled = false;
            Deselect();
            break;

            default:
            break;
        }
    }

    public void SetLocked()
    {
        LevelStatus_txt.SetText("");
        Blocker.SetActive(true);
        Button.enabled = false;
        Deselect();
    }
}
