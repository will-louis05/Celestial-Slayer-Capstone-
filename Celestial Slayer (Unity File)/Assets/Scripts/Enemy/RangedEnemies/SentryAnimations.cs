using UnityEngine;

public class SentryAnimations : MonoBehaviour
{
    [SerializeField] private Transform[] rings;
    [SerializeField] private Transform stars;
    [SerializeField] private float spinSpeed;
    [SerializeField] private float starSpeed;
    public bool spinstar;

    private void RingSpins()
    {
        foreach (var r in rings)
        {
            float zRotation = r.localRotation.z + (spinSpeed *Time.deltaTime);
            float xRotation = r.localRotation.x - (spinSpeed * Time.deltaTime);
            r.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
        }
    }
    private void StarSpin()
    {
        float zRotation = stars.localRotation.z - (starSpeed * Time.deltaTime);
        stars.localRotation = Quaternion.Euler(0, 0f, zRotation);
    }

    private void Update()
    {
        RingSpins();
        if(spinstar)
            StarSpin();
    }
}
