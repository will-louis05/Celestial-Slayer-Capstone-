using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public bool grounded;
    public static bool isAiming;

    [Header("Components")]
    [SerializeField] private GameObject playerOrientation;
    [SerializeField] private Transform yawTarget;

    [SerializeField] private Transform holdOffset;
    [SerializeField] private Transform crosshair;
    [SerializeField] private ParticleSystem reloadParticle;
    //[SerializeField] private ParticleSystem[] particleSpeedLines;

    private InputHandler inputHandler;
    private Transform cameraTransform;
    private Rigidbody rb;
    private GameObject[] spearCrosshairs;
    private Animator animator;

    [Header("Movement Values")]
    [SerializeField] private float groundMoveSpeed;
    [SerializeField] private float aimedMoveSpeed;
    [SerializeField] private float inAirMoveSpeed;
    [SerializeField] private float maxAirSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpHeight;

    [Header("Spear Types")]
    [SerializeField] private GameObject basicSpear;
    [SerializeField] private GameObject transferSpear;
    [SerializeField] private GameObject explosiveSpear;

    [Header("Spear Values")]
    [SerializeField] private float minThrowStrength;
    [SerializeField] private float maxThrowStrength;
    [SerializeField] private float throwStrengthIncrease;
    [SerializeField] private int totalSpearCount;
    [SerializeField] private float equipTime;
    [SerializeField] private float reloadTimeSec;
    [SerializeField] private float delayBeforeReload;
    private float timer;
    private int currentSpearCount;
    private float throwStrength;
    private enum CurrentSpearType {basicSpear, transferSpear, explosiveSpear}
    CurrentSpearType currentSpearType;

    private GameObject heldSpear;
    private List<GameObject> thrownSpears = new List<GameObject>();
    private bool spearEquiped;
    private bool holdingSpear;
    private bool inEquip;
    private bool inThrow;
    private bool inReload;
    private bool inDelayThrow;

    [Header("SFX")]
    [SerializeField] private AudioSource runSFX;
    [SerializeField] private AudioSource throwSFX;
    [SerializeField] private AudioSource jumpSFX;
    [SerializeField] private AudioSource recallSFX;
    //[SerializeField] private AudioSource landSFX;
    [SerializeField] private AudioSource aimSFX;

    [Header("Debugging Tools")]
    [SerializeField] private bool infiniteSpears;
    [SerializeField] private bool disableMovementInReload;
    public static bool inCombat;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = InputHandler.instance;
        cameraTransform = Camera.main.transform;
        animator = GetComponent<Animator>();

        Transform spearCrossParent = GameObject.Find("SpearCrosshairCount").transform;
        spearCrosshairs = new GameObject[spearCrossParent.childCount];
        for(int i = 0; i < spearCrossParent.childCount; i++)
        {
            spearCrosshairs[i] = spearCrossParent.GetChild(i).gameObject;
        }

        throwStrength = minThrowStrength;
        currentSpearCount = totalSpearCount;
    }

    private void Update()
    {
        Animations();

        InputManger();

        if (infiniteSpears)
            currentSpearCount = 5;



        if ((!holdingSpear && currentSpearCount != 0) || inEquip)
        {
            if (!inDelayThrow && !inEquip)
            {
                inDelayThrow = true;
                timer = Time.time;
            }
            else if (delayBeforeReload < (Time.time - timer))
            {
                inDelayThrow = false;
                SpearEquip();
            }
        }

        //Run SFX
        if (inputHandler.moveInput.magnitude > 0.01f && grounded)
        {
            if (!runSFX.isPlaying)
                runSFX.Play();
        }
        else
        {
            if (runSFX.isPlaying)
                runSFX.Stop();
        }
    }

    private void InputManger()
    {
        if ((inputHandler.equipBasicSpearTriggered && currentSpearType != CurrentSpearType.basicSpear) || (inputHandler.equipTranferSpearTriggered && currentSpearType != CurrentSpearType.transferSpear) || (inputHandler.equipExplosiveSpearTriggered && currentSpearType != CurrentSpearType.explosiveSpear))
        {
            if (inputHandler.equipBasicSpearTriggered)
            {
                currentSpearType = CurrentSpearType.basicSpear;
                inputHandler.equipBasicSpearTriggered = false;
            }
            else if (inputHandler.equipTranferSpearTriggered)
            {               
                currentSpearType = CurrentSpearType.transferSpear;
                inputHandler.equipTranferSpearTriggered = false;
            }
            else if(inputHandler.equipExplosiveSpearTriggered)
            {
                currentSpearType = CurrentSpearType.explosiveSpear;
                inputHandler.equipExplosiveSpearTriggered = false;     
            }
            GameObject spearToDestroy = heldSpear;
            heldSpear = null;
            Destroy(spearToDestroy);
            holdingSpear = false;
            inEquip = false;
        }

        //Aim SFX
        if (inputHandler.fireTriggered && holdingSpear && !inThrow)
            aimSFX.PlayOneShot(aimSFX.clip);

        if (inputHandler.fireTriggered && holdingSpear || inThrow)
            SpearThrow();

        if ((inputHandler.reloadTriggered && currentSpearCount != totalSpearCount && !inEquip) || inReload)
            SpearReload();

    }

    void FixedUpdate()
    {
        Movement();


        if(heldSpear != null)
        {
            //heldSpear.transform.parent = spearFollow; (Jude)
            //holdOffset.LookAt(spearFollow);
        }
    }

    void Movement()
    {
        Move();

        if (grounded && inputHandler.jumpTriggered)
        {
            Jump();
        }   
    }

    void Move()
    {
        //Get postion of camera to dictate where forwards is
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        Vector3 right = cameraTransform.right;
        right.y = 0f;

        Vector3 moveDirection = forward * inputHandler.moveInput.y + right * inputHandler.moveInput.x;
        float moveSpeed = 0;
        float maxSpeed = 0;
        rb.linearDamping = groundDrag;

        if (!grounded)
        {
            moveSpeed = inAirMoveSpeed;
            maxSpeed = maxAirSpeed;
            rb.linearDamping = 0;
        }
        else if (isAiming)
        {
            moveSpeed = aimedMoveSpeed;
            maxSpeed = moveSpeed;
            runSFX.pitch = 0.8f;
        }
        else
        {
            moveSpeed = groundMoveSpeed;
            maxSpeed = moveSpeed;
            runSFX.pitch = 1f;
        }

        if (inReload && disableMovementInReload)
        {
            moveSpeed = 0;
        }
        rb.AddForce(moveDirection.normalized * moveSpeed * 300f, ForceMode.Force);

        //Speed Control, stop player from endless acceleration
        Vector3 faltVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (faltVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedVelocity = faltVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        //Pretty sure these do the same thing, should remove one
        if(!isAiming)
        { 
            //Turns player towards camera
            Vector3 camAngle = Camera.main.transform.rotation.eulerAngles;
            Vector3 playerAngle = playerOrientation.transform.rotation.eulerAngles;
        
            //90 degree cone
            //float angleDif = Mathf.Clamp(Mathf.DeltaAngle(camAngle.y, playerAngle.y), -60f, 30f);
            Quaternion targetRotation = Quaternion.Euler(playerAngle.x, camAngle.y, playerAngle.z);
            playerOrientation.transform.rotation = Quaternion.Slerp(playerOrientation.transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
        else
        {        
            Vector3 lookDirection = yawTarget.forward;
            lookDirection.y = 0f;

            if (lookDirection.magnitude > 0)
            {
                Quaternion targetRotaiton = Quaternion.LookRotation(lookDirection);
                playerOrientation.transform.rotation = Quaternion.Slerp(playerOrientation.transform.rotation, targetRotaiton, 10f * Time.deltaTime);
            }
        }
    }

    void Jump()
    {
        rb.AddForce(jumpHeight * Vector3.up * 30, ForceMode.Impulse);
        inputHandler.jumpTriggered = false;

        animator.SetTrigger("Jump");

        //Jump SFX
        jumpSFX.pitch = Random.Range(0.9f, 1.1f);
        jumpSFX.Play();
    }

    void SpearEquip()
    {
       
        if (!inEquip)
        {
            timer = Time.time;
            animator.SetTrigger("InEquip");
            inEquip = true;

            GameObject spearToSpawn = null;
            switch (currentSpearType)
            {
                case CurrentSpearType.basicSpear:
                    spearToSpawn = basicSpear;
                    break;
                case CurrentSpearType.transferSpear:
                    spearToSpawn = transferSpear;
                    break;
                case CurrentSpearType.explosiveSpear:
                    spearToSpawn = explosiveSpear;
                    break;
            }
            //heldSpear = Instantiate(spearToSpawn, spearFollow); (Jude)
            heldSpear = Instantiate(spearToSpawn, holdOffset);
            heldSpear.transform.localRotation = Quaternion.Euler(new Vector3(90,0,0));  
            Animator spearAni = heldSpear.GetComponent<Animator>();
            var aniLength = spearAni.GetCurrentAnimatorStateInfo(0).length;
            aniLength = aniLength/ equipTime;
            spearAni.speed = aniLength;

            //var equpLength = spearAni.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("UpperBody")).length; 
            //equpLength = equpLength/ equipTime;
            //animator.speed = equpLength;

        }

        var elapsedTime = Time.time - timer;
        if (elapsedTime > equipTime)
        {
            Animator spearAni = heldSpear.GetComponent<Animator>();
            Destroy(spearAni);
            //animator.speed = 1;
            holdingSpear = true;
            inEquip = false;
        }
        
    }

    void SpearThrow()
    {
        //Build throw strength when aimed
        if (isAiming)
        {
            inThrow = true;
            animator.SetBool("InCharge", true);
            if (throwStrength < maxThrowStrength)
            {

                throwStrength += throwStrengthIncrease * Time.deltaTime;
                float throwPercentage = 1 - (throwStrength / (maxThrowStrength * 2));
                crosshair.localScale = new Vector3(throwPercentage, throwPercentage, throwPercentage);
            }
            else
            {
                throwStrength = maxThrowStrength;
            }
        }

        //ThrowSpear
        if (!inThrow || inThrow && !inputHandler.fireTriggered)
        {
            if (aimSFX.isPlaying)
                aimSFX.Stop();

            Spear spearScrp = heldSpear.GetComponent<Spear>();
            spearScrp.SpearThrown(throwStrength, maxThrowStrength);
            currentSpearCount -= 1;
            thrownSpears.Add(heldSpear);

            animator.SetTrigger("Throw");
            animator.SetBool("InCharge", false);

            spearCrosshairs[currentSpearCount].SetActive(false);

            //Throw SFX
            throwSFX.pitch = Random.Range(0.9f, 1.1f);
            throwSFX.Play();
            
            //particleSpeedLines[0].Play();
            //if (throwStrength > maxThrowStrength * 0.75f)
            //{
            //    particleSpeedLines[1].Play();
            //}
            //if (throwStrength == maxThrowStrength)
            //{
            //    particleSpeedLines[2].Play();
            //}

            throwStrength = minThrowStrength;
            crosshair.localScale = new Vector3(1,1,1);

            inThrow = false;
            heldSpear = null;
            holdingSpear = false;
            inputHandler.fireTriggered = false;
        }


        //Cancel Throw if player stops aiming while in throw
        if (inThrow && !isAiming)
        {
            if (aimSFX.isPlaying)
                aimSFX.Stop();

            inputHandler.fireTriggered = false;
            inThrow = false;
            crosshair.localScale = new Vector3(1, 1, 1);
            animator.SetBool("InCharge", false);
        }
    }

    void SpearReload() 
    {
        if (!inReload)
        {
            timer = Time.time;

            reloadParticle.Play();

            //Recall SFX
            recallSFX.Play();

            foreach (GameObject thrownSpear in thrownSpears)
            {
                if (thrownSpear != null)
                {
                    ParticleSystem spearReloadParticle = thrownSpear.GetComponentInChildren<ParticleSystem>();
                    spearReloadParticle.Play();
                }
            }
        }


        if (!inputHandler.reloadTriggered)
        {
            inReload = false;
            reloadParticle.Stop();
            animator.SetBool("InSummon", inReload);
            //Stop recall SFX
            recallSFX.Stop();

            foreach (GameObject thrownSpear in thrownSpears)
            {
                if (thrownSpear != null)
                {
                    ParticleSystem spearReloadParticle = thrownSpear.GetComponentInChildren<ParticleSystem>();
                    spearReloadParticle.Stop();
                }
            }
            return;
        }

        inReload = true;
        float elaspedTime = Time.time - timer;

        if (elaspedTime >= reloadTimeSec)
        {
            foreach(GameObject thrownSpear in thrownSpears)
            {
                if (thrownSpear != null)
                {
                    BasicSpear spearScr = thrownSpear.GetComponent<BasicSpear>();
                    spearScr.SpearDestroy();
                }
            }
            reloadParticle.Stop();

            foreach (GameObject spearCrosshair in spearCrosshairs)
            {
                spearCrosshair.SetActive(true);
            }
       
            thrownSpears.Clear();
            currentSpearCount = totalSpearCount;

            inputHandler.reloadTriggered = false;
            inReload = false;
        }
        animator.SetBool("InSummon", inReload);
    }

    private void Animations()
    {
        //(Jude) changed the velocity to represent a percentage of the max speed so there is a little bit on blending, but I cant find exactly what sets the mex speed. but I know the number (14)

        animator.SetFloat("VelocityX", (inputHandler.moveInput.x) * (rb.linearVelocity.magnitude / 14.0f));
        animator.SetFloat("VelocityY", (inputHandler.moveInput.y) * (rb.linearVelocity.magnitude / 14.0f));
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("InAim", isAiming);
        animator.SetBool("InAir", !grounded);
        animator.SetBool("Equipped", holdingSpear);
    }
}
