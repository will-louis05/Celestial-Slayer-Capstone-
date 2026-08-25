using UnityEngine;
using UnityEngine.Rendering;

public class EnableObjectTrigger : MonoBehaviour
{
    [SerializeField] private GameObject objectToEnable;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToEnable.SetActive(true);
            Destroy(gameObject);
        }
    }
}
