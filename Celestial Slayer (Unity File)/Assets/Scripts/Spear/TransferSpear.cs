using Unity.Cinemachine;
using UnityEngine;

public class TransferSpear : Spear
{
    public Transform player;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        Destroy(gameObject);
    }


}
