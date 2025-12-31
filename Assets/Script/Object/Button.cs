using UnityEngine;
using DG.Tweening;

public class Button : MonoBehaviour
{
    Tween scaleTween;

    void OnEnable()
    {
        // button active sẵn nhưng nhỏ
        transform.localScale = Vector3.zero;
    }

    void OnDisable()
    {
        transform.DOKill();
    }

    void OnDestroy()
    {
        transform.DOKill();
    }

    // ===== HÀM PHÓNG TO =====
    public void ScaleUp()
    {
        transform.DOKill();

        scaleTween = transform
            .DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject); // 🔑 auto kill khi destroy
    }
}
