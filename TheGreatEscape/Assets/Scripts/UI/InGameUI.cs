using UnityEngine;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    public PopUpPanel PopupPanel;
    public TextMeshProUGUI Attempts, Coins, Medallions;
    public Hourglass Hourglass;

    void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        PopupPanel.HideAllPanels();
        PopupPanel.gameObject.SetActive(false);
        SetCoins(Data.instance.Coins);
        Medallions.SetText(Data.instance.MedallionCount.ToString());
    }
        
    public void SetAttemptNo(int _no)
    {
        Attempts.SetText("Attempt : "+ _no);
    }

    public void SetCoins(int _coins)
    {
        Coins.SetText(_coins.ToString());
    }

    public void NotEnoughMedallions()
    {
        PopupPanel.ShowMedallion(false);
        // GameManager.instance.MenuState = MenuState.NotEnoughMedallions;
        GameManager.instance.MenuState = MenuState.Dead;
        GameManager.instance.LoadScene("Menu");
    }
}
