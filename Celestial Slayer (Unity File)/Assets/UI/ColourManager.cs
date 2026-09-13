using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ColourManager : MonoBehaviour
{
    public static ColourManager instance;

    [Header("UI")]
    [SerializeField] private Color primaryColour = Color.black;
    [SerializeField] private Color secondaryColour = Color.white;
    [SerializeField] private Color backgroundColour = Color.clear;
    [SerializeField] private List<Image> primaryUI = new List<Image>();
    [SerializeField] private List<Image> secondaryUI = new List<Image>();
    [SerializeField] private List<Image> backgroundUI = new List<Image>();
    [SerializeField] private List<TMP_Text> texts = new List<TMP_Text>();

    [Header("Crosshair")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Color basicColour;
    [SerializeField] private Color transferColour;
    [SerializeField] private Color explosiveColour;

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
}