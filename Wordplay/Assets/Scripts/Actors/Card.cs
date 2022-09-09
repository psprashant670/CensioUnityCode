using UnityEngine;
using DG.Tweening;
using TMPro;

public class Card : MonoBehaviour
{
    public Sprite Used, Flipped;
    public TextMeshPro Word;

    SpriteRenderer SpriteRenderer;
    Collider2D Collider2D;
    float scaleX;
    string word;

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D = GetComponent<Collider2D>();
        Word.gameObject.SetActive(false);
        scaleX = transform.localScale.x;
    }

    public void TryPlay()
    {
        LevelManager.instance.DisableCards();
        GameManager.instance.Data.CurrentLevel.WordSelected = word;
        transform.DOScaleX(0, 0.5f).OnComplete(()=> {
            SpriteRenderer.sprite = Flipped;
            Word.gameObject.SetActive(true);
            LevelManager.instance.levelHasBegun = true;
            transform.DOScaleX(scaleX, 0.5f).OnComplete(()=> InGameUI.instance.EnableInput(true));
        });
    }

    public void SetUsed()
    {
        SpriteRenderer.sprite = Used;
        Collider2D.enabled = false;
    }

    public void SetWord(string _word)
    {
        word = _word;
        Word.SetText(word);
    }
}
