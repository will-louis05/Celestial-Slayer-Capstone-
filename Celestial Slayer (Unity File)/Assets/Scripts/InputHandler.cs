using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string aim = "Aim";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string shoulderSwitch = "Shoulder Switch";
    [SerializeField] private string equipSpear = "Equip Spear";
    [SerializeField] private string equipTranferSpear = "Equip Transfer Spear";
    [SerializeField] private string reload = "Reload";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction aimAction;
    private InputAction fireAction;
    private InputAction shoulderSwitchAction;
    private InputAction equipBasicSpearAction;
    private InputAction equipTranferSpearAction;
    private InputAction reloadAction;

    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool aimTriggered { get; private set; }

    [Header("Triggers")]

    public bool jumpTriggered;
    public bool shoulderSwitchTriggered;
    public bool equipBasicSpearTriggered;
    public bool equipTranferTriggered;
    public bool fireTriggered;
    public bool reloadTriggered;


    public static InputHandler instance { get; private set; }

    private void Awake()
    {
        //Hide and Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Check if InputHandler exists and ensures there can't be two 
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }

        //Find All Actions
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        aimAction = playerControls.FindActionMap(actionMapName).FindAction(aim);
        fireAction = playerControls.FindActionMap(actionMapName).FindAction(fire);
        shoulderSwitchAction = playerControls.FindActionMap(actionMapName).FindAction(shoulderSwitch);
        equipBasicSpearAction = playerControls.FindActionMap(actionMapName).FindAction(equipSpear);
        equipTranferSpearAction = playerControls.FindActionMap(actionMapName).FindAction(equipTranferSpear);
        reloadAction = playerControls.FindActionMap(actionMapName).FindAction(reload);




        RegisterInputAction();
    }

    void RegisterInputAction()
    {
        //Checks if button has been pressed and updates the value
        moveAction.performed += context => moveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => moveInput = Vector2.zero;


        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;

        jumpAction.performed += context => jumpTriggered = true;
        jumpAction.canceled += context => jumpTriggered = false;

        aimAction.performed += context => aimTriggered = true;
        aimAction.canceled += context => aimTriggered = false;

        fireAction.performed += context => fireTriggered = true;
        fireAction.canceled += context => fireTriggered = false;

        shoulderSwitchAction.performed += context => shoulderSwitchTriggered = true;
        shoulderSwitchAction.canceled += context => shoulderSwitchTriggered = false;

        equipBasicSpearAction.performed += context => equipBasicSpearTriggered = true;
        equipBasicSpearAction.canceled += context => equipBasicSpearTriggered = false;

        equipTranferSpearAction.performed += context => equipTranferTriggered = true;
        equipTranferSpearAction.canceled += context => equipTranferTriggered = false;

        reloadAction.performed += context => reloadTriggered = true;
        reloadAction.canceled += context => reloadTriggered = false;
    }



    private void OnEnable()
    {
        //As new Input uses event system they need to be enabled and disabled
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        aimAction.Enable();
        fireAction.Enable();
        shoulderSwitchAction.Enable();
        equipBasicSpearAction.Enable();
        equipTranferSpearAction.Enable();
        reloadAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable(); 
        jumpAction.Disable();
        aimAction.Disable();
        fireAction.Disable();
        shoulderSwitchAction.Disable();
        equipBasicSpearAction.Disable(); 
        equipTranferSpearAction.Disable();
        reloadAction.Disable();
    }
}
