using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [SerializeField] private GameObject settingsScreen;
    private bool active = false;

    [SerializeField] private Slider mouseSens;
    [SerializeField] private Slider aimMouseSens;
    [SerializeField] private Slider controllerSens;
    [SerializeField] private Slider aimControllerSens;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mouseSens.value = InputHandler.instance.mouseSens;
        mouseSens.onValueChanged.AddListener(delegate { InputHandler.instance.mouseSens = mouseSens.value; });

        aimMouseSens.value = InputHandler.instance.aimedMouseSens;
        aimMouseSens.onValueChanged.AddListener(delegate { InputHandler.instance.aimedMouseSens = aimMouseSens.value; });

        controllerSens.value = InputHandler.instance.controllerSens;
        controllerSens.onValueChanged.AddListener(delegate { InputHandler.instance.controllerSens = controllerSens.value; });

        aimControllerSens.value = InputHandler.instance.aimedControllerSens;
        aimControllerSens.onValueChanged.AddListener(delegate { InputHandler.instance.aimedControllerSens = aimControllerSens.value; });
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