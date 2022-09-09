using UnityEngine;

public class Exit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("Player") && LevelManager.instance.hasKey)
        {
            GameManager.instance.MenuState = MenuState.Success;
            LevelManager.instance.SetLevelResult();
            GameManager.instance.LoadScene("Menu");
        }
    }
}
