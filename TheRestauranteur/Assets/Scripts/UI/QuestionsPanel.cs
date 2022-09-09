using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionsPanel : MonoBehaviour
{
    public static QuestionsPanel instance;

    public TextMeshProUGUI Question_txt;
    public List<ToggleGroupBtn> Options;
    public GameObject GameOverPanel, SubmitButton;

    void Awake() 
    {
        instance = this;
    }

    void Start() 
    {
        GameOverPanel.SetActive(false);
        SubmitButton.SetActive(false);
        Question_txt.SetText(Data.instance.Question);
    }

    public void SetQuestion(string _question)
    {
        Question_txt.SetText(_question);
    }

    public void OnSubmit()
    {
        List<Responses> responses = new List<Responses>();
        for (int i = 0; i < Options.Count; i++)
        {
            Responses response = new Responses();
            response.IdUser = Data.instance.UID;
            response.IdQuestion = 1;
            response.IdResponseGroup = Options[i].groupID;
            response.ReponseOptionNo = Options[i].SelectedOption.Equals("A") ? 1 : 2;
            response.IdResponse = Options[i].SelectedOption.Equals("A") ? Options[i].RespA.idResponse : Options[i].RespB.idResponse;
            responses.Add(response);
        }
        Data.instance.PostGame3ResponseLog_Co(GameManager.instance, responses);
        GameOverPanel.SetActive(true);
    }

    public void CheckIfAllSelected()
    {
        for (int i = 0; i < Options.Count; i++)
        {
            if (string.IsNullOrEmpty(Options[i].SelectedOption))
            {
                return;
            }
        }

        SubmitButton.SetActive(true);
    }
}
