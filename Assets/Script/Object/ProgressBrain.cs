using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ProgressBrain : MonoBehaviour
{
    public static ProgressBrain instance;

    public Transform brain;
    public Transform startPoint;
    public Transform endPoint;
    public GameObject endcartWin;

    public int maxScore = 200;
    public int scorePerTray = 25;
    public int currentScore = 0;

    public float moveTime = 0.3f;

    // ===== TIMER =====
    public float idleDelay = 2f;      // 2s không match mới bắt đầu trừ
    public float drainInterval = 1f;  // mỗi 1s trừ 1 điểm
    public int drainAmount = 1;

    private float idleTimer;
    private float drainTimer;

    private Vector3 startPos;
    private Vector3 endPos;
    public int index = 0;
    public TextMeshProUGUI text;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        startPos = startPoint.position;
        endPos = endPoint.position;
    }

    private void Update()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer < idleDelay)
            return;

        drainTimer += Time.deltaTime;

        if (drainTimer >= drainInterval)
        {
            drainTimer = 0f;
            ReduceScore(drainAmount);
        }
    }

    public void AddTrayMatch()
    {
        index++;
        text.text = index.ToString();
        currentScore += scorePerTray;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        if(currentScore >= 200 || index == 10)
        {
            StartCoroutine(ActiveWin());
            Luna.Unity.LifeCycle.GameEnded();
        }
        ResetIdleTimer();
        UpdateBrainPosition();
    }

    void ReduceScore(int amount)
    {
        if (currentScore <= 0) return;

        currentScore -= amount;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);

        UpdateBrainPosition();
    }

    void ResetIdleTimer()
    {
        idleTimer = 0f;
        drainTimer = 0f;
    }

    void UpdateBrainPosition()
    {
        float progress = (float)currentScore / maxScore;

        Vector3 targetPos = Vector3.Lerp(startPos, endPos, progress);

        brain.DOMove(targetPos, moveTime).SetEase(Ease.OutCubic);
    }
    IEnumerator ActiveWin()
    {
        yield return new WaitForSeconds(1f);
        endcartWin.SetActive(true);
    }
}
