using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private AudioSource landSFX;
    [SerializeField] private float requiredAirTime = 1f;
    private float airTimer;

    private void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();

        airTimer = requiredAirTime;
    }

    private void Update()
    {
        if (!playerController.grounded)
            airTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger != true)
        {
            if (airTimer <= 0f) // && Time.timeSinceLevelLoad > 0.01f)
            {
                landSFX.pitch = Random.Range(0.7f, 0.9f);
                landSFX.Play();
            }

            airTimer = requiredAirTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.isTrigger != true)
            playerController.grounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger != true)
        {
            playerController.grounded = false;

            airTimer = requiredAirTime;
        }
    }
}

