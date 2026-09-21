using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class ColourManager : MonoBehaviour
{
    public static ColourManager instance;
    private Coroutine coroutine;

    [Header("UI")]
    [SerializeField] private Color primaryColour = Color.black;
    [SerializeField] private Color secondaryColour = Color.white;
    [SerializeField] private Color backgroundColour = Color.clear;
    [SerializeField] private List<Image> primaryUI = new List<Image>();
    [SerializeField] private List<Image> secondaryUI = new List<Image>();
    [SerializeField] private List<Image> backgroundUI = new List<Image>();
    [SerializeField] private List<TMP_Text> texts = new List<TMP_Text>();
    [SerializeField] private GameObject filter;

    [Header("Crosshair")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Color basicColour = Color.aquamarine;
    [SerializeField] private Color transferColour = Color.purple;
    [SerializeField] private Color explosiveColour = Color.red;

    [Header("SFX")]
    [SerializeField] private AudioSource errorSFX;

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
        ApplyTint();
    }

    public void ApplyTint()
    {
        foreach (Image img in primaryUI)
            img.color = primaryColour;

        foreach (Image img in secondaryUI)
            img.color = secondaryColour;

        foreach (Image img in backgroundUI)
            img.color = backgroundColour;

        foreach (TMP_Text text in texts)
        {
            text.color = secondaryColour;

            Material mat = text.fontSharedMaterial;
            if (mat != null)
            {
                mat.EnableKeyword(ShaderUtilities.Keyword_Glow);
                mat.SetColor(ShaderUtilities.ID_GlowColor, secondaryColour);
            }
        }

        filter.gameObject.SetActive(false);
    }

    public void SetCrosshair(int index)
    {
        switch (index)
        {
            case 0:
                crosshair.color = basicColour;
                break;
            case 1:
                crosshair.color = transferColour;
                break;
            case 2:
                crosshair.color = explosiveColour;
                break;
        }
    }

    public void MissingSpearsFlash()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        if (errorSFX != null)
        {
            errorSFX.pitch = Random.Range(0.9f, 1.1f);
            errorSFX.Play();
        }

        for (int i = 0; i < 3; i++)
        {
            filter.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            filter.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        coroutine = null;
    }
}