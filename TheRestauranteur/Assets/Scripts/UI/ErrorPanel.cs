using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPanel : MonoBehaviour
{
    public TextMeshProUGUI ErrorMsg_txt;

    public void ShowError(string _msg)
    {
        gameObject.SetActive(true);
        ErrorMsg_txt.SetText(_msg);
    }
}
