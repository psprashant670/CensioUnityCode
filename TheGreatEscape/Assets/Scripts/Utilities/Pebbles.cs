using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pebbles : MonoBehaviour
{
    public List<Rigidbody2D> PebblesRb;
    public List<Guard> TriggerGaurds;
    public Transform AreaToInvestigate;
    public GameObject FX;
    public float PlayFXDelay;

    bool isTriggered = false;

    public void Activate()
    {
        if (isTriggered)
        {
            return;
        }
        isTriggered = true;

        foreach (Rigidbody2D _rb in PebblesRb)
        {
            _rb.isKinematic = false;
        }

        StartCoroutine(VanishAfter(3f));
        StartCoroutine(AleartGaurds(PlayFXDelay));
    }

    IEnumerator VanishAfter(float _time)
    {
        yield return new WaitForSeconds(_time);
        foreach (Rigidbody2D _rb in PebblesRb)
        {
            _rb.transform.DOScale(Vector3.zero, 1f).OnComplete(()=> { _rb.gameObject.SetActive(false); } );
        }
    }

    IEnumerator AleartGaurds(float _time)
    {
        yield return new WaitForSeconds(_time);
        FX.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        foreach (Guard _guard in TriggerGaurds)
        {
            _guard.InvestigateArea(AreaToInvestigate);
        }
    }
}
