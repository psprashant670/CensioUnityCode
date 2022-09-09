using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    public string BGMName;

    void Start()
    {
        StartCoroutine("PlayBGMOnSceneLoad");
    }

    IEnumerator PlayBGMOnSceneLoad()
    {
        yield return new WaitUntil( ()=> GameManager.isLoaded );
        
        if (BGMName.Equals(""))
        {
            AudioManager.instance.BGMFade(true, 1f);  
            AudioManager.instance.StopBGM();
            AudioManager.instance.BGMFade(false, 1f);  
        }
        else
        {
            AudioManager.instance.TransitionBGM(BGMName); 
        }
    }
}
