using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("NotGround"), LayerMask.NameToLayer("GroundCheck"));
        playerController = transform.parent.GetComponent<PlayerController>();
    }
    private void OnTriggerStay(Collider other)
    {
        playerController.grounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        playerController.grounded = false;
    }
}

