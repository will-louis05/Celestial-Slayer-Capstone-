using UnityEngine;

public class GlobalKillCounter : MonoBehaviour
{
    public static GlobalKillCounter instance { get; private set; }

    public int globalKillNumber = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //Reset on scene reset (old input system)
        if (Input.GetKeyDown(KeyCode.I))
            globalKillNumber = 0;
        if (Input.GetKeyDown(KeyCode.P))
            globalKillNumber = 0;
    }
}