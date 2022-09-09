using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadInstruction : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int index;

    void OnEnable() 
    {
        if (index < Data.instance.Instructions.Count && !string.IsNullOrEmpty(Data.instance.Instructions[index]))
        {
            Text.SetText(Data.instance.Instructions[index]);
        }
    }
}
