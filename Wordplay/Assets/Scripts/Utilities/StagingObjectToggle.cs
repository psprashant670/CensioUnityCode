using UnityEngine;

public class StagingObjectToggle : MonoBehaviour
{
    public GameObject NormalObject, StagingObject;

    void Awake() 
    {
        SetObject();
    }

    void OnEnable() 
    {
        SetObject();
    }

    void SetObject()
    {
        if (GameManager.instance.Data.isStagingDomain)
        {
            StagingObject.SetActive(true);
            NormalObject.SetActive(false);
        }
        else
        {
            StagingObject.SetActive(false);
            NormalObject.SetActive(true);
        }
    }
}
