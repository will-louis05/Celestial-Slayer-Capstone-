using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene;

    private void Update()
    {
        //Old input system
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Loading: " + nextScene);
            SceneManager.LoadScene(nextScene);
        }
    }
}