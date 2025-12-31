using UnityEngine;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    public float moveUpDistance = 100f;
    public float moveTime = 0.8f;
    public float delay = 0.3f;

    private Vector3 startPos;
    private CanvasGroup canvasGroup;

    void Start()
    {
        startPos = transform.localPosition;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        PlayLoop();
    }

    void PlayLoop()
    {
        transform.localPosition = startPos;
        canvasGroup.alpha = 1;

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalMoveY(
            startPos.y + moveUpDistance, moveTime
        ).SetEase(Ease.OutQuad));

        seq.Append(canvasGroup.DOFade(0, 0.15f));
        seq.AppendInterval(delay);

        seq.OnComplete(PlayLoop);
    }
}
