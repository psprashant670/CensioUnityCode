using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequiredColorUI : MonoBehaviour
{
    public Image ColorWheel;
    public Color Grey, Green, Red, Orange, Cherry, Blue;

    public void SetColor(BlockColor _selected)
    {
        switch (_selected)
        {
            case BlockColor.Grey:
            ColorWheel.color = Grey;
            break;

            case BlockColor.Green:
            ColorWheel.color = Green;
            break;

            case BlockColor.Red:
            ColorWheel.color = Red;
            break;

            case BlockColor.Orange:
            ColorWheel.color = Orange;
            break;

            case BlockColor.Cherry:
            ColorWheel.color = Cherry;
            break;

            case BlockColor.Blue:
            ColorWheel.color = Blue;
            break;

            default:
            break;
        }
    }
}
