using UnityEngine;
using System.Collections.Generic;

public class Tutorial : MonoBehaviour
{
    private PlayerController player;

    private CameraSwitcher cameraSwitcher;

    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform aimPosition;
    [SerializeField] private Transform tutStartPosition;
    public List<GameObject> enemySpawned = new List<GameObject>();

    public int currentTut = 0;
    public bool nextTut;

    private bool destoryNextUpdate;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        cameraSwitcher = GameObject.Find("ThirdPersonCamera").GetComponent<CameraSwitcher>();

        spawner.spawnEnemies = true;
        player.lockMovement = true;
        nextTut = true;

        cameraSwitcher.ResetCam();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            nextTut = true;
            currentTut = 5;
        }

        Rigidbody rbplayer = player.GetComponent<Rigidbody>();
        rbplayer.isKinematic = false;

        //if (Input.GetKeyDown(KeyCode.Space))
        //    enemySpawned = true;

        //Wait until enemies spawned
        //if (!enemySpawned)
        //{
        //    if (spawner.currentEnemyCount > 0)
        //        enemySpawned = true;
        //    return;
        //}

        if(currentTut == 4)
        {
            int enemyKilledCount = 0;
            foreach (GameObject enemy in enemySpawned)
            {
                if (enemy.GetComponent<Enemy>() == null)
                {
                    enemyKilledCount++;
                }
            }
            if(enemyKilledCount == 3)
            {
                currentTut = 5;
                nextTut=true;
            }
        }
        if (nextTut) 
        {
            if (currentTut == 0)
            {
                Debug.Log("First message: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //Welcome to Game, Puprose of game,ThrowSpear to kill enemy 
                Debug.Log("Welcome to Game, Puprose of game,ThrowSpear to kill enemy ");

                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = tutStartPosition.position;
                player.transform.rotation = tutStartPosition.rotation;
            }
            else if (currentTut == 1)
            {
                Debug.Log("Running message: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //Aim and ChargeThrow to throw with more strength
                Debug.Log("Aim and ChargeThrow to throw with more strength");

                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = aimPosition.position;
                player.transform.rotation = aimPosition.rotation;
                rb.isKinematic = false;
            }
            else if (currentTut == 2)
            {
                Debug.Log("Running message: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //Heavy enemeis can only be killed by a charged heavy basic spear
                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = tutStartPosition.position;
                player.transform.rotation = tutStartPosition.rotation;

            }
            else if (currentTut == 3) 
            {
                Debug.Log("Running message: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //TranferSpear switch place
            }
            else if(currentTut == 4)
            {
                Debug.Log("Running message: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = tutStartPosition.position;
                player.transform.rotation = tutStartPosition.rotation;
                //Expolsive Spear, larger explosion large you charge, refund spears
            }
            else if(currentTut == 5)
            {
                Debug.Log("Completing tutorial: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                Invoke(nameof(MoveDelay), .5f);
            }
            foreach (GameObject enemy in enemySpawned) 
            {
                if (enemy.GetComponent<Enemy>() == null)
                {
                    enemySpawned.Remove(enemy);
                    Destroy(enemy);
                }
            }
            nextTut = false;
        }
    }

    void MoveDelay()
    {
        //MovePlayer
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        player.transform.position = startPosition.position;
        player.transform.rotation = startPosition.rotation;

        foreach (GameObject enemy in enemySpawned)
        {
            if (enemy.GetComponent<Enemy>() == null)
            {
                enemySpawned.Remove(enemy);
                Destroy(enemy);
            }
        }
        Invoke(nameof(EnableMovement), .1f);
    }
    void EnableMovement()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        player.lockMovement = false;
        player.GetComponent<AmmoManager>().Reload();
        Destroy(gameObject);
    }
}