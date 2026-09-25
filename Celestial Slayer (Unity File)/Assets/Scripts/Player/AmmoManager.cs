using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    private GameObject[] spearCrosshairs;
    public Image[] spearCrosshairImages;
    public int killsNeedForRefund;
    [SerializeField] private int refundAmount = 1;

    public int totalSpearCount = 5;
    public int currentSpearCount { get; private set;}

    [Header("Star Colours")]
    [SerializeField] private Color basicSpearColour = Color.aquamarine;
    [SerializeField] private Color transferSpearColour = Color.purple;
    [SerializeField] private Color explosiveSpearColour = Color.red;

    [Header("Debugging Tools")]
    [SerializeField] private bool infiniteSpears;

    void Start()
    {
        //Spear UI icons
        Transform spearCrossParent = GameObject.Find("SpearCrosshairCount").transform;
        spearCrosshairs = new GameObject[spearCrossParent.childCount];
        spearCrosshairImages = new Image[spearCrossParent.childCount];
        for (int i = 0; i < spearCrossParent.childCount; i++)
        {
            spearCrosshairs[i] = spearCrossParent.GetChild(i).gameObject;
            spearCrosshairImages[i] = spearCrosshairs[i].GetComponent<Image>();
            spearCrosshairImages[i].fillAmount = 1f;
        }
        currentSpearCount = totalSpearCount;
        ColourManager.instance.SetCrosshair(0);
    }

    private void Update()
    {
        if (infiniteSpears)
            currentSpearCount = 5;

        //if (Input.GetKeyDown(KeyCode.Space))
        //    RefundSpear();
    }

    public void DecreaseSpearCount(int spearsToRemove)
    {
        //Unfill UI spear
        for (int i = 0; i < spearsToRemove; i++)
        {
            currentSpearCount -= 1;
            spearCrosshairImages[currentSpearCount].fillAmount = 0f;
            spearCrosshairImages[currentSpearCount].color = Color.white;
        }
    }

    public void CancelReloadUI()
    {
        for (int i = currentSpearCount; i < totalSpearCount; i++)
            spearCrosshairImages[i].fillAmount = 0f;
    }

    public void Reload()
    {
        for (int i = 0; i < spearCrosshairImages.Length; i++)
            spearCrosshairImages[i].fillAmount = 1f;
        currentSpearCount = totalSpearCount;
    }

    public void StarReloadUI(float elaspedTime, float reloadTimeSec)
    {
        float progress = Mathf.Clamp01(elaspedTime / reloadTimeSec);
        for (int i = currentSpearCount; i < totalSpearCount; i++)
            spearCrosshairImages[i].fillAmount = progress;
    }

    public void ShowSpearCost(int spearType, int spearsToRemove)
    {
        Color starColour = Color.white;
        for (int i = 0; i < currentSpearCount; i++)
        {
            spearCrosshairImages[i].color = starColour;
        }

        switch (spearType)
        {
            case 0:
                starColour = basicSpearColour;
                break;
            case 1:
                starColour = transferSpearColour;
                break;
            case 2:
                starColour = explosiveSpearColour;
                break;
        }

        int currentSpearCountUI = currentSpearCount;
        for (int i = 0; i < spearsToRemove; i++)
        {
            currentSpearCountUI -= 1;
            spearCrosshairImages[currentSpearCountUI].color = starColour;
        }
    }

    public void RefundSpear()
    {
        currentSpearCount += refundAmount;
        spearCrosshairImages[currentSpearCount - 1].fillAmount = 1f;

        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        ShowSpearCost((int)player.currentSpearType, player.spearsToRemove);
    }
}
