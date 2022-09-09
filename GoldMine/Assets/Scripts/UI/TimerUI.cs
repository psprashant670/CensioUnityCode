using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Image Fill;
    
    public void SetFill(float _fill)
    {
        Fill.fillAmount = _fill;
    }
}
