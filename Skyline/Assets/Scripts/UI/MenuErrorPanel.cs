using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuErrorPanel : MonoBehaviour
{
    public TextMeshProUGUI ErrorMsg_txt;

    public void ShowError(string _msg)
    {
        ErrorMsg_txt.SetText(_msg);
        gameObject.SetActive(true);
    }
}
