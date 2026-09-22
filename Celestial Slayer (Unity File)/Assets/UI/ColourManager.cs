using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourManager : MonoBehaviour
{
    public static ColourManager instance;
    private Coroutine coroutine;

    private AmmoManager ammoManager;

    [Header("Colours")]
    [SerializeField] private Color defaultColour = Color.aquamarine;
    [SerializeField] private Color transferColour = Color.purple;
    [SerializeField] private Color explosiveColour = Color.red;
    [SerializeField] private Color textColour = Color.white;

    [Header("References")]
    [SerializeField] private Image crosshair;
    [SerializeField] private List<Image> primaryUI = new List<Image>();
    [SerializeField] private List<TMP_Text> texts = new List<TMP_Text>();
    [SerializeField] private List<Image> missingSpears = new List<Image>();

    [Header("SFX")]
    [SerializeField] private AudioSource missingSFX;

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

        foreach (Image img in missingSpears)
            img.gameObject.SetActive(false);
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

    public void MissingSpears(int index, int amount)
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

        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(Flash(amount));

        if (missingSFX != null)
            missingSFX.Play();
    }

    private IEnumerator Flash(int n)
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
        coroutine = null;
    }
}