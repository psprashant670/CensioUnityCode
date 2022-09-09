using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;
    public AudioSource SFX_as, BGM_as;
    public Sound[] SFX;
    public Sound[] BGM;

    [HideInInspector]
    public bool Sound { get; set; }

    bool loopBGM = false;
    string activeBGM = "";

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }    
    }

    void Start() 
    {
        audioMixer.SetFloat("Master", LinearToDecibel(PlayerPrefs.GetFloat("Master", 1f)));
        audioMixer.SetFloat("SFX", LinearToDecibel(PlayerPrefs.GetFloat("SFX", 1f)));
        audioMixer.SetFloat("BGM", LinearToDecibel(PlayerPrefs.GetFloat("BGM", 1f)));

        // 1 = true, 0 = false
        Sound = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true:false ;
        if (!Sound)
        {
            audioMixer.SetFloat("Master", LinearToDecibel(0));
        }
    }

    public void SetVolume(float _val)
    {
        PlayerPrefs.SetFloat("Master", _val);
        audioMixer.SetFloat("Master", LinearToDecibel(_val));
    }

    public void PlayBGM(string _name)
    {
        Sound s = Array.Find(BGM, bgm => bgm.name == _name);

        if (s == null)
        {
            Debug.Log("Audio "+_name+" doesnt exist!");
            return;
        }
        
        s.source = BGM_as;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
        s.source.Play();

        activeBGM = _name;
    }

    public void StopBGM()
    {
        activeBGM = "";
        BGM_as.Stop();
    }

    public void PauseBGM(bool _val)
    {
        if (_val)
        {
            BGM_as.Pause();
        }
        else
        {
            BGM_as.Play();
        }
    }

    public void PlaySfx(string name, float _pitch)
    {
        Sound s = Array.Find(SFX, sfx => sfx.name == name);

        if (s == null)
        {
            Debug.Log("SFX "+name+" doesnt exist!");
            return;
        }
        
        s.source = SFX_as;
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.loop = s.loop;
        s.source.pitch = _pitch;
        s.source.Play();
    }

    public void TransitionBGM(string _bgm)
    {
        if (activeBGM.Equals(_bgm))
        {
            return;
        }
        PlayBGM(_bgm);
    }

    public void BGMFade(bool _fadeOut, float _duration)
    {
        if (_fadeOut)
        {
            StartCoroutine(FadeOutBgm(_duration));
        }
        else
        {
            StartCoroutine(FadeInBgm(_duration));
        }
    }

    public void ToggleSound(bool _val)
    {
        if (_val)
        {
            audioMixer.SetFloat("Master", LinearToDecibel(1));
        }
        else
        {
            audioMixer.SetFloat("Master", LinearToDecibel(0));
        }
    }

    // helper to conver from linear to log scale for audio
    private float LinearToDecibel(float linear)
    {
        float dB;
        
        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;
        
        return dB;
    }

    // IEnumerator BGMTransition(string _newTrack)
    // {
    //     // StartCoroutine(FadeOutBgm());
    //     yield return new WaitForSecondsRealtime(2.2f);
    //     PlayBGM(_newTrack);
    //     // StartCoroutine(FadeInBgm());
    // }

    IEnumerator FadeOutBgm(float _duration)
    {
        float timer = _duration;

        while (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            audioMixer.SetFloat("BGM", LinearToDecibel(Mathf.Clamp01(timer/_duration)));
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);
    }

    IEnumerator FadeInBgm(float _duration)
    {
        float timer = 0;

        while (timer < _duration)
        {
            timer += Time.unscaledDeltaTime;
            audioMixer.SetFloat("BGM", LinearToDecibel(Mathf.Clamp01(timer/_duration)));
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.2f);
    }
}


// Sound Class
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1;
    [HideInInspector]
    public AudioSource source;
    public bool loop;
}
