using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public SelectedLevel Level;
    public GameObject SelectionCircle, SelectedHighlight, Blocker;
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
        SelectedHighlight.SetActive(true);
        Data.instance.SelectedLevel = Level;
    }

    public void Deselect()
    {
        SelectionCircle.SetActive(false);
        SelectedHighlight.SetActive(false);
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
            LevelStatus_txt.SetText("Attempt Left "+Mathf.Abs(_attemptNo-4));

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
            Button.enabled = false;
            Deselect();
            break;

            case -1:
            LevelStatus_txt.SetText("Level Failed");
            Blocker.SetActive(true);
            Button.enabled = false;
            Deselect();
            break;

            default:
            break;
        }
    }

    public void SetLocked()
    {
        // LevelStatus_txt.SetText("");
        Blocker.SetActive(true);
        Button.enabled = false;
        Deselect();
    }
}
