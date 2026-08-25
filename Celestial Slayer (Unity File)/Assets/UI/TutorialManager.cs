using UnityEngine;
using DG.Tweening;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool tutorialOn = true;
    [SerializeField] string[] messages;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private CanvasGroup gradientGroup;
    [SerializeField] private Ease ease = Ease.OutQuint;

    //private RectTransform rectTrans;
    //private Vector3 startPos;

    private void Start()
    {
        //rectTrans = gradientGroup.GetComponent<RectTransform>();
        //startPos = rectTrans.position;

        gradientGroup.alpha = 0;
        textGroup.alpha = 0;

        if (tutorialOn)
            Sequence();
    }

    private void Sequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(3f);

        foreach (string s in messages)
        {
            sequence.AppendCallback(() => text.text = s)
                .Append(gradientGroup.DOFade(1f, 3f).SetEase(ease))
                .Join(textGroup.DOFade(1f, 3f).SetEase(ease))
                .AppendInterval(3f)
                .Append(gradientGroup.DOFade(0f, 3f).SetEase(ease))
                .Join(textGroup.DOFade(0f, 3f).SetEase(ease));
        }
    }
}
