using UnityEngine;
using System.Collections.Generic;

public class Spear : MonoBehaviour
{
    [Header("Spear Stats")]
    [SerializeField] private float spearLegnth;
    [SerializeField] private float spearSpeedRatio;
    [SerializeField] private float inverseForceRatio;

    [Header("Angle Checks")]
    [SerializeField] private Transform angleCheck;
    [SerializeField] private float xAllowedAngle;
    [SerializeField] private float yAllowedAngle;


    private Collider spearBodyCollider;
    private Rigidbody spearRb;
    private List<float> spearedObjectsMass = new List<float>();
    private List<GameObject> spearedObjects = new List<GameObject>();
    private bool held = true;
    private bool stuck = false;
    private bool inCollision = false;
    private float preCollisionSpeed;
    private bool enemySpeared;



    private Vector3 aimPoint;

    private void Start()
    {
        spearRb = GetComponent<Rigidbody>();
        spearBodyCollider = GetComponentInChildren<Collider>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if(!inCollision && !stuck)
            preCollisionSpeed = spearRb.linearVelocity.magnitude;
    }

    public void SpearThrown(float throwStrength, float maxThrowStrength)
    {
        transform.SetParent(null);

        Transform camTransform = Camera.main.transform;

        RaycastHit hit;
        if(Physics.Raycast(camTransform.position, camTransform.forward, out hit, 500))
        {
            aimPoint = hit.point;
            transform.LookAt(aimPoint);
        }
        else
        {
            aimPoint = camTransform.forward * 500;
            transform.LookAt(aimPoint);
        }

        Vector3 moveDirection = camTransform.forward * throwStrength * 250f;

        spearBodyCollider.enabled = true;

        spearRb.isKinematic = false;
        spearRb.useGravity = true;
        held = false;

        spearRb.AddForce(moveDirection, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!held && !stuck)
        {
            bool acceptAngle = RayCheck();
            if (acceptAngle)
                SpearHit(collision);
            if (stuck)
                gameObject.layer = 0;
        }
    }

    //bool AngleCheck(Collision collision)
    //{
    //    angleCheck.rotation = Quaternion.Euler(transform.forward);
    //    Vector3 contactPoint = collision.GetContact(0).point;
    //    angleCheck.LookAt(contactPoint);

    //    float xAngle = angleCheck.localEulerAngles.x;
    //    float yAngle = angleCheck.localEulerAngles.y;

    //    bool xCheck = false;
    //    bool yCheck = false;

    //    Debug.Log("X Angle: " + xAngle + " Y Angle: " + yAngle);

    //    if (xAngle > -xAllowedAngle && yAngle < xAllowedAngle)
    //        xCheck = true;

    //    if (yAngle > -yAllowedAngle && yAngle < yAllowedAngle)
    //        yCheck = true;

    //    if (xCheck && yCheck)
    //        return true;
    //    else
    //        return false;
    //}

    bool RayCheck()
    {
        if (Physics.Raycast(angleCheck.position, transform.forward, 0.5f))
            return true;

        return false;
    }

    void SpearHit(Collision collision)
    {
        inCollision = true;
        Transform collisionTraform = collision.transform;
        Enemy enemyScrp = collisionTraform.GetComponent<Enemy>();
        ISpearedObj spearedObj = collisionTraform.GetComponent<ISpearedObj>();
        Rigidbody spearedRb = collisionTraform.GetComponent<Rigidbody>();
        bool pierce = false;

        if (spearedObj != null)
        {
            spearedObj.Speared(out pierce, out stuck);       
        }
        else if (enemyScrp != null)
        {
            enemyScrp.EnemySpeared(out pierce, out stuck, spearRb);
            enemySpeared = true;
        }
        else if (spearedRb != null)
        {
            PierceAmount(collision);
            RbObjSpeared(spearedRb);
        }
        else
        {
            pierce = true;
            stuck = true;
        }


        if (pierce)
        {
            PierceAmount(collision);
        }
        if (stuck)
        {
            Destroy(spearRb);
            stuck = true;
        }




        inCollision = false;
    }

    void PierceAmount(Collision collision)
    {
        Vector3 contactPoint = collision.GetContact(0).point;

        float impaleDistance = preCollisionSpeed * spearSpeedRatio;
        //Debug.Log("Impale Distance " + impaleDistance + " Speed " + preCollisionSpeed + " SpearLength " + spearLegnth);
        if (impaleDistance > spearLegnth)
        {
            //Offset so spear is Always Showing a little
            impaleDistance = spearLegnth * 0.9f;
        }
        //Adjust offset as pivot is in the centre 
        impaleDistance -= 1;
        Vector3 spearMove = contactPoint + (transform.forward * impaleDistance);

        spearRb.isKinematic = true;
        this.transform.position = spearMove;
        spearRb.isKinematic = false;
    }

    void RbObjSpeared(Rigidbody spearedRb)
    {
        //Maybe Add the other speed effecting the spear slowing it more
        //float directionRelative = Vector3.Dot(preCollisionSpeed,spearedRb.linearVelocity);

        float inveseForce = spearedRb.mass * inverseForceRatio;

        float postCollisionSpeed = preCollisionSpeed - inveseForce;

        if (postCollisionSpeed < 0)
        {
            //Stop Spear if inverse speed is too strong
            spearRb.linearVelocity = Vector3.zero;
            stuck = true;
            Destroy(spearRb);
            return;
        }

        //RigidBody must be destory to prevent physics bugs as such save mass to reapply later
        spearedObjects.Add(spearedRb.gameObject);
        spearedRb.transform.SetParent(this.transform);
        spearedObjectsMass.Add(spearedRb.mass);
        Destroy(spearedRb);

        spearRb.linearVelocity = postCollisionSpeed * transform.forward;
    }

    public void SpearDestory()
    {
        for(int i = 0; i < spearedObjects.Count; i++)
        {
            GameObject spearedObject = spearedObjects[i];
            spearedObject.transform.SetParent(null);
            Rigidbody objRb = spearedObject.AddComponent<Rigidbody>();
            objRb.mass = spearedObjectsMass[i];
            objRb.isKinematic = false;
            objRb.useGravity = true;
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(aimPoint, 1);
    }
}
