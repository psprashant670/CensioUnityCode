using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3n4IntroOverride : MonoBehaviour
{
    public GameObject Next;

    void Start()
    {
        if (Data.instance.CurrentLevel.attemptNumber!=1)
        {
            gameObject.SetActive(false);
            Next.SetActive(true);
        }
    }

}
