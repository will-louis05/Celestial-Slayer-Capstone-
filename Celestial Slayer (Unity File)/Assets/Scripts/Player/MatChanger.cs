using UnityEngine;

public class MatChanger : MonoBehaviour
{
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private bool changeMat;
    [SerializeField] private MeshRenderer meshRenderer;
    private int MatsSize;
    private int currentMat;

    private void Start()
    {
        MatsSize = playerMaterials.Length;
    }

    private void Update()
    {
        if (changeMat)
        {
            MatsSize++;

        }
    }
}
