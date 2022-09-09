using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image AccuracyBarFill;

    public void SetFill(float _per)
    {
        AccuracyBarFill.fillAmount = _per;
    }
}
