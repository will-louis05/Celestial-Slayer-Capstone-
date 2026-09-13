using Unity.Cinemachine;
using UnityEngine;

public class TransferSpear : Spear
{
    private CinemachineOrbitalFollow thirdPersonCam;
    private AimCameraController aimCam;

    private Transform player;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        thirdPersonCam = GameObject.Find("ThirdPersonCamera").GetComponent<CinemachineOrbitalFollow>();
        aimCam = GameObject.Find("AimCamera").GetComponent<AimCameraController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
            Transfer(collision);
        else
            Destroy(gameObject);
    }

    private void Transfer(Collision collision)
    {
        Transform enemyHit = collision.transform.root;
        Vector3 enemyHitLocation = enemyHit.position;
        enemyHit.position = player.position;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        playerRb.isKinematic = true;
        player.position = enemyHitLocation;
        Transform playerOrientation = player.Find("Orientation");
        playerOrientation.LookAt(enemyHit);
        playerRb.isKinematic = false;

        PlayerController playerController = playerRb.GetComponent<PlayerController>();
        playerController.disableTurn = true;
        LookAtEnemy(enemyHit);

        Destroy(gameObject);
    }

    private void LookAtEnemy(Transform enemy)
    {
        Vector3 lookDirection = enemy.position - player.position;
        lookDirection.y = 0f;
        if (lookDirection.sqrMagnitude <= 0)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        player.rotation = lookRotation;
        Transform playerOrientation = player.Find("Orientation");
        Transform yawTarget = player.Find("YawTarget");
        playerOrientation.rotation = lookRotation;
        yawTarget.rotation = lookRotation;

        //Reset cameras
        if (PlayerController.isAiming)
        {
            aimCam.SetCameForward(player);
        }
        else
        {
            thirdPersonCam.HorizontalAxis.Value = lookRotation.eulerAngles.y;
            thirdPersonCam.VerticalAxis.Value = 20f;
        }
    }
}
