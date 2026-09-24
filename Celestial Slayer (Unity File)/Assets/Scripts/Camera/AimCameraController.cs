using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] private Transform yawTarget;
    [SerializeField] private Transform pitchTarget;

    [SerializeField] private float sensitivity;

    [SerializeField] private float pitchMin = -40f;
    [SerializeField] private float pitchMax = 55f;

    [SerializeField] private float shoulderSwitchSpeed = 5f;

    [SerializeField] private InputActionReference lookInput;

    private PlayerController player;
    private InputHandler inputHandler;
    private CinemachineCamera cam;
    private CinemachineThirdPersonFollow aimCam;

    private float yaw;
    private float pitch;
    private float targetCameraSide;

    private float fov;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        aimCam = GetComponent<CinemachineThirdPersonFollow>();
        targetCameraSide = aimCam.CameraSide;
    }

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        fov = cam.Lens.FieldOfView;

        Vector3 angles = yawTarget.rotation.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        inputHandler = InputHandler.instance;
        lookInput.asset.Enable();
    }

    private void Update()
    {
        //Vector2 look = lookInput.action.ReadValue<Vector2>() * sensitivity;
        Vector2 look = inputHandler.lookInput * sensitivity;

        yaw += look.x;
        pitch -= look.y;

        //Pitch clamp
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        aimCam.CameraSide = Mathf.Lerp(aimCam.CameraSide, targetCameraSide, Time.deltaTime * shoulderSwitchSpeed);

        if (inputHandler.shoulderSwitchTriggered)
        {
            targetCameraSide = aimCam.CameraSide < 0.5f ? 1 : 0f;
            inputHandler.shoulderSwitchTriggered = false;
        }

        //Aim cam zoom (needs to be linked to aimtime on playercontroller)
        if (player.inThrow)
            cam.Lens.FieldOfView = Mathf.MoveTowards(cam.Lens.FieldOfView, 25f, Time.deltaTime * 2f);
        else
            cam.Lens.FieldOfView = Mathf.MoveTowards(cam.Lens.FieldOfView, fov, Time.deltaTime * 60f);
    }

    public void SetCameForward(Transform camTranform)
    {
        Vector3 flatForward = camTranform.forward;
        flatForward.y = 0f;

        if (flatForward.magnitude < 0f)
            return;

        yaw = Quaternion.LookRotation(flatForward).eulerAngles.y;

        //Get cam pitch
        float camPitch = camTranform.eulerAngles.x;
        if (camPitch > 180f)
            camPitch -= 360f;
        pitch = Mathf.Clamp(camPitch, pitchMin, pitchMax);

        yawTarget.rotation = Quaternion.Euler(0f, yaw, 0f);
        pitchTarget.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }
}