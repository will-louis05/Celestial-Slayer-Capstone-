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

    [SerializeField] private float delay = 4f;
    [SerializeField] private float slowDuration = 1f;
    [SerializeField] private float quickDuration = 0.3f;
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

        if (tutorialOn)
            RunTutorial();
    }

    private void RunTutorial()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(1f)
            .AppendCallback(() => notificationSFX.Play())
            .Append(panelGroup.DOFade(1f, slowDuration).SetEase(ease))
            .Join(panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration))
            .AppendInterval(delay);

        foreach (string s in messages)
        {
            sequence.Append(textGroup.DOFade(0f, quickDuration).SetEase(ease))
                .AppendCallback(() =>
                {
                    text.text = s;
                    notificationSFX.Play();
                    panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration);
                })
                .Append(textGroup.DOFade(1f, slowDuration).SetEase(ease))
                .AppendInterval(delay);
        }

        sequence.Append(panelGroup.DOFade(0f, quickDuration).SetEase(ease));
    }
}
