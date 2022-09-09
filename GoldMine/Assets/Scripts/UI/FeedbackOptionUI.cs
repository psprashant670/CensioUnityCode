using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FeedbackOptionUI : MonoBehaviour
{
    public TextMeshProUGUI Option_txt;
    public GameObject CheckBox, CheckMark;
    public SubliminalMeasurement2Data SubliminalMeasurement2Data;
    public bool isChecked = false;
    public string ID;

    public void SetData()
    {
        Option_txt.SetText(SubliminalMeasurement2Data.gameFeedbackMsg);
        ID = SubliminalMeasurement2Data.subliminalMeasurement2Name.Substring(0, 1);
    }

    public void SetOption(bool _isChecked)
    {
        isChecked = _isChecked;
        CheckBox.SetActive(!_isChecked);
        CheckMark.SetActive(_isChecked);
    }
}
