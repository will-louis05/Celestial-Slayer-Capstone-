using UnityEngine;

public class SpearHold : MonoBehaviour
{
    [SerializeField] private Transform weaponHold;
    

    void Update()
    {
        transform.position = weaponHold.position;
        transform.rotation = weaponHold.rotation;
    }
}
