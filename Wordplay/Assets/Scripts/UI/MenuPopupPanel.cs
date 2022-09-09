using UnityEngine;

public class MenuPopupPanel : MonoBehaviour
{
    public GameObject TimeUp, Victory, GameOver;

    public void HideAll()
    {
        TimeUp.SetActive(false);
        Victory.SetActive(false);
        GameOver.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowTimeUp(bool _state)
    {
        HideAll();
        gameObject.SetActive(_state);
        TimeUp.SetActive(_state);
    }

    public void ShowVictory(bool _state)
    {
        HideAll();
        gameObject.SetActive(_state);
        Victory.SetActive(_state);
    }

    public void ShowGameOver(bool _state)
    {
        HideAll();
        gameObject.SetActive(_state);
        GameOver.SetActive(_state);
    }
}
