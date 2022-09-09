using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    public GameObject gfx_true, gfx_false;

    bool Sound, Path;

    void Start() 
    {
        SetupSound();
    }

    public void ToggleMute()
    {
        Sound = !Sound;
        PlayerPrefs.SetInt("Sound", Sound ? 1:0 );

        if (Sound)
        {
            gfx_true.SetActive(false);
            gfx_false.SetActive(true);
            AudioManager.instance.ToggleSound(Sound);
        }
        else
        {
            gfx_true.SetActive(true);
            gfx_false.SetActive(false);
            AudioManager.instance.ToggleSound(Sound);
        }
        // Debug.Log(Sound);
    }

    void SetupSound()
    {
        Sound = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true:false ;
        if (Sound)
        {
            gfx_true.SetActive(false);
            gfx_false.SetActive(true);
            AudioManager.instance.ToggleSound(Sound);
        }
        else
        {
            gfx_true.SetActive(true);
            gfx_false.SetActive(false);
            AudioManager.instance.ToggleSound(Sound);
        }
        // Debug.Log(Sound);
    }
}
