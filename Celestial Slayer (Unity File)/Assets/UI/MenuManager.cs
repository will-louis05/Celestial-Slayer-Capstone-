using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private GameObject settingsScreen;
    private bool active = false;

    [SerializeField] private Slider lookSens;
    [SerializeField] private Slider aimLookSens;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        lookSens.value = InputHandler.instance.lookSens;
        lookSens.onValueChanged.AddListener(delegate { InputHandler.instance.lookSens = lookSens.value; });

        aimLookSens.value = InputHandler.instance.aimLookSens;
        aimLookSens.onValueChanged.AddListener(delegate { InputHandler.instance.aimLookSens = aimLookSens.value; });
    }

    public void Settings()
    {
        if (active)
        {
            //Hide menu
            settingsScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            active = false;
        }
        else
        {
            //Show menu
            settingsScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            active = true;
        }
    }
}