using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Slider GameVolume;
    public Toggle MuteToggle;

    // Start is called before the first frame update
    void OnEnable()
    {
        GameVolume.value = PlayerPrefs.GetFloat("Master", 1f);
        OnGameVolumeChange(GameVolume.value);
        // MuteToggle.isOn = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true:false;
    }

    public void OnGameVolumeChange(float _val)
    {
        AudioManager.instance.SetVolume(_val);
    }

    public void OnToggle(bool _state)
    {
        AudioManager.instance.ToggleSound(_state);
        PlayerPrefs.SetInt("Sound", _state ? 1:0);
    }

    public void SetPanelState(bool _state)
    {
        gameObject.SetActive(_state);
    }
}
