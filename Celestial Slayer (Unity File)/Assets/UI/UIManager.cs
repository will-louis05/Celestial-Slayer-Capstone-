using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private Coroutine damageCoroutine;
    private Coroutine refundCoroutine;
    private Coroutine missingCoroutine;
    private Coroutine comboCoroutine;

    private Sequence sequence;

    private AmmoManager ammoManager;

    [Header("Colours")]
    [SerializeField] private Color defaultColour = Color.aquamarine; //16CBD4
    [SerializeField] private Color transferColour = Color.purple; //977AE0
    [SerializeField] private Color explosiveColour = Color.red; //CC0000
    [SerializeField] private Color textColour = Color.white;

    [Header("References")]
    [SerializeField] private Image crosshair;
    [SerializeField] private List<Image> primaryUI = new List<Image>();
    [SerializeField] private List<TMP_Text> texts = new List<TMP_Text>();
    [SerializeField] private List<Image> missingSpears = new List<Image>();
    [SerializeField] private GameObject refundUI;
    [SerializeField] private GameObject damageVignette;
    [SerializeField] private RectTransform killCounter;
    [SerializeField] private TMP_Text killComboNum;
    [SerializeField] private TMP_Text killTotalNum;

    private int comboKills = 0;
    private int globalKills = 0;

    [Header("Animation")]
    [SerializeField] private float comboTime = 3f;
    [SerializeField] private Ease ease = Ease.OutQuint;

    private Vector2 counterOnScreenPos;
    private Vector2 counterOffScreenPos;
    private bool counterShowing = false;

    [Header("SFX")]
    [SerializeField] private AudioSource missingSFX;
    [SerializeField] private AudioSource refundSFX;

    private void OnValidate()
    {
        ApplyTint();
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ammoManager = GameObject.Find("Player").GetComponent<AmmoManager>();

        ApplyTint();

        refundUI.SetActive(false);
        damageVignette.SetActive(false);

        foreach (Image img in missingSpears)
            img.gameObject.SetActive(false);

        killTotalNum.text = globalKills.ToString();

        Canvas.ForceUpdateCanvases();
        counterOnScreenPos = killCounter.anchoredPosition;
        counterOffScreenPos = counterOnScreenPos - new Vector2(200f, 0f);
        killCounter.anchoredPosition = counterOffScreenPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            AddKills(1);
    }

    public void ApplyTint()
    {
        foreach (Image img in primaryUI)
            img.color = defaultColour;

        foreach (TMP_Text text in texts)
        {
            text.color = textColour;
            Material mat = text.fontSharedMaterial;
            if (mat != null)
            {
                mat.EnableKeyword(ShaderUtilities.Keyword_Glow);
                mat.SetColor(ShaderUtilities.ID_GlowColor, textColour);
            }
        }
    }

    public void SetCrosshair(int index)
    {
        switch (index)
        {
            case 0:
                crosshair.color = defaultColour;
                break;
            case 1:
                crosshair.color = transferColour;
                break;
            case 2:
                crosshair.color = explosiveColour;
                break;
        }
    }

    public void AddKills(int amount)
    {
        comboKills += amount;
        if (comboKills < 100)
            killComboNum.text = comboKills.ToString();
        else
            killComboNum.text = "99";

        if (sequence.IsActive())
            sequence.Kill();

        if (!counterShowing)
        {
            counterShowing = true;
            killCounter.DOKill();
            killCounter.DOAnchorPos(counterOnScreenPos, 0.5f).SetEase(ease);
        }

        if (comboCoroutine != null)
            StopCoroutine(comboCoroutine);

        comboCoroutine = StartCoroutine(ComboRoutine());
    }

    private IEnumerator ComboRoutine()
    {
        yield return new WaitForSeconds(comboTime);

        ConfirmKills(comboKills);
        comboKills = 0;
        killComboNum.text = "0";

        comboCoroutine = null;
    }

    private void ConfirmKills(int amount)
    {
        globalKills += amount;

        if (globalKills < 100)
            killTotalNum.text = globalKills.ToString();
        else
            killTotalNum.text = "+";

        sequence = DOTween.Sequence();
        sequence.Append(killTotalNum.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f))
            .AppendInterval(1f)
            .Append(killCounter.DOAnchorPos(counterOffScreenPos, 0.2f).SetEase(Ease.Linear))
            .OnComplete(() => counterShowing = false);
    }

    public void DamageUI()
    {
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(FlashDamageRoutine());
    }

    public void RefundUI()
    {
        if (refundCoroutine != null)
            StopCoroutine(refundCoroutine);

        refundCoroutine = StartCoroutine(FlashRefundRoutine());

        if (refundSFX != null)
            refundSFX.Play();
    }

    public void MissingSpearsUI(int index, int amount)
    {
        Color color = defaultColour;
        switch (index)
        {
            case 1:
                color = transferColour;
                break;
            case 2:
                color = explosiveColour;
                break;
        }
        color.a = 0.5f;

        foreach (Image img in missingSpears)
        {
            img.color = color;
            img.gameObject.SetActive(false);
        }

        if (missingCoroutine != null)
            StopCoroutine(missingCoroutine);
        missingCoroutine = StartCoroutine(FlashSpearsRoutine(amount));

        if (missingSFX != null)
            missingSFX.Play();
    }

    private IEnumerator FlashRefundRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            refundUI.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            refundUI.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
        refundCoroutine = null;
    }

    private IEnumerator FlashSpearsRoutine(int n)
    {
        List<Image> images = new List<Image>();
        for (int i = ammoManager.currentSpearCount; i < ammoManager.currentSpearCount + n; i++)
            images.Add(missingSpears[i]);

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (Image img in images)
                img.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            foreach (Image img in images)
                img.gameObject.SetActive(false);
        }
        missingCoroutine = null;
    }

    private IEnumerator FlashDamageRoutine()
    {
        damageVignette.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        damageVignette.SetActive(false);
        damageCoroutine = null;
    }
}