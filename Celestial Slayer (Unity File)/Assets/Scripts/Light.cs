using UnityEngine;

public class LightScrp : MonoBehaviour, ISpearedObj
{
    private bool flickering;
    private Light lightComp;
    [SerializeField] private float minFlickerSpeed;
    [SerializeField] private float maxFlickerSpeed;
    private bool lightOn = true;
    void Start()
    {
        lightComp = GetComponentInChildren<Light>();
    }

    public void FuseBoxFlickerSet(float minFuseFlickerSpeed, float maxFuseFlickerSpeed)
    {
        minFlickerSpeed = minFuseFlickerSpeed;
        maxFlickerSpeed = maxFuseFlickerSpeed;
    }

    public void Speared(out bool pierce, out bool stuck)    
    {
        Interaction();
        pierce = true;
        stuck = true;
    }

    public void Interaction()
    {
        if (flickering)
        {
            return;
        }
        flickering = true;

        InvokeRepeating("Flicker", 0f, Random.Range(minFlickerSpeed, maxFlickerSpeed));
    }

    private void Flicker()
    {
        lightComp.enabled = lightOn;
        lightOn = !lightOn;
    }
}
