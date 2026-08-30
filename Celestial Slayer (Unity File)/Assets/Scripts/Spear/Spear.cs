using UnityEngine;

public class Spear : MonoBehaviour
{
    protected Collider spearBodyCollider;
    protected Rigidbody spearRb;
    protected bool held = true;
    protected float preCollisionSpeed;


    public void SpearThrown(float throwStrength, float maxThrowStrength)
    {
        transform.SetParent(null);

        Transform camTransform = Camera.main.transform;

        Vector3 moveDirection = camTransform.forward * throwStrength * 250f;

        spearBodyCollider.enabled = true;

        spearRb.isKinematic = false;
        spearRb.useGravity = true;
        held = false;

        spearRb.AddForce(moveDirection, ForceMode.Impulse);
        preCollisionSpeed = moveDirection.magnitude;
    }
}
