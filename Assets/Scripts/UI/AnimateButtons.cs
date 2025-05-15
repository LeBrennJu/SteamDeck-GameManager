using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AnimateButtons : MonoBehaviour
{
    public Button[] buttons;
    public float delayBetween = 0.1f;
    public float animationDuration = 0.5f;
    public float startYOffset = 200f;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rect = buttons[i].GetComponent<RectTransform>();
            Vector3 originalPos = rect.anchoredPosition;
            rect.anchoredPosition += Vector2.up * startYOffset;
            rect.gameObject.SetActive(false);

            // Delay par bouton
            float delay = i * delayBetween;

            // Active le bouton + anime vers sa position d’origine
            rect.gameObject.SetActive(true);
            rect.DOAnchorPos(originalPos, animationDuration)
                .SetDelay(delay)
                .SetEase(Ease.OutBack);
        }
    }
}
