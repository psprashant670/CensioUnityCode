using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsPanel : MonoBehaviour
{
    public List<GameObject> AllPanels;
    public GameObject Result100, Result95, Result85, ResultLessThan75, TimeUp;
    public GameObject TryAgainBtn, MoveOnBtn, OkBtn;

    public void ShowResult(bool _state)
    {
        if (!_state)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (GameObject _panel in AllPanels)
        {
            _panel.SetActive(false);
        }
        OkBtn.SetActive(false);

        float per, sum = 0;
        List<Game2BlockAccuracyLog> logs = LevelManager.instance.Game2BlockAccuracyLog.FindAll((log)=> log.IsCorrect == 1);
        for (int i = 0; i < logs.Count; i++)
        {
            sum += logs[i].AccuracyLevel;
        }
        per = Mathf.Clamp01(sum/(logs.Count)/100f);
        if (float.IsNaN(per))
        {
            per = 0;
        }
        
        if (!LevelManager.instance.hasAtleastOneBlock)
        {
            TimeUp.SetActive(true);
        }
        else if (per >= 0.95)
        {
            Result100.SetActive(true);
        }
        else if (per >= 0.85)
        {
            Result95.SetActive(true);
        }
        else if (per >= 0.75)
        {
            Result85.SetActive(true);
        }
        else if (per < 0.75)
        {
            ResultLessThan75.SetActive(true);
        }

        if (Data.instance.CurrentLevel.attemptNumber >= 4 || per >= 100)
        {
            TryAgainBtn.SetActive(false);
            MoveOnBtn.SetActive(false);
            OkBtn.SetActive(true);
        }

        gameObject.SetActive(true);
    }
}
