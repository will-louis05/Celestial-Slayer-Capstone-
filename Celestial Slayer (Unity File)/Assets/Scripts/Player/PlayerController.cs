using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool grounded;
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

    [Header("Movement Values")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float aimedMoveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float playerSlow;
    [SerializeField] private float airSpeedClamp;
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

    [Header("Debugging Tools")]
    [SerializeField] private bool infiniteSpears;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = InputHandler.instance;
        cameraTransform = Camera.main.transform;

        Transform spearCrossParent = GameObject.Find("SpearCrosshairCount").transform;
        spearCrosshairs = new GameObject[spearCrossParent.childCount];
        for(int i = 0; i < spearCrossParent.childCount; i++)
        {
            spearCrosshairs[i] = spearCrossParent.GetChild(i).gameObject;
        }

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Spear"));

        throwStrength = minThrowStrength;
        currentSpearCount = totalSpearCount;
    }

    private void Update()
    {
        //Check if grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f);
        InputManger();

        //For Debug Purposes
        //if(Input.GetKeyDown(KeyCode.F)) Debug.Log()

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
        if (isAiming)
        {
            AimedMove();
        }
        else
        {
            RegularMove();
        }

        if (grounded && inputHandler.jumpTriggered)
        {
            Jump();
        }   
    }

    void RegularMove()
    {
        //Get postion of camera to dictate where forwards is
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;


        Vector3 right = cameraTransform.right;
        right.y = 0f;



        Vector3 moveDirection = forward * inputHandler.moveInput.y + right * inputHandler.moveInput.x;

        //Stop player from speeding up in air
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 300f, ForceMode.Force);
            rb.linearDamping = groundDrag;

            //Speed Control, stop player from endless acceleration
            Vector3 faltVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (faltVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = faltVelocity.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }

            //Turns player model to face where they are walking
            //if (moveDirection.magnitude > 0)
            //{
            //    Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            //    playerOrientation.transform.rotation = (Quaternion.Slerp(playerOrientation.transform.rotation, toRotation, 10f * Time.deltaTime));
            //}
        }
        else
        {
            rb.linearDamping = 0;
            rb.AddForce(moveDirection.normalized * (moveSpeed / 4) * airSpeedClamp, ForceMode.Force);
        }

        //Turns player towards camera
        Vector3 camAngle = Camera.main.transform.rotation.eulerAngles;
        Vector3 playerAngle = playerOrientation.transform.rotation.eulerAngles;
        
        //90 degree cone
        //float angleDif = Mathf.Clamp(Mathf.DeltaAngle(camAngle.y, playerAngle.y), -60f, 30f);

        Quaternion targetRotation = Quaternion.Euler(playerAngle.x, camAngle.y, playerAngle.z);
        playerOrientation.transform.rotation = Quaternion.Slerp(playerOrientation.transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    private void AimedMove()
    {

        Vector3 forward = playerOrientation.transform.forward;
        forward.y = 0f;

        Vector3 right = playerOrientation.transform.right;
        right.y = 0f;

        Vector3 moveDirection = forward * inputHandler.moveInput.y + right * inputHandler.moveInput.x;

        //Stop player from speeding up in air
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * aimedMoveSpeed * 300f, ForceMode.Force);
            rb.linearDamping = groundDrag;

            //Speed Control, stop player from endless acceleration
            Vector3 faltVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (faltVelocity.magnitude > aimedMoveSpeed)
            {
                Vector3 limitedVelocity = faltVelocity.normalized * aimedMoveSpeed;
                rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
            }
        }
        else
        {
            rb.linearDamping = 0;
            rb.AddForce(moveDirection.normalized * (moveSpeed / 4) * airSpeedClamp, ForceMode.Force);
        }


        //Turns player model to face where camera is looking
        Vector3 lookDirection = yawTarget.forward;
        lookDirection.y = 0f;

        if (lookDirection.magnitude > 0)
        {
            Quaternion targetRotaiton = Quaternion.LookRotation(lookDirection);
            playerOrientation.transform.rotation = Quaternion.Slerp(playerOrientation.transform.rotation, targetRotaiton, 10f * Time.deltaTime);
        }
    }

    void Jump()
    {
        rb.AddForce(jumpHeight * Vector3.up * 30, ForceMode.Impulse);
        inputHandler.jumpTriggered = false;
    }

    void SpearEquip()
    {
       
        if (!inEquip)
        {
            reloadTimeTracker = Time.time;
            inEquip = true;
        }     
        var elapsedTime = Time.time - reloadTimeTracker;

        Debug.Log("spearequiping = " + inEquip + "time = " + elapsedTime);
        if (elapsedTime > equipTime)
        {
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

        if (isAiming)
        {
            inThrow = true;
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

        if (!inThrow || inThrow && !inputHandler.fireTriggered)
        {
            BasicSpear spearScrp = heldSpear.GetComponent<BasicSpear>();
            spearScrp.SpearThrown(throwStrength, maxThrowStrength);
            currentSpearCount -= 1;
            thrownSpears.Add(heldSpear);


            spearCrosshairs[currentSpearCount].SetActive(false);
            
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
        }
    }

    void SpearReload() 
    {
        if (!inReload)
        {
            reloadTimeTracker = Time.time;

            reloadParticle.Play();
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
            foreach (GameObject thrownSpear in thrownSpears)
            {
                ParticleSystem spearReloadParticle = thrownSpear.GetComponentInChildren<ParticleSystem>();
                spearReloadParticle.Stop();
            }
            return;
        }

        inReload = true;
        float elaspedTime = Time.time - reloadTimeTracker;

        if(elaspedTime >= reloadTimeSec)
        {
            foreach(GameObject thrownSpear in thrownSpears)
            {
                BasicSpear spearScr = thrownSpear.GetComponent<BasicSpear>();
                spearScr.SpearDestory();
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
    }
}
