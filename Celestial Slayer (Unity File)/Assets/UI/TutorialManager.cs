using UnityEngine;
using DG.Tweening;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private bool tutorialOn = true;
    [SerializeField] string[] messages;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private CanvasGroup panelGroup;

    [SerializeField] private float delay = 3f;
    [SerializeField] private Ease ease = Ease.OutQuint;

    [SerializeField] private AudioSource notificationSFX;

    //private RectTransform rectTrans;
    //private Vector3 startPos;

    private void Start()
    {
        //rectTrans = gradientGroup.GetComponent<RectTransform>();
        //startPos = rectTrans.position;

        panelGroup.alpha = 0;
        textGroup.alpha = 1;

        text.text = messages[0];

        if (tutorialOn)
            RunTutorial();
    }

    private void RunTutorial()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay)
            .Append(panelGroup.DOFade(1f, delay).SetEase(ease));

        foreach (string s in messages)
        {
            sequence.AppendCallback(() => text.text = s)
                .AppendCallback(() => notificationSFX.Play())
                .Append(textGroup.DOFade(1f, delay).SetEase(ease))
                .AppendInterval(delay)
                .Append(textGroup.DOFade(0f, delay).SetEase(ease));
        }

        sequence.Append(panelGroup.DOFade(0f, delay).SetEase(ease));
    }
}
