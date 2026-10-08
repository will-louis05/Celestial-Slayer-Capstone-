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

    public virtual void SpearThrown(float throwStrength, float maxThrowStrength)
    {
        transform.SetParent(null);
        Transform camTransform = Camera.main.transform;
        Transform spearfollow = camTransform.GetChild(0);
        Transform spearTela = camTransform.GetChild(1);
        //transform.rotation = spearfollow.rotation;
        //Debug.Log("postActio = " + transform.rotation + " spearFollowTrans = " + spearfollow.rotation);
        
        transform.LookAt(spearfollow.position);

        vfxSpawned = Instantiate(throwParticle, (transform.forward * 2f + transform.position), transform.rotation);
        Invoke(nameof(DestoryVFX), 2f);

        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;

        Vector3 moveDirection = transform.forward * throwStrength * 250f;
        throwSpeed = throwStrength * 18f; //Assuming base speed of ~90 (5 -> 90)

        spearBodyCollider.enabled = true;

        transform.position = spearTela.position;
        //transform.LookAt(spearfollow.position);

        //spearRb.interpolation = RigidbodyInterpolation.Interpolate;
        spearRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        spearRb.isKinematic = false;
        spearRb.useGravity = true;
        held = false;

        spearRb.AddForce(moveDirection, ForceMode.Impulse);
        preCollisionSpeed = moveDirection.magnitude;
    }

    private void DestoryVFX()
    {
        Destroy(vfxSpawned);
    }
}
