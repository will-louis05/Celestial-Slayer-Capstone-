using UnityEngine;

public class DoorOpen : MonoBehaviour, ICombatEvent
{
    [SerializeField] private GameObject door;
    public void PostCombatEvent()
    {
        Destroy(door);
    }
}
