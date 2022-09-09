using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public Transform Fill;
    public List<GameObject> Levels, Popups;
    public TextMeshProUGUI lvl1Target_txt, lvl2Target_txt, lvl3Target_txt, lvl4Target_txt;
    int level1Clearence, level2Clearence, level3Clearence, level4Clearence;
    int levelReached = 0;
    
    void Start()
    {
        foreach (Transform _child in Fill)
        {
            _child.gameObject.SetActive(false);
        }

        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].SetActive(false);
            Popups[i].SetActive(false);
        }

        level1Clearence = Data.instance.Festivals.Find((fest)=>fest.FestivalType==TileType.Wheat).levelClearenceAmount;
        Fill.GetChild(0).GetComponent<Bar>().RequiredQuantity = (level1Clearence/3f);
        Fill.GetChild(1).GetComponent<Bar>().RequiredQuantity = (level1Clearence/3f)*2;
        Fill.GetChild(2).GetComponent<Bar>().RequiredQuantity = level1Clearence;
        level2Clearence = Data.instance.Festivals.Find((fest)=>fest.FestivalType==TileType.Grape).levelClearenceAmount;
        Fill.GetChild(3).GetComponent<Bar>().RequiredQuantity = ((level2Clearence-level1Clearence)/3f)+level1Clearence;
        Fill.GetChild(4).GetComponent<Bar>().RequiredQuantity = (((level2Clearence-level1Clearence)/3f)*2)+level1Clearence;
        Fill.GetChild(5).GetComponent<Bar>().RequiredQuantity = level2Clearence;
        level3Clearence = Data.instance.Festivals.Find((fest)=>fest.FestivalType==TileType.Chocolate).levelClearenceAmount;
        Fill.GetChild(6).GetComponent<Bar>().RequiredQuantity = ((level3Clearence-level2Clearence)/3f)+level2Clearence;
        Fill.GetChild(7).GetComponent<Bar>().RequiredQuantity = (((level3Clearence-level2Clearence)/3f)*2)+level2Clearence;
        Fill.GetChild(8).GetComponent<Bar>().RequiredQuantity = level3Clearence;
        level4Clearence = Data.instance.Festivals.Find((fest)=>fest.FestivalType==TileType.Milk).levelClearenceAmount;
        Fill.GetChild(9).GetComponent<Bar>().RequiredQuantity = ((level4Clearence-level3Clearence)/3f)+level3Clearence;
        Fill.GetChild(10).GetComponent<Bar>().RequiredQuantity = (((level4Clearence-level3Clearence)/3f)*2)+level3Clearence;
        Fill.GetChild(11).GetComponent<Bar>().RequiredQuantity = level4Clearence;

        lvl1Target_txt.SetText(level1Clearence.ToString());
        lvl2Target_txt.SetText(level2Clearence.ToString());
        lvl3Target_txt.SetText(level3Clearence.ToString());
        lvl4Target_txt.SetText(level4Clearence.ToString());
        // SetBarFill(Data.instance.Score);
    }

    public void SetFill(float _per)
    {
        _per = Mathf.Clamp01(_per);
        int amount = Mathf.RoundToInt(Fill.childCount*_per);
        for (int i = 0; i < amount; i++)
        {
            Fill.GetChild(i).gameObject.SetActive(true);
        }

        Fill.GetChild(Fill.childCount-1).gameObject.SetActive((_per >= 0.99f));
    }

    public void SetBarFill(float _val)
    {
        foreach (Transform _child in Fill)
        {
            _child.gameObject.GetComponent<Bar>().SetBar(_val);
        }

        FestivalData festivalData = Data.instance.Festivals.Find((fest) => fest.FestivalType == LevelManager.instance.CurrentFestival);

        if (_val >= level4Clearence && levelReached <= 3)
        {
            levelReached = 4;
            SetLevel(levelReached);
            festivalData.FestivalProgress = TileType.Milk;
            LevelManager.instance.CurrentLevelProgress = TileType.Milk;
        }
        else if (_val >= level3Clearence && levelReached <= 2)
        {
            levelReached = 3;
            SetLevel(levelReached);
            festivalData.FestivalProgress = TileType.Chocolate;
            LevelManager.instance.CurrentLevelProgress = TileType.Chocolate;
        }
        else if (_val >= level2Clearence && levelReached <= 1)
        {
            levelReached = 2;
            SetLevel(levelReached);
            festivalData.FestivalProgress = TileType.Grape;
            LevelManager.instance.CurrentLevelProgress = TileType.Grape;
        }
        else if (_val >= level1Clearence && levelReached <= 0)
        {
            levelReached = 1;
            SetLevel(levelReached);
            festivalData.FestivalProgress = TileType.Wheat;
            LevelManager.instance.CurrentLevelProgress = TileType.Wheat;
        }

        InGameUI.instance.CongratzPanel.unlockedLevel = levelReached;
    }

    // 1 - 4, if 4 dont show levels..
    public void SetLevel(int _level)
    {
        foreach (GameObject lvl in Levels)
        {
            lvl.SetActive(false);
        }

        if (_level >= 4)
        {
            return;
        }

        Levels[_level-1].SetActive(true);
        StartCoroutine(AnimatePopup(Popups[_level-1]));
    }

    IEnumerator AnimatePopup(GameObject _popup)
    {
        _popup.SetActive(true);
        _popup.transform.localScale = Vector3.zero;
        _popup.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(2.3f);
        _popup.transform.DOScale(Vector3.zero, 0.3f);
        _popup.SetActive(false);
    }
}
