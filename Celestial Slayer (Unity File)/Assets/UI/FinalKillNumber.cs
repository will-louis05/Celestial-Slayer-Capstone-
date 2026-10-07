using TMPro;
using UnityEngine;

public class FinalKillNumber : MonoBehaviour
{
    private TMP_Text killNumber;

    private void Awake()
    {
        killNumber = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (GlobalKillCounter.instance != null)
        {
            killNumber.text = "TOTAL KILLS | " + GlobalKillCounter.instance.globalKillNumber;
            GlobalKillCounter.instance.globalKillNumber = 0;
        }
        else
        {
            killNumber.text = "TOTAL KILLS | 0";
        }
    }
}
