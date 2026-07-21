using UnityEngine;

public class Bottle : MonoBehaviour, ISpearedObj
{
    [SerializeField] private GameObject smashParticle;
    public void Speared(out bool pierce, out bool stuck)
    {
        pierce = true; stuck = false;
        Instantiate(smashParticle, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
