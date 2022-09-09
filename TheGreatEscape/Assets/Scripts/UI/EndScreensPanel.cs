using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreensPanel : MonoBehaviour
{
    public Data Data;
    public GameObject TimeRanOutPanel, DeadPanel, SuccessPanel, FailedPanel;
    public List<TextMeshProUGUI> Titles;

    void Start()
    {
        TimeRanOutPanel.SetActive(false);
        DeadPanel.SetActive(false);
        SuccessPanel.SetActive(false);

        switch (GameManager.instance.MenuState)
        {
            case MenuState.TimeRanOut:
            TimeRanOutPanel.SetActive(true);
            break;

            case MenuState.Dead:
            DeadPanel.SetActive(true);
            break;

            case MenuState.Success:
            SuccessPanel.SetActive(true);
            break;

            case MenuState.Failed:
            FailedPanel.SetActive(true);
            break;

            case MenuState.GaveUp:
            FailedPanel.SetActive(true);
            break;

            default:
            break;
        }

        foreach (TextMeshProUGUI _title in Titles)
        {
            _title.SetText(Data.CurrentLevel.challengeName);
        }
    }
}
