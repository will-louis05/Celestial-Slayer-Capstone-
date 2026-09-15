using UnityEngine;

public class Spear : MonoBehaviour
{
    protected Collider spearBodyCollider;
    protected Rigidbody spearRb;
    protected bool held = true;
    protected float throwSpeed;
    protected float preCollisionSpeed;
    private GameObject vfxSpawned;
    [SerializeField] protected GameObject throwParticle;

    protected virtual void Start()
    {
        spearRb = GetComponent<Rigidbody>();
        spearBodyCollider = GetComponentInChildren<Collider>();
    }

    public void SpearThrown(float throwStrength, float maxThrowStrength)
    {
        transform.SetParent(null);
        Transform camTransform = Camera.main.transform;
        Transform spearfollow = camTransform.GetChild(0);
        //transform.rotation = spearfollow.rotation;
        //Debug.Log("postActio = " + transform.rotation + " spearFollowTrans = " + spearfollow.rotation);
        transform.LookAt(spearfollow.position);

        Vector3 moveDirection = transform.forward * throwStrength * 250f;
        throwSpeed = throwStrength * 12f;

        spearBodyCollider.enabled = true;

        //spearRb.interpolation = RigidbodyInterpolation.Interpolate;
        spearRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        spearRb.isKinematic = false;
        spearRb.useGravity = true;
        held = false;

        vfxSpawned = Instantiate(throwParticle, (transform.forward * 2f + transform.position), transform.rotation);
        Invoke(nameof(DestoryVFX), 2f);

        spearRb.AddForce(moveDirection, ForceMode.Impulse);
        preCollisionSpeed = moveDirection.magnitude;
    }

    private void DestoryVFX()
    {
        Destroy(vfxSpawned);
    }
}
