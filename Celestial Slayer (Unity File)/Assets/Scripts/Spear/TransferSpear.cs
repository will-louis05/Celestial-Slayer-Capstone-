using UnityEngine;

public class TransferSpear : Spear
{
    public Transform player;

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
        playerRb.isKinematic = false;

        Camera camera = Camera.main;


        Destroy(gameObject);
    }


}
