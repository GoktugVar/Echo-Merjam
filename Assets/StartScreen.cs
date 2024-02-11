using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image backgorund;

    void Start()
    {
        text.DOFade(1, 1).OnComplete(() =>
        {
            StartCoroutine(FadeTextAndBackground());
        });
    }

    IEnumerator FadeTextAndBackground()
    {
        yield return new WaitForSeconds(2f);
        text.DOFade(0, 1);
        yield return new WaitForSeconds(1f);
        backgorund.DOFade(0, 1)
            .OnComplete(() => gameObject.SetActive(false));
    }

}
