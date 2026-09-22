using UnityEngine;
using System.Collections.Generic;

public class BasicSpear : Spear
{
    [Header("Spear Stats")]
    [SerializeField] private float spearLegnth;
    [SerializeField] private float spearSpeedRatio;
    [SerializeField] private float inverseForceRatio;

    //[Header("Components")]

    private List<float> spearedObjectsMass = new List<float>();
    private List<GameObject> spearedObjects = new List<GameObject>();
    
    private bool stuck = false;
    private bool inCollision = false;
    private int enemiesKilled;
    private List<Enemy> spearedEnemies = new List<Enemy>();
    public float timer = 1f;

    private Vector3 aimPoint;

    [Header("SFX")]
    [SerializeField] private AudioSource hitSFX;
    private bool hitPlayed;

    private void FixedUpdate()
    {
        if (!inCollision && !stuck)
            preCollisionSpeed = spearRb.linearVelocity.magnitude;

        if (!held)
            timer += Time.deltaTime;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform); 
        if (!held && !stuck)
        {
            bool acceptAngle = RayCheck();
            //Spears enemies when spear is already inside them before being thrown
            //if (collision.collider.CompareTag("Enemy"))
            //{
            //    acceptAngle = true;
            //}
            Debug.Log(acceptAngle);
            if (acceptAngle && !spearedObjects.Contains(collision.gameObject))
                SpearHit(collision);
        }
    }

    bool RayCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Debug.Log(hit);
            return true;
        }
        return false;
    }

    void SpearHit(Collision collision)
    {
        inCollision = true;
        Transform collisionTraform = collision.transform;
        Debug.Log(collisionTraform);
            
        ISpearedObj spearedObj = collisionTraform.GetComponent<ISpearedObj>();
        Rigidbody spearedRb = collisionTraform.GetComponent<Rigidbody>();

        if (spearedObj != null)
        {
            bool pierce = false;
            spearedObj.Speared(out pierce, out stuck);

            if (pierce)
                PierceAmount(collision);          
        }
        else if (collisionTraform.CompareTag("Enemy"))
        {
            Transform limbhit = collisionTraform;
            collisionTraform = collisionTraform.root;
           
            Enemy enemyScrp = collisionTraform.GetComponent<Enemy>();
            if (enemyScrp != null)
            {
                //Immediate hit speed bugfix
                if (timer > 0f && preCollisionSpeed < 1f)
                    preCollisionSpeed = throwSpeed;

                PierceAmount(collision);
                bool failedToPierce = enemyScrp.EnemySpeared(spearRb, limbhit, preCollisionSpeed, collision);
                if (failedToPierce)
                {
                    spearRb.isKinematic = true;
                    stuck = true;
                    transform.SetParent(limbhit);
                }
                else
                {
                    spearedEnemies.Add(enemyScrp);
                    AmmoManager ammoScrpt = GameObject.Find("Player").GetComponent<AmmoManager>();
                    if (enemiesKilled == ammoScrpt.killsNeedForRefund)
                        ammoScrpt.RefundSpear();
                }
            }
            else
            {
                PierceAmount(collision);
                stuck = true;
            }
        }
        else if (spearedRb != null)
        {
            PierceAmount(collision);
            RbObjSpeared(spearedRb);
        }
        else
        {
            PierceAmount(collision);
            stuck = true;
        }

        if (stuck)
            SpearStuck(collision);

        inCollision = false;

        //Hit SFX
        if (!hitPlayed)
        {
            hitPlayed = true;
            float soundSpeed = preCollisionSpeed / 100f;
            if (soundSpeed < 0.5f)
                soundSpeed = 0.5f;
            hitSFX.pitch = Random.Range(soundSpeed - 0.1f, soundSpeed + 0.1f);
            hitSFX.Play();
        }
    }

    void PierceAmount(Collision collision)
    {
        collision.collider.enabled = false;
        Vector3 contactPoint = collision.GetContact(0).point;

        float impaleDistance = preCollisionSpeed * spearSpeedRatio;
        if (impaleDistance > spearLegnth)
        {
            //Offset so spear is Always Showing a little
            impaleDistance = spearLegnth * 0.9f;
        }
        else if (impaleDistance < spearLegnth * 0.1f)
        {
            //Offset so spear always pierces a reasonable amount
            impaleDistance = spearLegnth * 0.1f;
        }

        //Adjust offset as pivot is in the centre 
        impaleDistance -= spearLegnth/2;
        Vector3 spearMove = contactPoint + (transform.forward * impaleDistance);

        spearRb.isKinematic = true;
        this.transform.position = spearMove;
        spearRb.isKinematic = false;

        //Bugfix reenabling all colliders after pierce calculated
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            //if (!c.GetComponent<Enemy>())
                c.enabled = true;
        }
        collision.collider.enabled = true;
    }

    void RbObjSpeared(Rigidbody spearedRb)
    {
        //Maybe Add the other speed effecting the spear slowing it more
        //float directionRelative = Vector3.Dot(preCollisionSpeed,spearedRb.linearVelocity);

        float inveseForce = spearedRb.mass * inverseForceRatio;

        //Immediate hit speed bugfix
        if (timer > 0f && preCollisionSpeed < 1f)
            preCollisionSpeed = throwSpeed;

        float postCollisionSpeed = preCollisionSpeed - inveseForce;

        if (postCollisionSpeed < 0)
        {
            //Stop Spear if inverse speed is too strong
            spearRb.linearVelocity = Vector3.zero;
            stuck = true;
            transform.SetParent(spearedRb.transform, true);
            spearBodyCollider.gameObject.layer = 0;

            spearRb.isKinematic = true;
            return;
        }

        //RigidBody must be destoryed to prevent physics bugs as such save mass to reapply later
        spearedObjects.Add(spearedRb.gameObject);
        spearedRb.transform.SetParent(this.transform);
        spearedObjectsMass.Add(spearedRb.mass);

        //Bugfix removing colliders on hit
        Collider collider = spearedRb.GetComponent<Collider>();
        if (collider != null) // && !collider.GetComponent<Enemy>())
            collider.enabled = false;

        Destroy(spearedRb);
        spearRb.linearVelocity = postCollisionSpeed * transform.forward;
    }

    private void SpearStuck(Collision collision)
    {
        //Allow Player To Interact With Spear Again
        spearBodyCollider.gameObject.layer = 0;
        spearRb.isKinematic = true;
        spearRb.useGravity = false;
        stuck = true;
        if(spearedEnemies.Count > 0)
        {
            foreach(Enemy enemy in spearedEnemies)
            {
                enemy.EnemyStuck();
            }
        }
    }

    public void SpearDestroyOld()
    {
        for (int i = 0; i < spearedObjects.Count; i++)
        {
            GameObject spearedObject = spearedObjects[i];
            spearedObject.transform.SetParent(null);
            if (spearedObject.CompareTag("Enemy"))
                continue;

            Rigidbody objRb = spearedObject.AddComponent<Rigidbody>();
            objRb.mass = spearedObjectsMass[i];
            objRb.isKinematic = false;
            objRb.useGravity = true;

            //Bugfix ensure colliders are enabled
            Collider collider = spearedObject.GetComponent<Collider>();
            if (collider != null) // && !collider.GetComponent<Enemy>())
                collider.enabled = true;

            
        }

        Destroy(gameObject);
    }

    public void SpearDestroy()
    {
        Destroy(this);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(aimPoint, 1);
    //}
}
