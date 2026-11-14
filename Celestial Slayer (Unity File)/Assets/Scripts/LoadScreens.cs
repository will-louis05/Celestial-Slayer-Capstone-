using TMPro;
using UnityEngine;

public class LoadScreens : MonoBehaviour
{
    [SerializeField] private GameObject[] loadScreens;
    [SerializeField] private TextMeshProUGUI loadingText;
    private int loadScreenint;
    bool inLoad;
    int loadCounter;
    [SerializeField] private float loadTime;

    public void ShowLoadScreen()
    {
        inLoad = true;
        loadScreenint = Random.Range(0, loadScreens.Length-1);
        loadScreens[loadScreenint].SetActive(true);
        LoadBuffer();
        Invoke(nameof(HideLoadScreen), loadTime);
    }

    public void HideLoadScreen()
    {
        inLoad = false;
        loadScreens[loadScreenint].SetActive(false);
        loadingText.text = "";
    }

    private void LoadBuffer()
    {
        if (inLoad) 
        {
            if (loadCounter == 0)
            {
                loadingText.text = "loading";

            }
            else if (loadCounter == 1)
            {
                loadingText.text = "loading.";

            }
            else if (loadCounter == 2)
            {
                loadingText.text = "loading..";

            }
            else if (loadCounter == 3)
            {
                loadingText.text = "loading...";

            }
            loadCounter++;
            if (loadCounter > 3)
                loadCounter = 0;
            Invoke(nameof(LoadBuffer), 0.4f);
        }

    }
}
