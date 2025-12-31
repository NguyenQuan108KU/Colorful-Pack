using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandHint : MonoBehaviour
{
    [Header("Preview")]
    public SpriteRenderer previewRenderer;
    private Sequence seq;

    public void Play(Slot fromSlot, Slot toSlot, DragItem item)
    {
        seq?.Kill();
        SetPreviewSprite(item);

        // parent vào slot nguồn
        transform.SetParent(fromSlot.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        // TÍNH localPosition của slot đích trong hệ fromSlot
        Vector3 toLocal =
            fromSlot.transform.InverseTransformPoint(toSlot.transform.position);

        seq = DOTween.Sequence();

        seq.Append(
            transform.DOLocalMove(toLocal, 1.1f)
                     .SetEase(Ease.InOutQuad)
        );

        seq.AppendCallback(() =>
        {
            gameObject.SetActive(false);
        });

        seq.AppendInterval(0.2f);

        seq.AppendCallback(() =>
        {
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(true);
        });

        seq.AppendInterval(0.15f);
        seq.SetLoops(-1);
    }

    void SetPreviewSprite(DragItem item)
    {
        if (previewRenderer == null) return;

        var itemSR = item.GetComponent<SpriteRenderer>();
        if (itemSR == null) return;

        previewRenderer.sprite = itemSR.sprite;

        Color c = itemSR.color;
        c.a = 0.6f; // 👈 80% opacity
        previewRenderer.color = c;
    }

    public void Stop()
    {
        seq?.Kill();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        seq?.Kill();
    }
}
