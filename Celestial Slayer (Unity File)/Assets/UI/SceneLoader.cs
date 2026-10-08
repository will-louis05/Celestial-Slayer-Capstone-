using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene;

    private float timer = 0f;

    private void Update()
    {
        if (timer < 0.1f)
        {
            timer += Time.deltaTime;
            return;
        }

        //Old input system
        if (Input.anyKeyDown)
        {
            Debug.Log("Loading: " + nextScene);
            SceneManager.LoadScene(nextScene);
        }
    }
}