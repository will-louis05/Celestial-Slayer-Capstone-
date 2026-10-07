using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableWinScreen : MonoBehaviour, ICombatEvent
{
    public void PostCombatEvent()
    {
        string scene = "WinScreen";
        Debug.Log("Loading: " + scene);
        SceneManager.LoadScene(scene);
    }

    public void TriggerEvent()
    {

    }
}