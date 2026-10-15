using UnityEngine;
using System.Collections.Generic;

public class LightScrp : MonoBehaviour, ISpearedObj
{
    private bool flickering;
    private List<Light> lightComp = new List<Light>();
    [SerializeField] private float minFlickerSpeed = 0.1f;
    [SerializeField] private float maxFlickerSpeed = 1;
    private bool lightOn = true;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Light light = transform.GetChild(i).GetComponent<Light>();
            if (light != null)
                lightComp.Add(light);
        }
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
        foreach(Light light in lightComp)
        {
            light.enabled = lightOn;
            lightOn = !lightOn;
        }
    }
}
