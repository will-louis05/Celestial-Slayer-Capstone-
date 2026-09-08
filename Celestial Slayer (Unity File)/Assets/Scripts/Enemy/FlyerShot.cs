using UnityEngine;

public class FlyerShot : MonoBehaviour
{
    private Vector3 playerPos;
    private float speed;
    private bool ballSpawned;


    public void ShotSpawn(Vector3 playerPosition, float ballSpeed)
    {
        playerPos = playerPosition;
        speed = ballSpeed;
        transform.LookAt(playerPos);
        ballSpawned = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (ballSpawned)
        {
            transform.position += transform.forward * speed;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
