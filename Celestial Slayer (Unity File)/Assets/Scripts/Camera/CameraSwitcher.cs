using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera freeLookCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private CinemachineInputAxisController inputAxiscontroller;
    //[SerializeField] private PlayerController player;
    //[SerializeField] private GameObject crosshiarUI;

    private bool isAiming = false;
    private Transform yawTarget;
    private Transform pitchTarget;

    private AimCameraController aimCamController;
    private InputHandler inputHandler;
    private Camera mainCamera;

    void Start()
    {
        aimCamController = aimCam.GetComponent<AimCameraController>();
        inputAxiscontroller = freeLookCam.GetComponent<CinemachineInputAxisController>();
        inputHandler = InputHandler.instance;
    }

    void Update()
    {
        if (inputHandler.aimTriggered && !isAiming)
        {
            EnterAim();
        }
        else if (!inputHandler.aimTriggered && isAiming)
        {
            ExitAim();
        }
    }

    void EnterAim()
    {
        isAiming = true;
        PlayerController.isAiming = true;

        SnapAimCamForward();

        aimCam.Priority = 1;
        freeLookCam.Priority = 0;

        inputAxiscontroller.enabled = false;

    }

    private void SnapAimCamForward()
    {
        aimCamController.SetCameForward(freeLookCam.transform); 
    }

    private void ExitAim()
    {
        isAiming = false;
        PlayerController.isAiming = false;

        SnapFreeCamBehind();

        aimCam.Priority = 0;
        freeLookCam.Priority = 1;

        inputAxiscontroller.enabled = true;
    }

    private void SnapFreeCamBehind()
    {
        CinemachineOrbitalFollow orbitalFollow = freeLookCam.GetComponent<CinemachineOrbitalFollow>();
        Vector3 forward = aimCam.transform.forward;
        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        orbitalFollow.HorizontalAxis.Value = angle;
    }
}
