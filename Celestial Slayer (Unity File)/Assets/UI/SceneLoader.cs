using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextScene;

    private void Update()
    {
        //Old input system
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Debug.Log("Loading: " + nextScene);
            SceneManager.LoadScene("MainLevel");
        }
    }
}