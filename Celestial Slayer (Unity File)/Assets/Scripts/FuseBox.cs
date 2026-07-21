using UnityEngine;
using System.Collections.Generic;


public class FuseBox : MonoBehaviour, ISpearedObj
{
    [SerializeField] private GameObject[] lights;
    private List<LightScrp> lightScrps = new List<LightScrp>();
    [SerializeField] private float minFlickerSpeed;
    [SerializeField] private float maxFlickerSpeed;
    [SerializeField] private GameObject fuseSpark;
    private bool damaged;

    void Start()
    {
        foreach(GameObject light in lights)
        {
            LightScrp lightScrp = light.GetComponent<LightScrp>();
            lightScrp.FuseBoxFlickerSet(minFlickerSpeed, maxFlickerSpeed);
            lightScrps.Add(lightScrp);
        }
    }


    public void Speared(out bool pierce, out bool stuck)
    {
        pierce = true;
        stuck = true;

        if (damaged)
            return;
        damaged = true;
        fuseSpark.SetActive(true);

        foreach (LightScrp light in lightScrps)
        {
            light.Interaction();
        }
    }
}
