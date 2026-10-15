using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene;
    bool wait;
    private float timer = 0f;

    private void Start()
    {
        Invoke(nameof(WaitF), 0.1f);
    }

    private void Update()
    {
        if (timer < 0.1f)
        {
            timer += Time.deltaTime;
            return;
        }

        //Old input system
        if (Input.anyKeyDown || wait)
        {
            Debug.Log("Loading: " + nextScene);
            SceneManager.LoadScene(nextScene);
        }
    }

    private void WaitF()
    {
        wait = true;
    }
}