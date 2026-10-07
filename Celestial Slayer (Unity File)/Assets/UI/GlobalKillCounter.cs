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
}