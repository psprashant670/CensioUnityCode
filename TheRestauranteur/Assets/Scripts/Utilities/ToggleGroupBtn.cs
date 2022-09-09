using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleGroupBtn : MonoBehaviour
{
    public string GroupName;
    public Image A, B;
    public Sprite A_UnChecked, B_UnChecked, A_Checked, B_Checked;
    public string SelectedOption = "";
    public TextMeshProUGUI Percentage_txt, A_txt, B_txt;
    public int groupID;
    public QuestionResponses RespA, RespB;

    void Start()
    {
        groupID = int.Parse(gameObject.name);
        Percentage_txt = transform.Find("percentage").GetComponent<TextMeshProUGUI>();
        A_txt = A.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        B_txt = B.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Percentage_txt.SetText(Data.instance.Responses.Find((res)=> res.idResponseGroup==groupID).additionInformation);
        RespA = Data.instance.Responses.Find((res)=> res.idResponseGroup==groupID && res.reponseOptionNo==1);
        RespB = Data.instance.Responses.Find((res)=> res.idResponseGroup==groupID && res.reponseOptionNo==2);
        A_txt.SetText(RespA.responseOptionDescription);
        B_txt.SetText(RespB.responseOptionDescription);

        A.sprite = A_UnChecked;
        B.sprite = B_UnChecked;
        SelectedOption = "";
    }

    public void SetValue(string _val)
    {
        SelectedOption = _val;
        if (_val.Equals("A"))
        {
            A.sprite = A_Checked;
            B.sprite = B_UnChecked;
        }
        else
        {
            A.sprite = A_UnChecked;
            B.sprite = B_Checked;
        }

        QuestionsPanel.instance.CheckIfAllSelected();
    }
}
