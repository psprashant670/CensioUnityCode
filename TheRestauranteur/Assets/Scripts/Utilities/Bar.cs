using UnityEngine;

public class Bar : MonoBehaviour
{
    public float RequiredQuantity;

    public void SetBar(float _value)
    {
        gameObject.SetActive(_value >= RequiredQuantity);
    }
}
