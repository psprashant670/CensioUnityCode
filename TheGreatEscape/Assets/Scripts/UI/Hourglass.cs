using UnityEngine;
using UnityEngine.UI;

public class Hourglass : MonoBehaviour
{
    public Sprite[] States;

    Image HourGlass;

    void Start() 
    {
        HourGlass = GetComponent<Image>();
    }

    public void SetHourGlass(float _percent)
    {
        if (_percent < 0.01f)
        {
            HourGlass.sprite = States[5];
        }
        else if (_percent < 0.16f)
        {
            HourGlass.sprite = States[4];
        }
        else if (_percent < 0.32f)
        {
            HourGlass.sprite = States[3];
        }
        else if (_percent < 0.50f)
        {
            HourGlass.sprite = States[2];
        }
        else if (_percent < 0.75f)
        {
            HourGlass.sprite = States[1];
        }
        else
        {
            HourGlass.sprite = States[0];
        }
        
    }
}
