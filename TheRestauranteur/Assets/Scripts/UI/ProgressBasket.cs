using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProgressBasket : MonoBehaviour
{
    public TileType Type;
    public Transform Fill;
    public GameObject BravoPopup;
    public float BravoPopupWaitTime = 2.3f;
    public bool secondaryAcuired = false;

    WaitForSeconds BravoPopupWait;
    int popupCount = 0;
    float minQuant;
    
    void Start()
    {
        popupCount = 0;
        BravoPopup.SetActive(false);
        secondaryAcuired = false;
        foreach (Transform _child in Fill)
        {
            _child.gameObject.SetActive(false);
        }

        BravoPopupWait = new WaitForSeconds(BravoPopupWaitTime);
    }

    public void ResetBasket(float _per)
    {
        StopAllCoroutines();
        secondaryAcuired = false;
        popupCount = 0;
        // if (_per >= 1f)
        // {
        //     popupCount = 2;
        // }
        // else if (_per >= 0.75f)
        // {
        //     popupCount = 1;
        // }
        // else if (_per >= minQuant)
        // {
        //     popupCount = 0;
        // }
        BravoPopup.SetActive(false);
        foreach (Transform _child in Fill)
        {
            _child.gameObject.SetActive(false);
        }

        int amount = Mathf.RoundToInt(Fill.childCount*_per);
        for (int i = 0; i < amount; i++)
        {
            Fill.GetChild(i).gameObject.SetActive(true);
        }
        Fill.GetChild(Fill.childCount-1).gameObject.SetActive((_per >= 1f));
    }

    public void SetFill(float _per)
    {
        _per = Mathf.Clamp01(_per);
        int amount = Mathf.RoundToInt(Fill.childCount*_per);
        for (int i = 0; i < amount; i++)
        {
            Fill.GetChild(i).gameObject.SetActive(true);
        }

        Fill.GetChild(0).gameObject.SetActive((_per > 0.01f));
        Fill.GetChild(Fill.childCount-1).gameObject.SetActive((_per >= 1f));
        
        CheckQuantity(_per);
    }

    void CheckQuantity(float _per)
    {
        FestivalData current = Data.instance.Festivals.Find((fest)=> fest.FestivalType==LevelManager.instance.CurrentFestival);
        minQuant = (float)current.rawMaterialMinQtyFuture/current.rawMaterialExactqty;
        if (_per >= 1f && popupCount==2)
        {
            StartCoroutine(AnimateBravo());
            popupCount = 3;
        }
        else if (_per >= 0.75f && popupCount==1)
        {
            StartCoroutine(AnimateBravo());
            popupCount = 2;
        }
        else if (_per >= minQuant && popupCount==0)
        {
            popupCount = 1;
            if (current.FestivalType != Type)
            {
                StartCoroutine(AnimateBravo());
                ValidateSecondary();
            }
        }
    }

    void ValidateSecondary()
    {
        if (!secondaryAcuired)
        {
            secondaryAcuired = true;
            Data.instance.Score += Data.instance.secondaryScore;
            switch (Type)
            {
                case TileType.Wheat:
                LevelManager.instance.festivalData.WheatSecAquired = true;
                break;

                case TileType.Grape:
                LevelManager.instance.festivalData.GrapeSecAquired = true;
                break;

                case TileType.Chocolate:
                LevelManager.instance.festivalData.ChocolateSecAquired = true;
                break;

                case TileType.Milk:
                LevelManager.instance.festivalData.MilkSecAquired = true;
                break;
            }
        }
        InGameUI.instance.SetScore();
    }

    IEnumerator AnimateBravo()
    {
        BravoPopup.transform.localScale = Vector3.zero;
        BravoPopup.SetActive(true);
        BravoPopup.transform.DOScale(Vector3.one, 0.3f);
        yield return BravoPopupWait;
        BravoPopup.transform.DOScale(Vector3.zero, 0.3f);
        BravoPopup.SetActive(false);
    }
}
