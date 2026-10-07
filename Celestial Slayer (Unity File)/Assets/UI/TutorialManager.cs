using UnityEngine;
using DG.Tweening;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Settings")]
    [SerializeField] [TextArea(1, 10)] private string[] messages;

    [Header("Animation")]
    [SerializeField] private float delay = 5f;
    [SerializeField] private float slowDuration = 1f;
    [SerializeField] private float quickDuration = 0.3f;
    [SerializeField] private Ease ease = Ease.OutQuint;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private AudioSource notificationSFX;

    private Sequence sequence;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        panelGroup.alpha = 0;
        textGroup.alpha = 1;
    }

    public void TutorialMessage(int n)
    {
        if (sequence != null && sequence.IsActive())
            sequence.Kill();

        sequence = DOTween.Sequence();

        if (n == 0)
        {
            //First message
            sequence.AppendInterval(1f)
                .AppendCallback(() =>
                {
                    text.text = messages[n];
                    notificationSFX.Play();
                    panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration);
                })
                .Append(panelGroup.DOFade(1f, slowDuration).SetEase(ease));
        }
        else if (n == 5)
        {
            //Final message (5 is currently end of tutorial)
            sequence.Append(panelGroup.DOFade(0f, quickDuration).SetEase(ease));
        }
        else
        {
            panelGroup.alpha = 1f;

            //Tutorial message
            sequence.Append(textGroup.DOFade(0f, quickDuration).SetEase(ease))
                .AppendCallback(() =>
                {
                    text.text = messages[n];
                    notificationSFX.Play();
                    panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration);
                })
                .Append(textGroup.DOFade(1f, slowDuration).SetEase(ease));
        }
    }

    public void RunFullTutorial()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f)
            .AppendCallback(() => notificationSFX.Play())
            .Append(panelGroup.DOFade(1f, slowDuration).SetEase(ease))
            .Join(panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration))
            .AppendInterval(delay);

        foreach (string s in messages)
        {
            seq.Append(textGroup.DOFade(0f, quickDuration).SetEase(ease))
                .AppendCallback(() =>
                {
                    text.text = s;
                    notificationSFX.Play();
                    panelGroup.transform.DOPunchScale(Vector3.one * 0.05f, quickDuration);
                })
                .Append(textGroup.DOFade(1f, slowDuration).SetEase(ease))
                .AppendInterval(delay);
        }

        seq.Append(panelGroup.DOFade(0f, quickDuration).SetEase(ease));
    }
}
