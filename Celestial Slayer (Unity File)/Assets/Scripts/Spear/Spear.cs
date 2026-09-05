using UnityEngine;
using UnityEngine.EventSystems;

public class Spear : MonoBehaviour
{
    protected Collider spearBodyCollider;
    protected Rigidbody spearRb;
    protected bool held = true;
    protected float preCollisionSpeed;

    protected virtual void Start()
    {
        spearRb = GetComponent<Rigidbody>();
        spearBodyCollider = GetComponentInChildren<Collider>();
    }

    public void SpearThrown(float throwStrength, float maxThrowStrength)
    {
        transform.SetParent(null);

        Transform camTransform = Camera.main.transform;
        Vector3 spearfollow = camTransform.GetChild(0).position;
        transform.position = new Vector3(transform.position.x, spearfollow.y, transform.position.z);
        transform.LookAt(spearfollow);

        Vector3 moveDirection = transform.forward * throwStrength * 250f;

        spearBodyCollider.enabled = true;

        spearRb.interpolation = RigidbodyInterpolation.Interpolate;
        spearRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        spearRb.isKinematic = false;
        spearRb.useGravity = true;
        held = false;

        spearRb.AddForce(moveDirection, ForceMode.Impulse);
        preCollisionSpeed = moveDirection.magnitude;
    }
}
