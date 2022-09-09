using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MapPanel : MonoBehaviour
{
    public Image L1, L2, L3, L4;
    public TextMeshProUGUI L1_txt, L2_txt, L3_txt, L4_txt, BottomMsg_txt; 
    public Sprite L1Found, L2Found, L3Found, L4Found;

    public void ShopMapPanel(bool _state)
    {
        gameObject.SetActive(_state);
    }

    public void ShowMap(int _id)
    {
        Debug.Log(_id);
        switch (_id)
        {
            case 1:
            L1_txt.gameObject.SetActive(false);
            L1.sprite = L1Found;
            break;

            case 2:
            L2_txt.gameObject.SetActive(false);
            L2.sprite = L2Found;
            break;

            case 3:
            L3_txt.gameObject.SetActive(false);
            L3.sprite = L3Found;
            break;

            case 4:
            L4_txt.gameObject.SetActive(false);
            L4.sprite = L4Found;
            break;

            default:
            break;
        }
    }

    public void SetMessage(string _msg)
    {
        BottomMsg_txt.SetText(_msg);
    }
}
