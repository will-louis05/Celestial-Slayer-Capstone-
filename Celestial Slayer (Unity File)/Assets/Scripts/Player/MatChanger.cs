using UnityEngine;

public class MatChanger : MonoBehaviour
{
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private bool changeMat;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    private int MatsSize;
    private int currentMat;

    private void Start()
    {
        MatsSize = playerMaterials.Length;
        meshRenderer.material = playerMaterials[currentMat];
    }

    private void Update()
    {
        Debug.Log(changeMat);
        if (changeMat)
        {
            changeMat = false;
            currentMat++;
            if (currentMat == MatsSize)
            {
                currentMat = 0;
            }
            meshRenderer.material = playerMaterials[currentMat];

        }
    }
}
