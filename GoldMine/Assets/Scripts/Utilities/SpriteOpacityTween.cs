using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOpacityTween : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public AnimationCurve Curve;

    void Update()
    {
        var color = SpriteRenderer.color;
        color.a = Curve.Evaluate(Time.time);
    }
}
