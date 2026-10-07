using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private PlayerController player;

    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform aimPosition;
    [SerializeField] private Transform tutStartPosition;

    private int currentTut = 0;
    private bool enemySpawned = false;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        spawner.spawnEnemies = true;
        player.lockMovement = true;
    }

    void Update()
    {
        if (currentTut == 0)
        {
            Debug.Log("First message: " + currentTut);
            TutorialManager.instance.TutorialMessage(currentTut);

            //Welcome to Game, Puprose of game,ThrowSpear to kill enemy 
            Debug.Log("Welcome to Game, Puprose of game,ThrowSpear to kill enemy ");
            currentTut++;

            //MovePlayer
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            player.transform.position = tutStartPosition.position;
            player.transform.rotation = tutStartPosition.rotation;
            rb.isKinematic = false;
            player.lockMovement = false;
            enemySpawned = false;

            return;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //    enemySpawned = true;

        //Wait until enemies spawned
        if (!enemySpawned)
        {
            if (spawner.currentEnemyCount > 0)
                enemySpawned = true;
            return;
        }

        if (spawner.currentEnemyCount == 0) 
        {
            if (currentTut == 1)
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
                Debug.Log("Heavy enemeis can only be killed by a charged heavy basic spear");
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

                //Expolsive Spear, larger explosion large you charge, refund spears
            }
            else if(currentTut == 5)
            {
                Debug.Log("Completing tutorial: " + currentTut);
                TutorialManager.instance.TutorialMessage(currentTut);

                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = startPosition.position;
                player.transform.rotation = startPosition.rotation;
                rb.isKinematic = false;

                Destroy(gameObject);
            }

            currentTut++;
            enemySpawned = false;
        }
    }
}