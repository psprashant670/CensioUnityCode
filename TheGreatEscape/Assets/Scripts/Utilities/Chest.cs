using UnityEngine;

public class Chest : MonoBehaviour
{
    public Medallion Type;
    public Sprite EmptySprite;
    public bool updateInventoryNow = false;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag.Equals("Player"))
        {
            LevelManager.instance.hasKey = true;
            if (Type != Medallion.None)
            {
                LevelManager.instance.MedallionTypeFound = Type;
            }
            GetComponent<SpriteRenderer>().sprite = EmptySprite;
            if (updateInventoryNow)
            {
                LevelManager.instance.CheckMedallion();
            }
        }
    }
}

public enum Medallion
{
    None,
    Red,
    Green,
    Blue,
    Yellow
}