using UnityEngine;



public class Tutorial : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    private PlayerController player;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform aimPosition;
    [SerializeField] private Transform tutStartPosition;
    private int currentTut;
    private bool enemySpawned;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        spawner.spawnEnemies = true;
        player.lockMovment = true;
    }
    void Update()
    {
        if (spawner.currentEnemyCount == 0 && currentTut != 0) 
        {
            if (currentTut == 1)
            {
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
                //Heavy enemeis can only be killed by a charged heavy basic spear
                Debug.Log("Heavy enemeis can only be killed by a charged heavy basic spear");

            }
            else if (currentTut == 3) 
            {
                //TranferSpear switch place
            }
            else if(currentTut == 4)
            {
                //Expolsive Spear, larger explosion large you charge, refund spears
            }
            else if(currentTut == 5)
            {
                //MovePlayer
                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                player.transform.position = startPosition.position;
                player.transform.rotation = startPosition.rotation;
                rb.isKinematic = false;

                Destroy(gameObject);
            }
            currentTut++;
        }
        if (currentTut == 0)
        {
            //Welcome to Game, Puprose of game,ThrowSpear to kill enemy 
            Debug.Log("Welcome to Game, Puprose of game,ThrowSpear to kill enemy ");
            currentTut++;
            //MovePlayer
            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            player.transform.position = tutStartPosition.position;
            player.transform.rotation = tutStartPosition.rotation;
            rb.isKinematic = false;
            player.lockMovment = false;
        }

    }
}
