using DG.Tweening;
using System.Collections;
using UnityEngine;

public class EndCart_Win : MonoBehaviour
{
    public GameObject confettiLeft;
    public GameObject confettiRight;
    public GameObject winText;
    public GameObject chest;
    public GameObject button;
    

    public float textDelay = 0.5f;
    public float chestDelay = 0.5f;

    void OnEnable()
    {
        confettiLeft.SetActive(false);
        confettiRight.SetActive(false);
        winText.SetActive(false);
        chest.SetActive(false);

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        confettiLeft.SetActive(true);
        confettiRight.SetActive(true);

        yield return new WaitForSeconds(textDelay);

        winText.SetActive(true);

        yield return new WaitForSeconds(chestDelay);

        chest.SetActive(true);
        yield return new WaitForSeconds(chestDelay);
        button.SetActive(true);
        button.transform.localScale = Vector3.zero;

        button.transform
            .DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutQuad); // mượt, không bật
    }
}
