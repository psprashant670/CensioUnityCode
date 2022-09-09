using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;

public class BuildingBlock : MonoBehaviour
{
    public BlockColor Color;
    public float Accuracy { get { return m_Accuracy; } }
    public bool isClicked = false, hasLanded = false, isLogged = false;
    public Action<BuildingBlock> Touched, Landed;
    public Rigidbody2D rb2D;
    public Collider2D Collider2D;
    public GameObject FX;

    float maxWidth, m_Accuracy;
    int sequenceNo = 0, IsCorrect = 0;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        maxWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        
    }

    public void OnSpawn(int _sequenceNo)
    {
        sequenceNo = _sequenceNo;
        isClicked = false;
        hasLanded = false;
        isLogged = false;
        rb2D.isKinematic = true;
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        Collider2D.isTrigger = true;
        m_Accuracy = 0;
    }

    public void SetRigid()
    {
        if (!isClicked)
        {
            Data.instance.CurrentLevel.blocksClicked++;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.001f*Data.instance.CurrentLevel.blocksPresented);
            Collider2D.isTrigger = false;
            isClicked = true;
            hasLanded = false;
            transform.parent = null;
            rb2D.isKinematic = false;
            Touched?.Invoke(this);
        }
    }

    public void SetKinematic()
    {
        hasLanded = true;
        rb2D.isKinematic = true;
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;
        FX?.SetActive(true);
    }

    public void SetCorrectBlock()
    {
        if (LevelManager.instance.RequiredBlockColor == Color)
        {
            Data.instance.CurrentLevel.correctBlocks++;
            IsCorrect = 1;
        }
    }

    public void AddRemainingLogs()
    {
        if (!isLogged && gameObject.activeSelf && !hasLanded)
        {
            CreateLog();
        }
    }

    public void CreateLog()
    {
        if (!LevelManager.instance.hasBegun)
        {
            return;
        }
        Game2BlockAccuracyLog log = new Game2BlockAccuracyLog();
        log.AccuracyLevel = Mathf.RoundToInt(m_Accuracy);
        log.BlockNo = sequenceNo;
        // log.successful = m_Accuracy>=Data.instance.AccuracyRequired ? 1 : 0;
        log.IsCorrect = IsCorrect;
        log.IdLevel = Data.instance.CurrentLevel.idLevel;
        log.AttemptNo = Data.instance.CurrentLevel.attemptNumber;
        log.IdUser = Data.instance.UID;
        log.GameName = Data.instance.gameName;
        LevelManager.instance.Game2BlockAccuracyLog.Add(log);
        isLogged = true;

        // update UI
        float per, sum = 0;
        List<Game2BlockAccuracyLog> logs = LevelManager.instance.Game2BlockAccuracyLog.FindAll((log)=> log.IsCorrect == 1);
        for (int i = 0; i < logs.Count; i++)
        {
            sum += logs[i].AccuracyLevel;
        }
        per = Mathf.Clamp01(sum/(logs.Count)/100f);
        InGameUI.instance.SetAccuracyBar(per);
    }

    public void CheckAccuracy(Vector3 _previousPos)
    {
        float diff = Mathf.Abs(_previousPos.x-transform.position.x);
        float per = Mathf.Abs((Mathf.Clamp(diff, 0, maxWidth)/maxWidth)-1);
        m_Accuracy = 100f*per;
        // Debug.Log(m_Accuracy);

        SetKinematic();

        Vector3 CamTargetPos = LevelManager.instance.CamTarget.position;
        float Height = transform.position.y>CamTargetPos.y ? transform.position.y : CamTargetPos.y;
        LevelManager.instance.SetCamTarget(Height);

        if (m_Accuracy>=Data.instance.AccuracyRequired)
        {
            Data.instance.CurrentLevel.successfulBlocks++;
        }
        CreateLog();
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        // if (hasLanded || (other.gameObject.tag.Equals("Block") && !hasLanded && !other.collider.GetComponent<BuildingBlock>().hasLanded))
        // {
        //     return;
        // }
        if (hasLanded)
        {
            return;
        }

        if (other.gameObject.tag.Equals("Block"))
        {
            BuildingBlock buildingBlock = other.collider.GetComponent<BuildingBlock>();
            if (buildingBlock != null)
            {
                CheckAccuracy(buildingBlock.transform.position);
                Landed?.Invoke(this);
            }
        }
        else if (other.gameObject.tag.Equals("FirstBlock"))
        {
            CheckAccuracy(other.transform.position);
            Landed?.Invoke(this);
        }
        else
        {
            m_Accuracy = 0f;
            Landed?.Invoke(this);
            CreateLog();
            LeanPool.Despawn(gameObject);
        }
    }

    void OnEnable() 
    {
        Landed += LevelManager.instance.OnLanded;
        FX?.SetActive(false);
    }

    void OnDisable() 
    {
        Landed -= LevelManager.instance.OnLanded;
    }
}
