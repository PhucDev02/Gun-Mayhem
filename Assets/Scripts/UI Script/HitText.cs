using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class HitText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
   [SerializeField] Color color;
    private void OnValidate()
    {
        color = text.color;
        text = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        text.color = color;
        transform.localScale = Vector3.one * 1.5f;
        transform.DOScale(1,0.25f).SetEase(Ease.OutCirc).onComplete+=Disable;
    }

    private void Disable()
    {
        text.DOFade(0, 0.25f).SetDelay(0.25f).OnComplete(() =>
        {
            ObjectPool.Instance.Recall(gameObject);
        });
    }
}
