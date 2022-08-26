using UnityEngine;

[ExecuteAlways]
public class Cell : MonoBehaviour
{
    public CellType cellType;
    public Material material;
    public MeshRenderer meshRenderer;

    private void Awake ()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start ()
    {
        meshRenderer.material = material;
    }
}