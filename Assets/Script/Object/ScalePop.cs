using UnityEngine;
using DG.Tweening;

public class ScalePopLoop : MonoBehaviour
{
    public Vector3 minScale = Vector3.one * 0.9f;
    public Vector3 maxScale = Vector3.one * 1.1f;
    public float duration = 0.4f;
    public Ease ease = Ease.InOutQuad;

    Tween _tween;

    void OnEnable()
    {
        Play();
    }

    void Play()
    {
        _tween?.Kill();

        transform.localScale = minScale;

        _tween = transform
            .DOScale(maxScale, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo); // 🔁 lặp vô hạn
    }

    void OnDisable()
    {
        _tween?.Kill();
    }
}
