using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [SerializeField] private float fireRate;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private GameObject shot;
    private Transform player;
    private float timeSinceLastFired;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }


    private void Update()
    {
        Vector3 directionFromPlayer = player.position - transform.position;
        if (distanceFromPlayer < directionFromPlayer.magnitude)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionFromPlayer, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Fire();
                }
            }

                if (timeSinceLastFired < fireRate)
                {

                }
        }
    }

    private void Fire()
    {
        Instantiate(shot, (transform.forward + 2 * transform.position), Quaternion.identity);
    }
}
