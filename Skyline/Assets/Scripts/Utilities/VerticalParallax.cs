using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalParallax : MonoBehaviour
{
    public Transform BGImage;
    public float MinY, MaxY, MaxLevelHeight;
    float Height;
    Camera cam; 

    void Start()
    {
        cam = Camera.main;
    }

    public void ReCalculate()
    {
        Height = Mathf.Lerp(MaxY, MinY, cam.transform.position.y/MaxLevelHeight);
        Height = Mathf.Clamp(Height, MinY, MaxY);
        // BGImage.localPosition = new Vector3(BGImage.localPosition.x, Height, BGImage.localPosition.z);
        BGImage.DOKill();
        BGImage.DOLocalMoveY(Height, 0.2f);
    }
}
