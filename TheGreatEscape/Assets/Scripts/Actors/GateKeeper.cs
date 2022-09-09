using UnityEngine;

public class GateKeeper : MonoBehaviour
{
    Data Data;

    void Start() 
    {
        Data = Data.instance;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("Player"))
        {
            if (!Data.RedMedallion || !Data.GreenMedallion || !Data.BlueMedallion || !Data.YellowMedallion)
            {
                LevelManager.instance.SetLevelFailedResult();
                InGameUI.instance.PopupPanel.ShowMedallion(true);
                // //Linear fix
                // Data.instance.PreviousLevelData.idLevel++;
            }
            else if (Data.RedMedallion && Data.GreenMedallion && Data.BlueMedallion && Data.YellowMedallion && LevelManager.instance.hasKey)
            {
                GameManager.instance.MenuState = MenuState.Success;
                LevelManager.instance.SetLevelResult();
                InGameUI.instance.PopupPanel.ShowVictoryPanel(true);
            }
        }
    }
}
