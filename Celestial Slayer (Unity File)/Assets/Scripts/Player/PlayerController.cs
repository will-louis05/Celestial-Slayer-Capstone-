using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool grounded;
    public static bool isAiming;

    [Header("Components")]
    [SerializeField] private GameObject playerOrientation;
    [SerializeField] private Transform yawTarget;
    [SerializeField] private GameObject spear;
    [SerializeField] private Transform holdOffset;
    [SerializeField] private Transform spearFollow;
    [SerializeField] private Transform crosshair;
    [SerializeField] private ParticleSystem reloadParticle;
    [SerializeField] private ParticleSystem[] particleSpeedLines;

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

    [Header("Spear Values")]
    [SerializeField] private float minThrowStrength;
    [SerializeField] private float maxThrowStrength;
    [SerializeField] private float throwStrengthIncrease;
    [SerializeField] private int totalSpearCount;
    [SerializeField] private float equipTime;
    [SerializeField] private float reloadTimeSec;
    private float reloadTimeTracker;
    private int currentSpearCount;
    private float throwStrength;
    private enum CurrentSpearType {basicSpear, transferSpear}
    CurrentSpearType currentSpearType;

    private GameObject heldSpear;
    private List<GameObject> thrownSpears = new List<GameObject>();
    private bool spearEquiped;
    private bool holdingSpear;
    private bool inEquip;
    private bool inThrow;
    private bool inReload;
    private float throwTimer;

    [Header("SFX")]
    [SerializeField] private AudioSource runSFX;
    [SerializeField] private AudioSource throwSFX;
    [SerializeField] private AudioSource jumpSFX;
    [SerializeField] private AudioSource recallSFX;
    //[SerializeField] private AudioSource landSFX;

    [Header("Debugging Tools")]
    [SerializeField] private bool infiniteSpears;
    [SerializeField] private bool disableMovementInReload;
    [SerializeField] private GameObject debugLight;
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

        ////WONT WORK WITH NEW GROUND CHECK
        //bool wasGrounded = grounded;

        //OLD GROUND CHECK
        //grounded = Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f);

        //Land SFX
        //if (!wasGrounded && grounded && Time.timeSinceLevelLoad > 1f)
        //{
        //    landSFX.pitch = Random.Range(0.7f, 0.9f);
        //    landSFX.Play();
        //}

        InputManger();

        if (infiniteSpears)
            currentSpearCount = 5;

        if ((spearEquiped && !holdingSpear && currentSpearCount != 0) || inEquip)
        {
            SpearEquip();
        }
        else if (!spearEquiped && holdingSpear)
        {
            SpearUnequip();
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
        if (inputHandler.equipBasicSpearTriggered)
        {
            spearEquiped = !spearEquiped;
            inputHandler.equipBasicSpearTriggered = false;
        }

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
            holdOffset.LookAt(spearFollow);
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
            animator.SetBool("InEquip", true);
            debugLight.SetActive(true);
            reloadTimeTracker = Time.time;
            inEquip = true;
        }     

        var elapsedTime = Time.time - reloadTimeTracker;
        if (elapsedTime > equipTime)
        {
            debugLight.SetActive(false);
            animator.SetBool("InEquip", false);
            heldSpear = Instantiate(spear, holdOffset);
            heldSpear.transform.SetParent(holdOffset);
            holdingSpear = true;
            inEquip = false;
        }
        
    }

    void SpearUnequip()
    { 
        GameObject spearToDestory = heldSpear;
        Destroy(spearToDestory);
        heldSpear = null;
        holdingSpear = false;
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
            BasicSpear spearScrp = heldSpear.GetComponent<BasicSpear>();
            spearScrp.SpearThrown(throwStrength, maxThrowStrength);
            currentSpearCount -= 1;
            thrownSpears.Add(heldSpear);

            animator.SetTrigger("Throw");
            animator.SetBool("InCharge", false);

            spearCrosshairs[currentSpearCount].SetActive(false);

            //Throw SFX
            throwSFX.pitch = Random.Range(0.9f, 1.1f);
            throwSFX.Play();
            
            particleSpeedLines[0].Play();
            if (throwStrength > maxThrowStrength * 0.75f)
            {
                particleSpeedLines[1].Play();
            }
            if (throwStrength == maxThrowStrength)
            {
                particleSpeedLines[2].Play();
            }

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
            reloadTimeTracker = Time.time;

            reloadParticle.Play();

            //Recall SFX
            recallSFX.Play();

            foreach (GameObject thrownSpear in thrownSpears)
            {
                ParticleSystem spearReloadParticle = thrownSpear.GetComponentInChildren<ParticleSystem>();
                spearReloadParticle.Play();
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
                ParticleSystem spearReloadParticle = thrownSpear.GetComponentInChildren<ParticleSystem>();
                spearReloadParticle.Stop();
            }
            return;
        }

        inReload = true;
        float elaspedTime = Time.time - reloadTimeTracker;

        if (elaspedTime >= reloadTimeSec)
        {
            foreach(GameObject thrownSpear in thrownSpears)
            {
                BasicSpear spearScr = thrownSpear.GetComponent<BasicSpear>();
                spearScr.SpearDestroy();
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
        animator.SetFloat("VelocityX", inputHandler.moveInput.x);
        animator.SetFloat("VelocityY", inputHandler.moveInput.y);
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("InAim", isAiming);
        animator.SetBool("InAir", !grounded);
    }
}
